using FlexCel.XlsAdapter;
using NLog;
using PayMem.RoadnetAnywhere;
using PayMem.RoadnetAnywhere.Apex;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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

                //var data = GetDataFromWebService();
                //WriteDataToExcelFile(filename, data);
            }
        }

        private async void Export_Button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_outputFilePathAndName))
            {
                MessageBox.Show("Please select an output file first.");
                return;
            }

            await ConnectToRNAServer();

            var testData = new List<ServiceLocationLocal>
            {
                new ServiceLocationLocal
                {
                    Location_ID = "1",
                    Location_Type = "Type1",
                    Description = "Test Description 1",
                    // ... continue for all your properties ...
                    Latitude = 123.456,
                    Longitude = 456.789
                },
                new ServiceLocationLocal
                {
                    Location_ID = "2",
                    Location_Type = "Type2",
                    Description = "Test Description 2",
                    // ... continue for all your properties ...
                    Latitude = 789.123,
                    Longitude = 123.456
                },
                // ... continue adding as many test data rows as you need ...
            };

            //var data = GetDataFromWebService();
            WriteDataToExcelFile(_outputFilePathAndName, testData);
        }

        private void WriteDataToExcelFile(string filename, IEnumerable<ServiceLocationLocal> data)
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
            foreach (var dataRow in data)
            {
                xls.SetCellValue(row, 1, dataRow.Location_ID);
                xls.SetCellValue(row, 2, dataRow.Location_Type);
                xls.SetCellValue(row, 3, dataRow.Description);
                xls.SetCellValue(row, 4, dataRow.Address_Line1);
                xls.SetCellValue(row, 5, dataRow.City);
                xls.SetCellValue(row, 6, dataRow.State);
                xls.SetCellValue(row, 7, dataRow.Zip_Code);
                xls.SetCellValue(row, 8, dataRow.County);
                xls.SetCellValue(row, 9, dataRow.Delivery_Days);
                xls.SetCellValue(row, 10, dataRow.WKLYFREQ);
                xls.SetCellValue(row, 11, dataRow.Phone);
                xls.SetCellValue(row, 12, dataRow.Priority);
                xls.SetCellValue(row, 13, dataRow.Zone);
                xls.SetCellValue(row, 14, dataRow.Instructions);
                xls.SetCellValue(row, 15, dataRow.Geocode_Quality);
                xls.SetCellValue(row, 16, dataRow.User_Modified);
                xls.SetCellValue(row, 17, dataRow.Date_Modified);
                xls.SetCellValue(row, 18, dataRow.Date_Added);
                xls.SetCellValue(row, 19, dataRow.Service_Time_Type);
                xls.SetCellValue(row, 20, dataRow.Last_Order_Date);
                xls.SetCellValue(row, 21, dataRow.Latitude);
                xls.SetCellValue(row, 22, dataRow.Longitude);

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

        public async Task ConnectToRNAServer()
        {
            // Connect to RNA Server
            Stopwatch sw = Stopwatch.StartNew();
            // _logger.Info("     Connecting to RNA Server...");

            connection = new ConnectionService();
            var connectionArgs = new ConnectionArgs
            {
                IsAdminUser = false,
                SupportedCustomerAlias = "test",
                SearchForSupportedCustomer = true,
                RnaEnvironment = RnaEnvironment.Production,
                UseRouteNavigatorClientId = false,
                //ServiceUser = "steve@SA.com",
                //Password = "Omnitracs123"
                ServiceUser = "customreports@sparksanalytics.com",
                Password = "Roadnet@123"   // sps

            };
            var result = await connection.InitAsync(connectionArgs);
            var regions = connection.GetRegions();
            var retriever = new RetrieverService(connection);

            serviceLocations = await retriever.RetrieveAllAsync<ServiceLocation>(new RetrievalOptions
            {
                PropertyInclusionMode = PropertyInclusionMode.All
            }, regions[0], progress);

            if (result.Status == ConnectionStatus.AuthenticationError)
            {
                _logger.Info("     Connecting to RNA Server Failed.");
                _logger.Info("     ServiceUser: " + connectionArgs.ServiceUser);
                _logger.Info("     Password: " + connectionArgs.Password);
            }
            else
            {
                _logger.Info("     Connected Successfully to RNA Server. Elapsed Seconds: " + sw.Elapsed.TotalSeconds);
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
