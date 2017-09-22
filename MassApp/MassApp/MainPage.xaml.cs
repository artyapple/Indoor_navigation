using MassApp.Models;
using MassApp.Models.Entities;
using Robotics.Mobile.Core.Bluetooth.LE;
using System;
using System.Collections.Generic;
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
        private readonly List<Beacon> _registeredBeaconList;

        public MainPage()
        {
            InitializeComponent();
            _bluetoothAdapter = DependencyService.Get<IAdapter>();
            _deviceList = new List<IDevice>();
            _registeredBeaconList = new List<Beacon>();

            _registeredBeaconList.Add(new Beacon
            {
                ID = Guid.Parse("00000000-0000-0000-0000-e807ca2f5bfb"),
                Name = "IR  WunderBar",
            });

            _registeredBeaconList.Add(new Beacon
            {
                ID = Guid.Parse("00000000-0000-0000-0000-cf817ea2cdb8"),
                Name = "HTU WunderBar"
            });

            _registeredBeaconList.Add(new Beacon
            {
                ID = Guid.Parse("00000000-0000-0000-0000-8866A5A7D65C"),
                Name = "IPhone"
            });
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
            var registered = _registeredBeaconList.SingleOrDefault(o => o.ID == e.Device.ID);
            if (registered != null)
            {
                var device = _bluetoothAdapter.ConnectedDevices.SingleOrDefault(o => o.ID == e.Device.ID);

                registered.Finded = true;
                registered.LastFindedDate = DateTime.Now;
                registered.Rssi = e.Device.Rssi;

                Print();
            }
        }

        private void Print()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                MyLabel.Text = $"ScanCount: {++count} \n";
                foreach (Beacon RegisteredBeacon in _registeredBeaconList)
                {
                    MyLabel.Text += $"\n {RegisteredBeacon.Name} {RegisteredBeacon.Rssi}dBm";
                }
            });
        }
    }
}
