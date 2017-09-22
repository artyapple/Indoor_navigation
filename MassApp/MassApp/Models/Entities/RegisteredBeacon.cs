using System;
using System.Collections.Generic;
using System.Text;

namespace MassApp.Models.Entities
{
    public class RegisteredBeacon
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public int Longtitude { get; set; }
        public int Laitude { get; set; }
    }
}
