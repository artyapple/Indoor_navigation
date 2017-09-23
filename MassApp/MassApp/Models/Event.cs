using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MassApp.Models
{
    [XmlRoot]
    public class Event
    {
        public string Id { get; set; }
    }
}
