using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using RobotManager.Core.ViewModels;

namespace RobotManager.Pages
{
	[MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, WrapInNavigationPage = true, NoHistory = true)]
	public partial class StartPage : MvxContentPage<StartViewModel>
	{
		public StartPage ()
		{
			InitializeComponent ();
		}
	}
}