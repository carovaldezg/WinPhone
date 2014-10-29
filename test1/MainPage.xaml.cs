using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using test1.Resources;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using bd1;
using Newtonsoft.Json;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Phone.Net.NetworkInformation;


namespace test1
{
    public partial class MainPage : PhoneApplicationPage
    {
        dbJsonMethods dbjson;
        gpsValues gpsvalues;
        Geolocator geolocator = null;
        sendDataToServer senddata;
        FilesManager filesmanager;
        gpsDataCollector gps;



        // Constructor
        public MainPage()
        {
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            gpsvalues = new gpsValues();
            dbjson = new dbJsonMethods();
            senddata = new sendDataToServer();
            filesmanager = new FilesManager();
            geolocator = new Geolocator();
            //THE FOLLOWING LINE IS FOR TEST PURPOSES, TO DELETE ALL THE DATA OF THE TABLE.
           // dbjson.delete();
            gps = new gpsDataCollector(this);
          
                InitializeComponent();
           

        }



       





        //Get the permission from the user to access to the GPS
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }


       
        //Click on show DB data button
        private void butondb_Click(object sender, RoutedEventArgs e)
        {
           List<gpsDataJasonTable> data = dbjson.getAllRows();
            String s="";
            for (int i = 0; i < data.Count; i++)
            {
                s = s +  data.ElementAt(i).position+"\n"; 
            }
            texboxdata.Visibility = Visibility.Visible;
            
            if (s == "")
                texboxdata.Text = "no data in the db";
            else
                texboxdata.Text = s;
        }
        //******************************************************************
        //                  GPS GET DATA METHODS
        //******************************************************************

        //Start to obtain the location info
        private void buttonstartloop_Click(object sender, RoutedEventArgs e)
        {
            
                gps.startCollectingData();
            
        }


        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters
                    status = "ready";
                    break;
                case PositionStatus.NotAvailable:
                    status = "not available";
                    // not used in WindowsPhone, Windows desktop uses this value to signal that there is no hardware capable to acquire location information
                    break;
                case PositionStatus.NotInitialized:
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state

                    break;
             }

            Dispatcher.BeginInvoke(() =>
            { 
               StatusTextBlock.Text = status;
            });
        }

        public static long UnixTimestampFromDateTime(DateTime date)
        {
            long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }

        void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                gpsvalues.latitude = args.Position.Coordinate.Latitude.ToString("0.00000000");
                gpsvalues.longitude = args.Position.Coordinate.Longitude.ToString("0.00000000");
                gpsvalues.accuracy = args.Position.Coordinate.Accuracy.ToString();
                DateTime now = new DateTime();
                now = DateTime.Now;
                String timestamp = (UnixTimestampFromDateTime(now)).ToString();
                gpsvalues.timestamp = timestamp;
                String row = JsonConvert.SerializeObject(gpsvalues);
                dbjson.addRow(row);
                
            });
        }


        //******************************************************************
        //                  END OF GPS GET DATA METHODS
        //******************************************************************

       

        private void buttonsend_Click(object sender, RoutedEventArgs e)
        {
            if (!dbjson.isEmpty())
                if (DeviceNetworkInformation.IsNetworkAvailable)
                {
                    List<gpsDataJasonTable> data = dbjson.getAllRows();
                    gpsDataJasonTable[] arraydata = data.ToArray();
                    //Get the actual day, it will be the file name
                    DateTime date = DateTime.Today;
                    String filename = date.ToString("yyyyMMdd");
                    filesmanager.createJsonFile(filename, arraydata);
                    filesmanager.ZipFilesFromStorage(filename, "data.zip");
                    byte[] datatosend = filesmanager.formatZipFileToSend("data.zip");
                    senddata.buffer = datatosend;
                    //Check the wifi on!
                    senddata.sendFileHttpPost();
                    //Check if the info was sent (ask how to do this control better)
                    if (senddata.getResponse() != null && senddata.getResponse().Trim().Contains("OK"))
                    {
                        filesmanager.deleteCreatedFiles("data.zip", filename);
                        //Delete the database info if it was sent
                        dbjson.delete();
                    }
                }
                else
                    MessageBox.Show("ERROR","there is no internet connection, the data cannot be send", MessageBoxButton.OK);
            else
                MessageBox.Show("ERROR", "There is no data to send in the database", MessageBoxButton.OK);
           
            
               
        }

       
/*


        //*******************************************************************************
        //  Send to the server methods
        //*******************************************************************************
        private void sendFileHttpPost()
        {
            System.Uri myUri = new System.Uri("http://beta.apisense.io/WindowsPhone/upload/data/caro/1.0/");
            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(myUri);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/octet-stream";
            myRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback),myRequest);

        }

        void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            // End the stream request operation
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);
            
            


       //     postStream.Write(b1, 0, b1.Length);
            postStream.Close();
 
    // Start the web request
         myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);

        }


        //Get the response from the server
        void GetResponsetStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult);
            using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
            {
                string result = httpWebStreamReader.ReadToEnd();
                //show results
                MessageBox.Show(result);
            }
        }
        //*******************************************************************************
        // End send to the server methods
        //*******************************************************************************

       
        */

}//END CLASS

    
}