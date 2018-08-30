using RobotManager.Core.DI;
using RobotManager.Interfaces;
using RobotManager.Services;

namespace RobotManager.DI
{
	public class FormsDiModule
	{
		public static void LoadTo(IDiResolver resolver)
		{
			resolver.Register<MenuService, IMenuService>();
		}
	}
}