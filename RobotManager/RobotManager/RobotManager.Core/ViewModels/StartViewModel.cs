using RobotManager.Core.Services.Interfaces;

namespace RobotManager.Core.ViewModels
{
	public class StartViewModel : ViewModelBase
	{
		public StartViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public string ConnectButtonText => TextSource.GetText("Connect");
	}
}