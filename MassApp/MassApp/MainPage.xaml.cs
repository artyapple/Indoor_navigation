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
        private List<Position> _collectedPositions;
        private const string FileName = "indoor_navigation_log";
        private readonly IStorage _storage;
        private readonly Position _maxPosition;
        private Position _lastPosition;
        private Position _targetPosition;
        private bool _isGoingUp;

        public MainPage()
        {
            InitializeComponent();
            _storage = DependencyService.Get<IStorage>();
            _bluetoothAdapter = DependencyService.Get<IAdapter>();
            _deviceList = new List<IDevice>();
            _beaconList = new List<Beacon>();
            _collectedPositions = new List<Position>();
            _lastPosition = new Position();
            _targetPosition = new Position();
            _isGoingUp = false;

            MyImage.Source = ImageSource.FromFile("bckg.png");

            string toDeserialize = BeaconDataMock.Json;
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            DataSyncResponse response = JsonConvert.DeserializeObject<DataSyncResponse>(toDeserialize, serializerSettings);

            _registeredBeaconList = response.Beacons;
            _maxPosition = response.MaxMapPosition;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MyImage.MinimumHeightRequest = Application.Current.MainPage.Height;
            MyImage.MinimumWidthRequest = Application.Current.MainPage.Width;

            ArrowImage.Margin = new Thickness(150, Application.Current.MainPage.Height / 2);
            //MyLabel.Text = "scanning...";
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
                        RegisteredBeacon = registered
                    };
                    _beaconList.Add(beacon);
                }

                beacon.Finded = true;
                beacon.LastFindedDate = DateTime.Now;
                beacon.Rssi = Math.Abs(e.Device.Rssi);

                Position actualPosition = CalculateActualPosition();
                if(actualPosition != null)
                {
                    bool goUp = (ArrowImage.TranslationY / Application.Current.MainPage.Height * _maxPosition.ValueY - actualPosition.ValueY) >= 0;
                    if (goUp != _isGoingUp)
                    {
                        ArrowImage.Rotation = goUp ? 180 : 0;
                        _isGoingUp = !_isGoingUp;
                    }
                    _lastPosition = actualPosition;
                }


                double verticalPos = Application.Current.MainPage.Height * _lastPosition.ValueY / _maxPosition.ValueY;
                //int padding = 50;
                //ArrowImage.Margin = new Thickness((Application.Current.MainPage.Width - padding) / 2, verticalPos);
                ArrowImage.TranslateTo(0, verticalPos, 2000);

                double diffToTarget = _targetPosition.ValueY - _lastPosition.ValueY;
                if (diffToTarget != 0)
                {
                    //TargetArrowImage.Rotation = diffToTarget > 0 ? 0 : 180;
                    TargetArrowImage.RotateTo(diffToTarget > 0 ? 0 : 180, 500);
                }



                PrintPosition(_lastPosition);

                string path = _storage.AppExternalPath;
                string filename = Path.Combine(path, FileName);

                string datum = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff");
                string text = $"{datum} {beacon.Name} {beacon.Rssi}";
                string readed = string.Empty;


                if (File.Exists(filename))
                    readed = File.ReadAllText(filename);

                File.WriteAllText(filename, readed + "\n" + text);

                Print();
            }
        }

        private Position CalculateActualPosition()
        {
            Beacon beacon = _beaconList.Where(o => o.Rssi < 65).OrderBy(o => o.Rssi).FirstOrDefault();

            if(beacon == null)
            {
                return null;
            }

            return new Position
            {
                ValueX = beacon.RegisteredBeacon.PositionX,
                ValueY = beacon.RegisteredBeacon.PositionY
            };

            //List<Position> positions = new List<Position>();
            //List<Beacon> beacons = Get3NearstesBeacons();

            //if (beacons.Count <= 1)
            //{
            //    return null;
            //}
            //for (int i = 0; i < beacons.Count - 1; i++)
            //{
            //    for (int j = i + 1; j < beacons.Count; j++)
            //    {
            //        positions.AddRange(GetPosition(beacons[i], beacons[j]));
            //    }
            //}

            //// filter positions
            //int deviation = 2; // in meter
            //List<List<Position>> mathedPositions = new List<List<Position>>();
            //foreach (var position in positions)
            //{
            //    int group = -1;
            //    for (int i = 0; i < mathedPositions.Count; i++)
            //    {
            //        Position match = mathedPositions[i][0];
            //        if (Math.Abs(match.ValueX - position.ValueX) <= deviation
            //            && Math.Abs(match.ValueY - position.ValueY) <= deviation)
            //        {
            //            group = i;
            //            i = mathedPositions.Count;
            //        }
            //    }

            //    if (group != -1)
            //    {
            //        mathedPositions[group].Add(position);
            //    }
            //    else
            //    {
            //        List<Position> match = new List<Position>();
            //        match.Add(position);
            //        mathedPositions.Add(match);
            //    }
            //}

            //// find max matched group and calculate average
            //var orderedMatchedPositions = mathedPositions.OrderByDescending(o => o.Count).ToList();
            //if (orderedMatchedPositions.Any())
            //{
            //    List<Position> corectPositions = orderedMatchedPositions[0];
            //    double x = corectPositions.Select(o => o.ValueX).Average();
            //    double y = corectPositions.Select(o => o.ValueY).Average();

            //    return new Position
            //    {
            //        ValueX = x,
            //        ValueY = y
            //    };
            //}

            //return null;
        }

        private List<Beacon> Get3NearstesBeacons()
        {
            return _beaconList.OrderByDescending(o => o.Rssi).Take(3).ToList();
        }

        private List<Position> GetPosition(Beacon beacon1, Beacon beacon2)
        {
            List<Position> result = new List<Position>();
            int pow = 2;
            double x1 = beacon1.RegisteredBeacon.PositionX;
            double x2 = beacon2.RegisteredBeacon.PositionX;
            double y1 = beacon1.RegisteredBeacon.PositionY;
            double y2 = beacon2.RegisteredBeacon.PositionY;
            double distance1 = DecibelToMeter(beacon1.Rssi, beacon1.RegisteredBeacon.DbFrom1Meter);
            double distance2 = DecibelToMeter(beacon2.Rssi, beacon2.RegisteredBeacon.DbFrom1Meter);

            beacon1.DistanceTouser = distance1;
            beacon2.DistanceTouser = distance2;

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

            if (resultX1 >= 0 && resultY1 >= 0
                && resultX1 <= _maxPosition.ValueX && resultY1 <= _maxPosition.ValueY)
            {
                result.Add(new Position
                {
                    ValueX = resultX1,
                    ValueY = resultY1
                });
            }
            if (resultX2 >= 0 && resultY2 >= 0
                 && resultX2 <= _maxPosition.ValueX && resultY2 <= _maxPosition.ValueY)
            {
                result.Add(new Position
                {
                    ValueX = resultX2,
                    ValueY = resultY2
                });
            }

            return result;
        }

        private double DecibelToMeter(double decibel, double decibelEtalon)
        {
            decibel = Math.Abs(decibel);
            if (decibel < 46)
                return 0.6;
            else if(decibel < 56)
                return 0.9;
            else if (decibel < 58)
                return 1.2;
            else if (decibel < 60)
                return 1.5;
            else if (decibel < 63)
                return 1.8;
            else if (decibel < 70)
                return 2.4;
            else
                return 3;

            //if (decibel == 0)
            //{
            //    return -1; // Невозможно определить расстояние
            //}
            //double ratio = (decibel - 2)/ decibelEtalon;
            //if (ratio < 1)
            //{
            //    return Math.Pow(ratio, 10);
            //}
            //else
            //{
            //    //return 0.2 * Math.Pow(ratio, 12) + 1;   // magic epta
            //    return 0.89976 * Math.Pow(ratio, 7.7095) + 0.111;
            //}
        }

        private void PrintPosition(Position position)
        {
            //Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            //{
            //    MyLabel2.Text = string.Empty;
            //    if (position != null)
            //    {
            //    MyLabel2.Text += $"x: {position.ValueX} y: {position.ValueY}";
            //    }
            //    else
            //    {
            //        MyLabel2.Text = "position is null";
            //    }
            //});
        }

        private void Print()
        {
            //Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            //{
            //    //MyLabel.Text = $"ScanCount: {++count} \n";
            //    //foreach (Beacon beacon in _beaconList)
            //    //{
            //    //    MyLabel.Text += $"\n {beacon.Name} {beacon.Rssi}dBm";
            //    //}
            //});
        }
    }
}
