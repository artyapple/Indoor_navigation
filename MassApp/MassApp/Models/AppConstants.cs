using System;
using System.Collections.Generic;
using System.Text;

namespace MassApp.Models
{
    public class AppConstants
    {
        public struct Services
        {
            public static Guid DeviceService => Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb");
            public static Guid BatteryService => Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb");
        }
    }
}
