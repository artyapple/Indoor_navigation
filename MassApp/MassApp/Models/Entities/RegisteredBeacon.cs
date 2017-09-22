using System;
using System.Collections.Generic;
using System.Text;

namespace MassApp.Models.Entities
{
    public class RegisteredBeacon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public int DbFrom1Meter { get; set; }
    }
}
