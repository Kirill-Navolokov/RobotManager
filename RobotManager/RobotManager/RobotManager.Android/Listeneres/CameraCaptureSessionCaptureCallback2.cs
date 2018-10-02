using Android.Hardware.Camera2;
using RobotManager.Droid.CustomRenderer;

namespace RobotManager.Droid.Listeneres
{
	public class CameraCaptureSessionCaptureCallback2 : CameraCaptureSession.CaptureCallback
	{
		public CameraRenderer Parent { get; private set; }

		public CameraCaptureSessionCaptureCallback2(CameraRenderer parent)
		{
			Parent = parent;
		}


		public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
		{
			Parent.UnlockFocus();
		}
	}
}