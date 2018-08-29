using MvvmCross.Commands;
using IServiceProvider = RobotManager.Core.Services.Interfaces.IServiceProvider;

namespace RobotManager.Core.ViewModels
{
	public class MenuViewModel : ViewModelBase
	{
		public MenuViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			GoToSettingsPageCommand = new MvxCommand(OnSettingsCommand);
			GoToAboutPageCommand = new MvxCommand(OnAboutCommand);
		}

		public string SettingsTitle => TextSource.GetText("Settings");

		public string AboutTitle => TextSource.GetText("About");

		public MvxCommand GoToSettingsPageCommand { get; }

		public MvxCommand GoToAboutPageCommand { get; }

		private void OnSettingsCommand()
		{
			NavigationService.Navigate<SettingsViewModel>();
		}

		private void OnAboutCommand()
		{
			NavigationService.Navigate<AboutViewModel>();
		}
	}
}