using MvvmCross.Localization;

namespace RobotManager.Core.Services.Implementation
{
	public class ServiceProvider : Interfaces.IServiceProvider
	{
		public ServiceProvider(IMvxLanguageBinder languageBinder)
		{
			LanguageBinder = languageBinder;
		}

		public IMvxLanguageBinder LanguageBinder { get; }
	}
}