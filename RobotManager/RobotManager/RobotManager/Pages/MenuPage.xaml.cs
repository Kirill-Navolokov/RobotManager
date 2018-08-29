using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using RobotManager.Core.ViewModels;

namespace RobotManager.Pages
{
	[MvxMasterDetailPagePresentation(MasterDetailPosition.Master, WrapInNavigationPage = false, NoHistory = true)]
	public partial class MenuPage : MvxContentPage<MenuViewModel>
	{
		public MenuPage ()
		{
			InitializeComponent ();
		}
	}
}