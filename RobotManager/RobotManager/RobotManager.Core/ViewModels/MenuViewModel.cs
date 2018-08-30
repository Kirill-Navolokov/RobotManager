using System.Threading.Tasks;
using MvvmCross.Commands;
using IServiceProvider = RobotManager.Core.Services.Interfaces.IServiceProvider;

namespace RobotManager.Core.ViewModels
{
	public class MenuViewModel : ViewModelBase
	{
		public MenuViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			StartCommand = new MvxAsyncCommand(OnStartCommand);
			SettingsCommand = new MvxAsyncCommand(OnSettingsCommand);
			AboutCommand = new MvxAsyncCommand(OnAboutCommand);
		}

		public string SettingsTitle => TextSource.GetText("Settings");

		public string AboutTitle => TextSource.GetText("About");

		public string StartTitle => TextSource.GetText("Start");

		public MvxAsyncCommand SettingsCommand { get; }

		public MvxAsyncCommand AboutCommand { get; }

		public MvxAsyncCommand StartCommand { get; }

		private async Task OnSettingsCommand()
		{
			await NavigateTo<SettingsViewModel>();
		}

		private async Task OnAboutCommand()
		{
			await NavigateTo<AboutViewModel>();
		}

		private async Task OnStartCommand()
		{
			await NavigateTo<StartViewModel>();
		}
	}
}