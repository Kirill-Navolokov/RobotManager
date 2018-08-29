using MvvmCross.Forms.Views;
using RobotManager.Core.ViewModels;
using Xamarin.Forms.Xaml;

namespace RobotManager.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : MvxContentPage<AboutViewModel>
	{
		public AboutPage ()
		{
			InitializeComponent ();
		}
	}
}