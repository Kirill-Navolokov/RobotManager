using IServiceProvider = RobotManager.Core.Services.Interfaces.IServiceProvider;

namespace RobotManager.Core.ViewModels
{
	public class ManagementViewModel : ViewModelBase
	{
		public ManagementViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}
	}
}