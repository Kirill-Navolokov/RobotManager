using MvvmCross.Localization;

namespace RobotManager.Core.Services.Interfaces
{
	public interface IServiceProvider
	{
		IMvxLanguageBinder LanguageBinder { get; }
	}
}