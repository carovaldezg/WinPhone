using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Devices.Geolocation;

namespace test1
{
    class gpsDataCollector
    {
        Geolocator geolocator = null;
        gpsValues gpsvalues;
        dbJsonMethods dbjson;
        bool tracking = false;
        MainPage window;
        public gpsDataCollector(MainPage win)
        {
            if (App.geolocator == null)
                geolocator = new Geolocator();
            else
                geolocator = App.geolocator;
            gpsvalues = new gpsValues();
            dbjson = new dbJsonMethods();
            window = win;
        }

        public void startCollectingData()
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            if (!tracking)
            {
                
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                geolocator.MovementThreshold = 50; // The units are meters.

                geolocator.StatusChanged += geolocator_StatusChanged;
                geolocator.PositionChanged += geolocator_PositionChanged;
                if (!App.RunningInBackground)
                   window.buttonstartloop.Content = "Stop";
          

                tracking = true;
                
            }
            else
            {
                if (!App.RunningInBackground)
                    window.buttonstartloop.Content = "Track Location";
                geolocator.PositionChanged -= geolocator_PositionChanged;
                geolocator.StatusChanged -= geolocator_StatusChanged;
              //  geolocator = null;
                tracking = false;
               
            }
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

           
          
      
        }

        public static long UnixTimestampFromDateTime(DateTime date)
        {
            long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }

        void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
       //     Dispatcher.BeginInvoke(() =>
         //   {
                gpsvalues.latitude = args.Position.Coordinate.Latitude.ToString("0.00000000");
                gpsvalues.longitude = args.Position.Coordinate.Longitude.ToString("0.00000000");
                gpsvalues.accuracy = args.Position.Coordinate.Accuracy.ToString();
                DateTime now = new DateTime();
                now = DateTime.Now;
                String timestamp = (UnixTimestampFromDateTime(now)).ToString();
                gpsvalues.timestamp = timestamp;
                String row = JsonConvert.SerializeObject(gpsvalues);
                dbjson.addRow(row);

           // });
        }

        //******************************************************************
        //                  GPS Functions
        //******************************************************************
        public Boolean isGpsEnable()
        {
            ShellToast toast = new ShellToast();
            toast.Title = "[title]";
            toast.Content = tracking.ToString();
            toast.Show();
            return tracking;
        }

  /*      public Boolean onLocationChanged(String provider, int distance, int period)
        {


        }
        */
        //******************************************************************
        //                  END GPS functions
        //******************************************************************

        

    }
}
