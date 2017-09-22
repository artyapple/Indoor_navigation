using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using MassApp.Common;
using MassApp.Droid.Common;

namespace MassApp.Droid
{
	[Activity (Label = "MassApp", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar; 

			base.OnCreate (bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            RegisterDependencies();

            global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new MassApp.App ());
		}

        void RegisterDependencies()
        {
            DependencyService.Register<IStorage, StorageConcreate>();
        }
    }
}

