using IServiceProvider = RobotManager.Core.Services.Interfaces.IServiceProvider;

namespace RobotManager.Core.ViewModels
{
	public class MenuViewModel : ViewModelBase
	{
		public MenuViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}
	}
}