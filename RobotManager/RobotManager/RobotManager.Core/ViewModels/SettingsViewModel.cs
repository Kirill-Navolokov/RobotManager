using IServiceProvider = RobotManager.Core.Services.Interfaces.IServiceProvider;

namespace RobotManager.Core.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public SettingsViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}
	}
}