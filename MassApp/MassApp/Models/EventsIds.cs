using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MassApp.Models
{
    [XmlRoot]
    public class EventsIds
    {
        public EventsIds()
        {
            Event = new List<Models.Event>();
        }

        [XmlElement]
        public List<Event> Event { get; set; }
    }
}
