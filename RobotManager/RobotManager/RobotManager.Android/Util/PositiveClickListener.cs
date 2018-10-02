using Android;
using Android.Content;
using Android.Support.V4.App;
using RobotManager.Droid.CustomRenderer;

namespace RobotManager.Droid.Util
{
	internal class PositiveClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
	{
		private readonly ConfirmationDialog _confirmationDialog;

		public PositiveClickListener(ConfirmationDialog confirmationDialog)
		{
			_confirmationDialog = confirmationDialog;
		}

		public void OnClick(IDialogInterface dialog, int which)
		{
			ActivityCompat.RequestPermissions(_confirmationDialog.Activity,
				new[] { Manifest.Permission.Camera },
				CameraRenderer.RequestCameraPermissionCode);
		}
	}
}