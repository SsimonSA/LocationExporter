using FlexCel.XlsAdapter;
using NLog;
using PayMem.RoadnetAnywhere;
using PayMem.RoadnetAnywhere.Apex;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace LocationExporter
{
    public partial class LocationExportDialog : Form
    {
        private string _outputDirectory;
        public ConnectionService connection;
        public Dictionary<long, ServiceLocation> serviceLocations = new Dictionary<long, ServiceLocation>();
        private IProgress<RetrieverProgress> progress;
        public class RegionInfo
        {
            public string Display { get; set; }
            public PayMem.RoadnetAnywhere.Apex.Region Region { get; set; }
        }
        public List<RegionInfo> regionsInfo = new List<RegionInfo>();
        public int activeRegionIx = 0;
        public Region activeRegion;

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public LocationExportDialog()
        {
            InitializeComponent();

            this.Load += new EventHandler(LocationExportDialog_Load);

            string version = Application.ProductVersion;

            string fullAppDecription = "Breakthru Beverage Location Exporter v" + version;

            this.Text = fullAppDecription;

            _logger.Info(fullAppDecription);

            _outputDirectory = Properties.Settings.Default.OutputDirectory;

            // Set the range of the progress bar.
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            progress = new Progress<RetrieverProgress>(p =>
            {
                progressBar1.Value = (int)(p.RecordsRetrievedInBusinessUnitCount*100 / p.TotalRecordsInBusinessUnitCount);
            });

        }

        private async void LocationExportDialog_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            label_OutputDirectory.Text = "Output Directory: " + _outputDirectory;

            // Connect to RNA Server
            Stopwatch sw = Stopwatch.StartNew();
            _logger.Info("     Connecting to RNA Server...");
            progressLabel.Text = "Connecting to RNA Server...";

            connection = new ConnectionService();

            PasswordUtility passwordUtility = new PasswordUtility();
            string password = passwordUtility.GetPassword();

            if (password.Equals(""))
            {
                password = "#Omnitracs123";
            }

            var connectionArgs = new ConnectionArgs
            {
                IsAdminUser = false,
                SupportedCustomerAlias = "test",
                SearchForSupportedCustomer = true,
                RnaEnvironment = RnaEnvironment.Production,
                UseRouteNavigatorClientId = false,
                ServiceUser = "steve2@SA.com",
                Password = password
            };
            var result = await connection.InitAsync(connectionArgs);

            if (result.Status == ConnectionStatus.AuthenticationError)
            {
                _logger.Info("     Connecting to RNA Server Failed.");
                _logger.Info("     ServiceUser: " + connectionArgs.ServiceUser);
                _logger.Info("     Password: " + connectionArgs.Password);
                progressLabel.Text = $"Connection to RNA Server failed. Check User {connectionArgs.ServiceUser}'s password.";
                MessageBox.Show($"Logging in to RNA Server failed.  Make sure WebServices User: {connectionArgs.ServiceUser} is setup and that the password is set properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var regions = connection.GetRegions();
                foreach (var r in regions)
                {
                    RegionInfo regionInfo = new RegionInfo
                    {
                        Display = r.Identifier + " - " + r.Description,
                        Region = r
                    };
                    regionsInfo.Add(regionInfo);
                }
                regionsInfo = regionsInfo.OrderBy(o => o.Display).ToList();

                comboBox_Regions.DisplayMember = "Display";
                comboBox_Regions.DataSource = regionsInfo;
                activeRegionIx = Properties.Settings.Default.ActiveRegionIx;
                comboBox_Regions.SelectedIndex = activeRegionIx;

                _logger.Info("     Connected Successfully to RNA Server. Elapsed Seconds: " + sw.Elapsed.TotalSeconds);
                progressLabel.Text = "Connection to RNA Server successful.  Ready for Export.";
            }
            Cursor = Cursors.Default;
        }

        private void Quit_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.OutputDirectory = _outputDirectory;
            Properties.Settings.Default.ActiveRegionIx = activeRegionIx;
            Properties.Settings.Default.Save();
            Application.Exit();
        }

        private void FileChooser_Button_Click(object sender, EventArgs e)
        {
            string selectedPath = _outputDirectory;
            var t = new Thread((ThreadStart)(() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog
                {
                    RootFolder = System.Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = true,
                    SelectedPath = _outputDirectory
                };
                if (fbd.ShowDialog() == DialogResult.Cancel)
                    return;

                selectedPath = fbd.SelectedPath;
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            _outputDirectory = selectedPath;
            label_OutputDirectory.Text = "Output Directory: " + _outputDirectory;
        }

        private async void Export_Button_Click(object sender, EventArgs e)
        {
            Export_Button.Enabled = false;
            if (string.IsNullOrEmpty(_outputDirectory))
            {
                MessageBox.Show("Please select an output directory first.");
                return;
            }
            Cursor = Cursors.WaitCursor;

            await RetrieveLocationInfoFromRNA();

            progressLabel.Text = "Writing Location Information to Excel...";
            string fullFilename = _outputDirectory + @"\ServiceLocations - " + activeRegion.Identifier + ".xls";
            WriteDataToExcelFile(fullFilename);

            progressLabel.Text = "Location Export Complete.";

            Export_Button.Enabled = true;
            Cursor = Cursors.Default;
        }

        private void WriteDataToExcelFile(string filename)
        {
            var xls = new XlsFile(true);
            xls.NewFile(1); // Creates a new Excel file with one worksheet.

            var columnNames = new List<string>
            {
                "Location_ID",
                "Location_Type",
                "Description",
                "Address_Line1",
                "City",
                "State",
                "Zip_Code",
                "County",
                "Delivery_Days",
                "WKLYFREQ",
                "Phone",
                "Priority",
                "Zone",
                "Instructions",
                "Geocode_Quality",
                "User_Modified",
                "Date_Modified",
                "Date_Added",
                "Service_Time_Type",
                "Last_Order_Date",
                "Latitude",
                "Longitude"
            };

            // Add column names
            for (int i = 0; i < columnNames.Count; i++)
            {
                xls.SetCellValue(1, i + 1, columnNames[i]);
            }

            // Add data
            int row = 2;
            foreach (var location in serviceLocations)
            {
                xls.SetCellValue(row, 1, location.Value.Identifier);
                xls.SetCellValue(row, 2, "SIT");
                xls.SetCellValue(row, 3, location.Value.Description);
                xls.SetCellValue(row, 4, location.Value.Address.AddressLine1);
                xls.SetCellValue(row, 5, location.Value.Address.Locality.AdminDivisionCity);
                xls.SetCellValue(row, 6, location.Value.Address.Locality.AdminDivision1);
                xls.SetCellValue(row, 7, location.Value.Address.Locality.PostalCode);
                xls.SetCellValue(row, 8, location.Value.Address.Locality.AdminDivision2);
                xls.SetCellValue(row, 9, location.Value.DayOfWeekFlags_DeliveryDays);
                if (location.Value.CustomProperties != null)
                {
                    if (location.Value.CustomProperties.ContainsKey("WKLYFREQ"))
                        if (location.Value.CustomProperties["WKLYFREQ"] != null)
                            xls.SetCellValue(row, 10, location.Value.CustomProperties["WKLYFREQ"].ToString());
                }
                xls.SetCellValue(row, 11, location.Value.PhoneNumber);
                xls.SetCellValue(row, 12, location.Value.Priority);
                xls.SetCellValue(row, 13, location.Value.Zone);
                xls.SetCellValue(row, 14, location.Value.StandardInstructions);
                xls.SetCellValue(row, 15, location.Value.GeocodeMethod_GeocodeMethod.ToString());
                xls.SetCellValue(row, 16, location.Value.ModifiedBy);
                xls.SetCellValue(row, 17, location.Value.ModifiedTime.ToLocalTime());
                xls.SetCellValue(row, 18, location.Value.ModifiedTime.ToShortDateString());
                xls.SetCellValue(row, 19, location.Value.ServiceTimeTypeIdentifier);
                xls.SetCellValue(row, 20, location.Value.LastOrderDate);
                xls.SetCellValue(row, 21, location.Value.Coordinate.Latitude);
                xls.SetCellValue(row, 22, location.Value.Coordinate.Longitude);
                
                row++;
            }

            try
            {
                xls.Save(filename);
            }

            catch (IOException)
            {
                MessageBox.Show("The file could not be saved because it is open in another program. Please close the file and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task RetrieveLocationInfoFromRNA()
        {
            var retriever = new RetrieverService(connection);
            progressLabel.Text = "Retrieving Location Information...";

            serviceLocations = await retriever.RetrieveAllAsync<ServiceLocation>(new RetrievalOptions
            {
                PropertyInclusionMode = PropertyInclusionMode.All
            }, activeRegion, progress);

            progressLabel.Text = "Retrieving Location Complete";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            activeRegionIx = comboBox_Regions.SelectedIndex;
            activeRegion = regionsInfo[activeRegionIx].Region;
        }

        private void Setup_button_Click(object sender, EventArgs e)
        {
            PasswordInputForm passwordInputForm = new PasswordInputForm();
            passwordInputForm.ShowDialog();
        }
    }
}
