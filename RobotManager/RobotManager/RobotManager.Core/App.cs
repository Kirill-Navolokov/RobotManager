using MvvmCross;
using MvvmCross.Localization;
using MvvmCross.Plugin.ResxLocalization;
using MvvmCross.ViewModels;
using RobotManager.Core.Resources;
using RobotManager.Core.Services.Implementation;
using RobotManager.Core.Services.Interfaces;
using RobotManager.Core.ViewModels;

namespace RobotManager.Core
{
	public class App : MvxApplication
	{
		public override void Initialize()
		{
			base.Initialize();

			RegisterDependencies();

			RegisterAppStart<MainViewModel>();
		}

		/// <summary>
		/// Setup IoC container
		/// </summary>
		private void RegisterDependencies()
		{
			Mvx.RegisterSingleton<IMvxTextProvider>(new MvxResxTextProvider(CommonResources.ResourceManager));
			Mvx.RegisterSingleton<IMvxLanguageBinder>(new MvxLanguageBinder(string.Empty, string.Empty));
			Mvx.RegisterType<IServiceProvider, ServiceProvider>();
		}
	}
}