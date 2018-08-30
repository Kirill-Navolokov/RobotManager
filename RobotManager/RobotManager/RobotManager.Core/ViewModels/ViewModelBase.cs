using System.Threading.Tasks;
using MvvmCross.Localization;
using MvvmCross.ViewModels;
using RobotManager.Core.Services.Interfaces;

namespace RobotManager.Core.ViewModels
{
	public class ViewModelBase : MvxViewModel
	{
		public ViewModelBase(IServiceProvider serviceProvider)
		{
			TextSource = serviceProvider.LanguageBinder;
		}

		public string Title { get; set; }

		protected IMvxLanguageBinder TextSource { get; }

		protected async Task NavigateTo<T>() where T : ViewModelBase
		{
			await NavigationService.Navigate<T>();
		}
	}
}