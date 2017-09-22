using MassApp.Models.Entities;
using Robotics.Mobile.Core.Bluetooth.LE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassApp.Models
{
    public class Beacon : RegisteredBeacon
    { 
        public int Rssi { get; set; }
        public bool Finded { get; set; }
        public DateTime LastFindedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Connected { get; set; }
        public DateTime LastConnectedDate { get; set; }
        public DateTime LastDisconnectedDate { get; set; }
        public int Battery { get; set; }

        //public Beacon(RegisteredBeacon registeredBeacon, IDevice device)
        //{
        //    ID = Guid.NewGuid();
        //    CreatedDate = DateTime.Now;
        //    LastConnectedDate = DateTime.Now;
        //    Connected = true;
        //}
    }
}
