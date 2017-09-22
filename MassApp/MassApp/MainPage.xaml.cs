using MassApp.Common;
using MassApp.Models;
using MassApp.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Robotics.Mobile.Core.Bluetooth.LE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MassApp
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private List<IDevice> _deviceList;
        private readonly IAdapter _bluetoothAdapter;
        private readonly List<RegisteredBeacon> _registeredBeaconList;
        private readonly List<Beacon> _beaconList;
        private const string FileName = "indoor_navigation_log";
        private readonly IStorage _storage;

        public MainPage()
        {
            InitializeComponent();
            _storage = DependencyService.Get<IStorage>();
            _bluetoothAdapter = DependencyService.Get<IAdapter>();
            _deviceList = new List<IDevice>();
            _beaconList = new List<Beacon>();

            string toDeserialize = BeaconDataMock.Json;
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            DataSyncResponse response = JsonConvert.DeserializeObject<DataSyncResponse>(toDeserialize, serializerSettings);

            _registeredBeaconList = response.Beacons;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MyLabel.Text = "scanning...";
            _bluetoothAdapter.DeviceDiscovered += BluetoothAdapter_DeviceDiscovered;
            _bluetoothAdapter.ScanTimeoutElapsed += BluetoothAdapter_ScanTimeoutElapsed;
            _bluetoothAdapter.StartScanningForDevices();
        }

        private void BluetoothAdapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            _bluetoothAdapter.StartScanningForDevices();
        }

        private void BluetoothAdapter_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            var registered = _registeredBeaconList.SingleOrDefault(o => o.Id == e.Device.ID);
            if (registered != null)
            {
                var device = _bluetoothAdapter.ConnectedDevices.SingleOrDefault(o => o.ID == e.Device.ID);

                var beacon = _beaconList.SingleOrDefault(o => o.Id == e.Device.ID);
                if (beacon == null)
                {
                    beacon = new Beacon
                    {
                        Id = e.Device.ID,
                        Name = registered.Name,
                        RegisteredBeacon = beacon
                    };
                    _beaconList.Add(beacon);
                }

                beacon.Finded = true;
                beacon.LastFindedDate = DateTime.Now;
                beacon.Rssi = e.Device.Rssi;

                //string path = _storage.AppExternalPath;
                //string filename = Path.Combine(path, FileName);

                //string datum = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff");
                //string text = $"{datum} {e.Device.ID} {beacon.Rssi}";
                //string readed = string.Empty;

                //if(File.Exists(filename))
                //    readed = File.ReadAllText(filename);

                //File.WriteAllText(filename, readed + "\n" + text);

                Print();
            }
        }

        private Position GetPosition(Beacon beacon1, Beacon beacon2)
        {
            int pow = 2;
            double x1 = beacon1.RegisteredBeacon.PositionX;
            double x2 = beacon2.RegisteredBeacon.PositionX;
            double y1 = beacon1.RegisteredBeacon.PositionY;
            double y2 = beacon2.RegisteredBeacon.PositionY;
            Position result = new Position();
            double distance1 = DecibelToMeter(beacon1.Rssi, beacon1.RegisteredBeacon.DbFrom1Meter);
            double distance2 = DecibelToMeter(beacon2.Rssi, beacon2.RegisteredBeacon.DbFrom1Meter);

            double distanceBetweenBeacons = Math.Sqrt(Math.Pow(x2 - x1, pow) + Math.Pow(y2 - y1, pow));
            double b = ((distance2 * distance2) - (distance1 * distance1) + (distanceBetweenBeacons * distanceBetweenBeacons)) / (2 * distanceBetweenBeacons);
            double a = distanceBetweenBeacons - b;
            double h = Math.Sqrt((distance2 * distance2) - (b * b));
            double x0 = x1 + (a / distanceBetweenBeacons) * (x2 - x1); 
            double y0 = y1 + (a / distanceBetweenBeacons) * (y2 - y1);

            double resultX1 = x0 - ((y2 - y1) / distanceBetweenBeacons * h);
            double resultY1 = y0 + ((x2 - x1) / distanceBetweenBeacons * h);

            double resultX2 = x0 + ((y2 - y1) / distanceBetweenBeacons * h);
            double resultY2 = y0 - ((x2 - x1) / distanceBetweenBeacons * h);

            // TODO: filter better second value 

            if(resultX1 >= 0 && resultY1 >= 0)
            {
                result.ValueX = resultX1;
                result.ValueY = resultY1;
            }
            else
            {
                result.ValueX = resultX2;
                result.ValueY = resultY2;
            }

            return result;
        }

        private double DecibelToMeter(int decibel, int decibelEtalon)
        {
            if (decibel == 0)
            {
                return -1; // Невозможно определить расстояние
            }
            var ratio = decibel / decibelEtalon;
            if (ratio < 1)
            {
                return Math.Pow(ratio, 10);
            }
            else
            {
                return 0.89976 * Math.Pow(ratio, 7.7095) + 0.111;
            }
        }

        private void Print()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                MyLabel.Text = $"ScanCount: {++count} \n";
                foreach (Beacon beacon in _beaconList)
                {
                    MyLabel.Text += $"\n {beacon.Name} {beacon.Rssi}dBm";
                }
            });
        }
    }
}
