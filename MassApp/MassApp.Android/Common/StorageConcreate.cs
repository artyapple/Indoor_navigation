using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MassApp.Common;

namespace MassApp.Droid.Common
{
    public class StorageConcreate : IStorage
    {
        public string AppExternalPath => Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
    }
}