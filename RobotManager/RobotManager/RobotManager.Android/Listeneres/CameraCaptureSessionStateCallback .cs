﻿using System;
using Android.Hardware.Camera2;
using RobotManager.Droid.CustomRenderer;

namespace RobotManager.Droid.Listeneres
{
	public class CameraCaptureSessionStateCallback : CameraCaptureSession.StateCallback
	{
		public CameraRenderer Parent { get; private set; }

		public CameraCaptureSessionStateCallback(CameraRenderer parent)
		{
			Parent = parent;
		}

		public override void OnConfigured(CameraCaptureSession session)
		{
			// The camera is already closed
			if (null == Parent.CameraDevice)
			{
				return;
			}

			// When the session is ready, we start displaying the preview.
			Parent.CaptureSession = session;
			try
			{
				// Auto focus should be continuous for camera preview.
				Parent.PreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)
					ControlAFMode.ContinuousPicture);

				// Finally, we start displaying the camera preview.
				Parent.mPreviewRequest = Parent.PreviewRequestBuilder.Build();
				Parent.CaptureSession.SetRepeatingRequest(Parent.mPreviewRequest,
					Parent.CaptureCallback, Parent.BackgroundHandler);
			}
			catch (CameraAccessException e)
			{
				Parent.ShowToast("Failed");
			}

		}

		public override void OnConfigureFailed(CameraCaptureSession session)
		{
			throw new NotImplementedException();
		}
	}
}