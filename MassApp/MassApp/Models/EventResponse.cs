using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MassApp.Models
{
    [XmlRoot]
    public class EventResponse
    {
        [XmlElement]
        public EventsIds EventsIds { get; set; }
    }
}
