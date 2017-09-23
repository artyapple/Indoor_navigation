using System;
using System.Collections.Generic;
using System.Text;

namespace MassApp.Models
{
    public class EventInfo
    {
        public EventInfo(string name, string imagUrl)
        {
            Name = name;
            ImageUrl = imagUrl;
        }

        public string Name { get; private set; }
        public string ImageUrl { get; private set; }
    }
}
