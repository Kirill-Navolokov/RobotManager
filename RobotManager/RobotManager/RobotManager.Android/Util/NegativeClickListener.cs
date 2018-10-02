using Android.Content;

namespace RobotManager.Droid.Util
{
	internal class NegativeClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
	{
		private readonly ConfirmationDialog _confirmationDialog;

		public NegativeClickListener(ConfirmationDialog confirmationDialog)
		{
			_confirmationDialog = confirmationDialog;
		}

		public void OnClick(IDialogInterface dialog, int which)
		{
			var activity = _confirmationDialog.Activity;

			activity?.Finish();
		}
	}
}