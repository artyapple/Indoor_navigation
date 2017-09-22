using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace MassApp.Common
{
    public class BeaconDataMock
    {
        public static string Json
        {
            get
            {
                IStorage storage = DependencyService.Get<IStorage>();

                string path = Path.Combine(storage.AppExternalPath, "beacon.properties");
                return File.ReadAllText(path);
            }
        }
    }
}
