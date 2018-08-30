using MvvmCross;
using MvvmCross.Forms.Platforms.Android.Core;
using MvvmCross.Forms.Presenters;

namespace RobotManager.Droid
{
	public class Setup : MvxFormsAndroidSetup<Core.App, App>
	{
		protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter)
		{
			var formsPresenter = base.CreateFormsPagePresenter(viewPresenter);
			Mvx.RegisterSingleton(formsPresenter);

			return formsPresenter;
		}
	}
}