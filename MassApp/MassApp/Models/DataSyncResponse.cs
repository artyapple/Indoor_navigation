using MassApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassApp.Models
{
    public class DataSyncResponse
    {
        public List<RegisteredBeacon> Beacons { get; set; }
    }
}
