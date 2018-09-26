using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Forms.Platforms.Android.Views;
using RobotManager.Core.ViewModels;

namespace RobotManager.Droid
{
	[Activity(Label = "@string/app_name",
			Theme = "@style/MainTheme",
			ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : MvxFormsAppCompatActivity<RootViewModel>
	{
		internal static MainActivity Instance { get; private set; }

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			Instance = this;
		}
	}
}