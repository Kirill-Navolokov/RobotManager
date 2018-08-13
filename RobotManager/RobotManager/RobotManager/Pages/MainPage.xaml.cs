using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using RobotManager.Core.ViewModels;

namespace RobotManager.Pages
{
	[MvxMasterDetailPagePresentation(MasterDetailPosition.Root, WrapInNavigationPage = false, NoHistory = true)]
	public partial class MainPage : MvxMasterDetailPage<MainViewModel>
	{
		public MainPage()
		{
			InitializeComponent();
		}
	}
}