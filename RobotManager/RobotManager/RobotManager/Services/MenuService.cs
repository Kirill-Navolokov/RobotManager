using RobotManager.Interfaces;
using Xamarin.Forms;

namespace RobotManager.Services
{
	public class MenuService : IMenuService

	{
		public void CloseMenu()
		{
			if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
			{
				masterDetailPage.IsPresented = false;
			}
			else if (Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
			{
				nestedMasterDetail.IsPresented = false;
			}
		}
	}
}