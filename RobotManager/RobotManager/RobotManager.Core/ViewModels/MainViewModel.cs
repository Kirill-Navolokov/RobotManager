using System;
using System.Threading.Tasks;
using MvvmCross.ViewModels;
using IServiceProvider = RobotManager.Core.Services.Interfaces.IServiceProvider;

namespace RobotManager.Core.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private bool _isInitialized;

		public MainViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void ViewAppearing()
		{
			if (_isInitialized)
			{
				return;
			}

			MvxNotifyTask.Create(async () => await Init(), OnException);

			_isInitialized = true;
		}

		private async Task Init()
		{
			await NavigationService.Navigate<MenuViewModel>();
			await NavigationService.Navigate<StartViewModel>();
		}

		private void OnException(Exception ex)
		{
			//DialogService.ShowAlert(ex.Message);
		}
	}
}