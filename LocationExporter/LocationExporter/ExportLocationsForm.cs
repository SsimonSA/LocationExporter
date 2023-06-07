using FlexCel.XlsAdapter;
using NLog;
using PayMem.RoadnetAnywhere;
using PayMem.RoadnetAnywhere.Apex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace LocationExporter
{
    public partial class LocationExportDialog : Form
    {
        private string _outputFilePathAndName;
        public ConnectionService connection;
        public Dictionary<long, ServiceLocation> serviceLocations = new Dictionary<long, ServiceLocation>();
        private IProgress<RetrieverProgress> progress;

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public class ServiceLocationLocal
        {
            public string Location_ID { get; set; }
            public string Location_Type { get; set; }
            public string Description { get; set; }
            public string Address_Line1 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip_Code { get; set; }
            public string County { get; set; }
            public string Delivery_Days { get; set; }
            public string WKLYFREQ { get; set; }
            public string Phone { get; set; }
            public string Priority { get; set; }
            public string Zone { get; set; }
            public string Instructions { get; set; }
            public string Geocode_Quality { get; set; }
            public string User_Modified { get; set; }
            public string Date_Modified { get; set; }
            public string Date_Added { get; set; }
            public string Service_Time_Type { get; set; }
            public string Last_Order_Date { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public LocationExportDialog()
        {
            _logger.Info("Location Export Application Starting.");

            InitializeComponent();
            _outputFilePathAndName = Properties.Settings.Default.OutputFilePathAndName;
            FileDirAndName_TextBox.Text = _outputFilePathAndName;

            // Set the range of the progress bar.
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            progress = new Progress<RetrieverProgress>(p =>
            {
                progressBar1.Value = (int)(p.RecordsRetrievedInBusinessUnitCount*100 / p.TotalRecordsInBusinessUnitCount);
            });
        }

        private void Quit_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FileChooser_Button_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = _outputFilePathAndName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filename = saveFileDialog.FileName;
                FileDirAndName_TextBox.Text = filename;
                _outputFilePathAndName = filename;
                Properties.Settings.Default.OutputFilePathAndName = _outputFilePathAndName;
                Properties.Settings.Default.Save();
            }
        }

        private async void Export_Button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_outputFilePathAndName))
            {
                MessageBox.Show("Please select an output file first.");
                return;
            }
            Cursor = Cursors.WaitCursor;

            await RetrieveLocationInfoFromRNA();

            progressLabel.Text = "Writing Location Information to Excel...";
            WriteDataToExcelFile(_outputFilePathAndName);

            progressLabel.Text = "Location Export Complete.";

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
                if (location.Value.CustomProperties.ContainsKey("WKLYFREQ"))
                    if (location.Value.CustomProperties["WKLYFREQ"] != null)
                        xls.SetCellValue(row, 10, location.Value.CustomProperties["WKLYFREQ"].ToString());
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
            // Connect to RNA Server
            Stopwatch sw = Stopwatch.StartNew();
            _logger.Info("     Connecting to RNA Server...");
            progressLabel.Text = "Connecting to RNA Server...";

            connection = new ConnectionService();
            var connectionArgs = new ConnectionArgs
            {
                IsAdminUser = false,
                SupportedCustomerAlias = "test",
                SearchForSupportedCustomer = true,
                RnaEnvironment = RnaEnvironment.Production,
                UseRouteNavigatorClientId = false,
                ServiceUser = "steve2@SA.com",
                Password = "#Omnitracs123"
            };
            var result = await connection.InitAsync(connectionArgs);

            if (result.Status == ConnectionStatus.AuthenticationError)
            {
                _logger.Info("     Connecting to RNA Server Failed.");
                _logger.Info("     ServiceUser: " + connectionArgs.ServiceUser);
                _logger.Info("     Password: " + connectionArgs.Password);
            }
            else
            {
                _logger.Info("     Connected Successfully to RNA Server. Elapsed Seconds: " + sw.Elapsed.TotalSeconds);

                var regions = connection.GetRegions();
                var retriever = new RetrieverService(connection);

                int rix = regions.FindIndex(r => r.Identifier == "31");
                progressLabel.Text = "Retrieving Location Information...";

                if (rix != -1)
                {
                    serviceLocations = await retriever.RetrieveAllAsync<ServiceLocation>(new RetrievalOptions
                    {
                        PropertyInclusionMode = PropertyInclusionMode.All
                    }, regions[rix], progress);
                }
                progressLabel.Text = "Retrieving Location Complete";
            }
        }
    }
}
