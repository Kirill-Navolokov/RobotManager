using RobotManager.Core.Services.Interfaces;

namespace RobotManager.Core.ViewModels
{
	public class AboutViewModel : ViewModelBase
	{
		public AboutViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			Title = TextSource.GetText("About");
		}

		public string Content => TextSource.GetText("AboutPage_Content");
	}
}