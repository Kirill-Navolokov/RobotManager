using System.Threading.Tasks;
using MvvmCross.Commands;
using RobotManager.Core.Services.Interfaces;

namespace RobotManager.Core.ViewModels
{
	public class StartViewModel : ViewModelBase
	{
		public StartViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			SettingsCommand = new MvxAsyncCommand(OnSettingsCommand);
			ConnectCommand = new MvxAsyncCommand(OnConnectCommand);
		}

		public string ConnectButtonText => TextSource.GetText("Connect");

		public MvxAsyncCommand SettingsCommand { get; }

		public MvxAsyncCommand ConnectCommand { get; }

		private async Task OnSettingsCommand()
		{
			await NavigateTo<SettingsViewModel>();
		}

		private async Task OnConnectCommand()
		{
			await NavigateTo<ManagementViewModel>();
		}
	}
}