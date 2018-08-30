using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Forms.Platforms.Android.Views;
using RobotManager.Core.DI;
using RobotManager.DI;

namespace RobotManager.Droid
{
	[Activity(Label = "@string/app_name",
			Icon = "@drawable/icon_robot",
			Theme = "@style/MainTheme.Splash",
			MainLauncher = true,
			ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SplashActivity : MvxFormsSplashScreenActivity<Setup, Core.App, App>
	{
		protected override void RunAppStart(Bundle bundle)
		{
			FormsDiModule.LoadTo(DiResolver.Instance);
			StartActivity(typeof(MainActivity));
			base.RunAppStart(bundle);
		}
	}
}