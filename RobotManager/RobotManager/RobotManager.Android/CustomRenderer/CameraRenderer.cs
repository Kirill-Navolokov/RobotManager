using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using RobotManager.Droid.CustomRenderer;
using RobotManager.Droid.Listeneres;
using RobotManager.Droid.Util;
using RobotManager.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(ManagementPage), typeof(CameraRenderer))]
namespace RobotManager.Droid.CustomRenderer
{
	//Display Camera Stream: http://developer.xamarin.com/recipes/android/other_ux/textureview/display_a_stream_from_the_camera/
	//Camera Rotation: http://stackoverflow.com/questions/3841122/android-camera-preview-is-sideways
	public class CameraRenderer : PageRenderer
	{
		private View _view;
		public Activity Activity => Context as Activity;

		public CameraRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || Element == null)
			{
				return;
			}

			_mSurfaceTextureListener = new TextureViewSurfaceTextureListener(this);
			_mStateCallback = new CameraDeviceStateCallback(this);
			CaptureCallback = new CameraCaptureSessionCaptureCallback(this);

			_view = Activity.LayoutInflater.Inflate(Resource.Layout.camera_layout, this, false);
			TextureView = _view.FindViewById<TextureView>(Resource.Id.textureView);

			StartBackgroundThread();

			// When the screen is turned off and turned back on, the SurfaceTexture is already
			// available, and "onSurfaceTextureAvailable" will not be called. In that case, we can open
			// a camera and start preview from here (otherwise, we wait until the surface is ready in
			// the SurfaceTextureListener).
			if (TextureView.IsAvailable)
			{
				OpenCamera(TextureView.Width, TextureView.Height);
			}
			else
			{
				TextureView.SurfaceTextureListener = _mSurfaceTextureListener;
			}

			AddView(_view);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
			var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

			_view.Measure(msw, msh);
			_view.Layout(0, 0, r - l, b - t);
		}

		private void RequestCameraPermission()
		{
			if (ActivityCompat.ShouldShowRequestPermissionRationale(Activity, Manifest.Permission.Camera))
			{
				new ConfirmationDialog().Show(Activity.FragmentManager, FragmentDialog);
			}
			else
			{
				ActivityCompat.RequestPermissions(Activity, new[] { Manifest.Permission.Camera }, RequestCameraPermissionCode);
			}
		}

		public void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			if (requestCode == RequestCameraPermissionCode)
			{
				if (grantResults.Length != 1 || grantResults[0] != Permission.Granted)
				{
					ErrorDialog.NewInstance(/*Activity.GetString(Resource.String.request_permission)*/ "Permission")
							.Show(Activity.FragmentManager, FragmentDialog);
				}
			}
		}

		/// <summary>
		/// Sets up member variables related to camera
		/// </summary>
		/// <param name="width">The width of available size for camera preview</param>
		/// <param name="height">The height of available size for camera preview</param>
		private void SetUpCameraOutputs(int width, int height)
		{
			var activity = Activity;
			var manager = (CameraManager)activity.GetSystemService(Service.CameraService);
			try
			{
				foreach (var cameraId in manager.GetCameraIdList())
				{
					var characteristics = manager.GetCameraCharacteristics(cameraId);

					// We don't use a front facing camera in this sample.
					var facing = ((Integer)characteristics.Get(CameraCharacteristics.LensFacing))?.IntValue();

					if (facing != null && facing == (int)LensFacing.Front)
					{
						continue;
					}

					var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

					if (map == null)
					{
						continue;
					}

					// For still image captures, we use the largest available size.
					var outputSizes = Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg));


					var largest = (Android.Util.Size)outputSizes[outputSizes.Count / 2];//Collections.Min(outputSizes,new CompareSizesByArea());
					_mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, 2);
					//mImageReader.SetOnImageAvailableListener(this, BackgroundHandler);

					// Find out if we need to swap dimension to get the preview size relative to sensor
					// coordinate.
					var displayRotation = activity.WindowManager.DefaultDisplay.Rotation;
					//noinspection ConstantConditions
					_mSensorOrientation = ((Integer)characteristics.Get(CameraCharacteristics.SensorOrientation)).IntValue();
					var swappedDimensions = false;
					switch (displayRotation)
					{
						case SurfaceOrientation.Rotation0:
						case SurfaceOrientation.Rotation180:
							if (_mSensorOrientation == 90 || _mSensorOrientation == 270)
							{
								swappedDimensions = true;
							}
							break;
						case SurfaceOrientation.Rotation90:
						case SurfaceOrientation.Rotation270:
							if (_mSensorOrientation == 0 || _mSensorOrientation == 180)
							{
								swappedDimensions = true;
							}
							break;
					}

					Android.Graphics.Point displaySize = new Android.Graphics.Point();
					activity.WindowManager.DefaultDisplay.GetSize(displaySize);
					var rotatedPreviewWidth = width;
					var rotatedPreviewHeight = height;
					var maxPreviewWidth = displaySize.X;
					var maxPreviewHeight = displaySize.Y;

					if (swappedDimensions)
					{
						rotatedPreviewWidth = height;
						rotatedPreviewHeight = width;
						maxPreviewWidth = displaySize.Y;
						maxPreviewHeight = displaySize.X;
					}

					if (maxPreviewWidth > MaxPreviewWidth)
					{
						maxPreviewWidth = MaxPreviewWidth;
					}

					if (maxPreviewHeight > MaxPreviewHeight)
					{
						maxPreviewHeight = MaxPreviewHeight;
					}

					// Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
					// bus' bandwidth limitation, resulting in gorgeous previews but the storage of
					// garbage capture data.
					_mPreviewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))),
						rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
						maxPreviewHeight, largest);

					_mCameraId = cameraId;
					return;
				}
			}
			catch (CameraAccessException e)
			{
				e.PrintStackTrace();
			}
			catch (NullPointerException e)
			{
				// Currently an NPE is thrown when the Camera2API is used but not supported on the
				// device this code runs.
				ErrorDialog.NewInstance(/*Activity.GetString(Resource.String.camera_error)*/"Error")
							.Show(Activity.FragmentManager, FragmentDialog);
			}
		}

		/// <summary>
		/// Opens the camera specified by _mCameraId
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void OpenCamera(int width, int height)
		{
			if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.Camera)
					!= Permission.Granted)
			{
				RequestCameraPermission();
				return;
			}
			SetUpCameraOutputs(width, height);
			ConfigureTransform(width, height);
			var activity = Activity;
			var manager = (CameraManager)activity.GetSystemService(Service.CameraService);
			try
			{
				if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
				{
					throw new RuntimeException("Time out waiting to lock camera opening.");
				}
				manager.OpenCamera(_mCameraId, _mStateCallback, BackgroundHandler);
			}
			catch (CameraAccessException e)
			{
				e.PrintStackTrace();
			}
			catch (InterruptedException e)
			{
				throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
			}
		}

		/// <summary>
		/// Closes the current CameraDevice
		/// </summary>
		private void CloseCamera()
		{
			try
			{
				mCameraOpenCloseLock.Acquire();
				if (null != CaptureSession)
				{
					CaptureSession.Close();
					CaptureSession = null;
				}
				if (null != CameraDevice)
				{
					CameraDevice.Close();
					CameraDevice = null;
				}
				if (null != _mImageReader)
				{
					_mImageReader.Close();
					_mImageReader = null;
				}
			}
			catch (InterruptedException e)
			{
				throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
			}
			finally
			{
				mCameraOpenCloseLock.Release();
			}
		}

		/// <summary>
		/// Starts a background thread and its Handler
		/// </summary>
		private void StartBackgroundThread()
		{
			_mBackgroundThread = new HandlerThread("CameraBackground");
			_mBackgroundThread.Start();
			BackgroundHandler = new Handler(_mBackgroundThread.Looper);
		}

		/// <summary>
		/// Stops the background thread and its Handler
		/// </summary>
		private void StopBackgroundThread()
		{
			_mBackgroundThread.QuitSafely();
			try
			{
				_mBackgroundThread.Join();
				_mBackgroundThread = null;
				BackgroundHandler = null;
			}
			catch (InterruptedException e)
			{
				e.PrintStackTrace();
			}
		}

		/// <summary>
		/// Creates a new CameraCaptureSession for camera preview
		/// </summary>
		public void CreateCameraPreviewSession()
		{
			try
			{
				var texture = TextureView.SurfaceTexture;
				if (texture == null)
					throw new System.Exception($"{nameof(texture)} is null");

				// We configure the size of default buffer to be the size of camera preview we want.
				texture.SetDefaultBufferSize(_mPreviewSize.Width, _mPreviewSize.Height);

				// This is the output Surface we need to start preview.
				var surface = new Surface(texture);

				// We set up a CaptureRequest.Builder with the output Surface.
				PreviewRequestBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
				PreviewRequestBuilder.AddTarget(surface);

				// Here, we create a CameraCaptureSession for camera preview.
				var surfaces = new List<Surface> { surface, _mImageReader.Surface };
				CameraDevice.CreateCaptureSession(surfaces,
					new CameraCaptureSessionStateCallback(this), null);
			}
			catch (CameraAccessException e)
			{
				e.PrintStackTrace();
			}
		}

		/// <summary>
		/// Configures the necessary Matrix transformation to TextureView.
		/// This method should be called after the camera preview size is determined in
		/// SetUpCameraOutputs and also the size of TextureView is fixed.
		/// </summary>
		/// <param name="viewWidth"></param>
		/// <param name="viewHeight"></param>
		public void ConfigureTransform(int viewWidth, int viewHeight)
		{
			var activity = Activity;
			if (null == TextureView || null == _mPreviewSize || null == activity)
			{
				return;
			}
			var rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
			var matrix = new Matrix();
			var viewRect = new RectF(0, 0, viewWidth, viewHeight);
			var bufferRect = new RectF(0, 0, _mPreviewSize.Height, _mPreviewSize.Width);
			var centerX = viewRect.CenterX();
			var centerY = viewRect.CenterY();
			if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation)
			{
				bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
				matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
				var scale = System.Math.Max(
						(float)viewHeight / _mPreviewSize.Height,
						(float)viewWidth / _mPreviewSize.Width);
				matrix.PostScale(scale, scale, centerX, centerY);
				matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
			}
			else if ((int)SurfaceOrientation.Rotation180 == rotation)
			{
				matrix.PostRotate(180, centerX, centerY);
			}
			TextureView.SetTransform(matrix);
		}

		/// <summary>
		/// Capture a still picture. This method should be called when we get a response in
		/// CaptureCallback from both LockFocus()
		/// </summary>
		public void CaptureStillPicture()
		{
			try
			{
				var activity = Activity;
				if (null == activity || null == CameraDevice)
				{
					return;
				}
				// This is the CaptureRequest.Builder that we use to take a picture.
				var captureBuilder =
						CameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
				captureBuilder.AddTarget(_mImageReader.Surface);

				// Use the same AE and AF modes as the preview.
				captureBuilder.Set(CaptureRequest.ControlAfMode,
						(int)ControlAFMode.ContinuousPicture);

				// Orientation
				var rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
				captureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

				var captureCallback = new CameraCaptureSessionCaptureCallback2(this);

				CaptureSession.StopRepeating();
				CaptureSession.Capture(captureBuilder.Build(), captureCallback, null);
			}
			catch (CameraAccessException e)
			{
				e.PrintStackTrace();
			}
		}

		/// <summary>
		/// Retrieves the JPEG orientation from the specified screen rotation
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		private int GetOrientation(int rotation)
		{
			// Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
			// We have to take that into account and rotate JPEG properly.
			// For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
			// For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
			return (Orientations.Get(rotation) + _mSensorOrientation + 270) % 360;
		}

		/// <summary>
		/// Unlock the focus. This method should be called when still image capture sequence is
		/// finished
		/// </summary>
		public void UnlockFocus()
		{
			try
			{
				// Reset the auto-focus trigger
				PreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger,
					(int)ControlAFTrigger.Cancel);
				CaptureSession.Capture(PreviewRequestBuilder.Build(), CaptureCallback,
					BackgroundHandler);
				// After this, the camera will go back to the normal state of preview.
				mState = CameraState.Preview;
				CaptureSession.SetRepeatingRequest(mPreviewRequest, CaptureCallback,
						BackgroundHandler);
			}
			catch (CameraAccessException e)
			{
				e.PrintStackTrace();
			}
		}
		
		#region Camera2 sample code
		/**
		 * Conversion from screen rotation to JPEG orientation.
		 */
		static readonly SparseIntArray Orientations = new SparseIntArray();
		public const int RequestCameraPermissionCode = 1;
		const string FragmentDialog = "dialog";

		static CameraRenderer()
		{
			Orientations.Append((int)SurfaceOrientation.Rotation0, 90);
			Orientations.Append((int)SurfaceOrientation.Rotation90, 0);
			Orientations.Append((int)SurfaceOrientation.Rotation180, 270);
			Orientations.Append((int)SurfaceOrientation.Rotation270, 180);
		}

		/// <summary>
		/// Tag for the Log
		/// </summary>
		public new const string Tag = nameof(CameraRenderer);

		// Camera states moved to an enum

		/// <summary>
		/// Max preview width that is guaranteed by Camera2 API
		/// </summary>
		const int MaxPreviewWidth = 1920;

		/// <summary>
		/// Max preview height that is guaranteed by Camera2 API
		/// </summary>
		const int MaxPreviewHeight = 1080;

		private TextureViewSurfaceTextureListener _mSurfaceTextureListener;

		/// <summary>
		/// Id of the current <see cref="mCameraDevice"/>
		/// </summary>
		string _mCameraId;

		/// <summary>
		/// A TextureVuew for camera preview
		/// </summary>
		internal TextureView TextureView { get; set; }

		/// <summary>
		/// A CameraCaptureSession for camera preview
		/// </summary>
		internal CameraCaptureSession CaptureSession { get; set; }

		/// <summary>
		/// A reference to the opened CameraDevice
		/// </summary>
		internal CameraDevice CameraDevice { get; set; }

		/// <summary>
		/// The Size of camera preview
		/// </summary>
		private Android.Util.Size _mPreviewSize;

		/// <summary>
		/// The CameraDeviceStateCallback is called when CameraDevice changes its state
		/// </summary>
		private CameraDeviceStateCallback _mStateCallback;

		/// <summary>
		/// An additional thread for running tasks that shouldn't block the UI
		/// </summary>
		private HandlerThread _mBackgroundThread;

		/// <summary>
		/// A Handler for running tasks in the background
		/// </summary>
		internal Handler BackgroundHandler { get; set; }

		/// <summary>
		/// An ImageReader that handlers still image capture
		/// </summary>
		private ImageReader _mImageReader;

		/// <summary>
		/// CaptureRequest.Builder for the camera preview
		/// </summary>
		public CaptureRequest.Builder PreviewRequestBuilder;

		/// <summary>
		/// CameraRequest generated by the PreviewRequestBuilder
		/// </summary>
		public CaptureRequest mPreviewRequest;

		/// <summary>
		/// The current state of camera state for taking pictures
		/// </summary>
		public CameraState mState = CameraState.Preview;

		/// <summary>
		/// A Semaphore to prevent the app from exiting before closing the camera
		/// </summary>
		public Semaphore mCameraOpenCloseLock = new Semaphore(1);
		
		/// <summary>
		/// Orientation of the camera sensor
		/// </summary>
		private int _mSensorOrientation;

		/// <summary>
		/// A CameraCaptureSessionCaptureCallback that handles events related to JPEG capture
		/// </summary>
		public CameraCaptureSessionCaptureCallback CaptureCallback { get; private set; }

		/// <summary>
		/// Shows a Toast on the UI thread
		/// </summary>
		/// <param name="text">The message to show</param>
		public void ShowToast(string text)
		{
			var activity = Activity;
			if (activity != null)
			{
				activity.RunOnUiThread(() =>
				{
					Toast.MakeText(activity, text, ToastLength.Short).Show();
				});
			}
		}

		/// <summary>
		/// Given choices of Sizes supported by a camera, choose the smallest one that
		/// is at least as large as the respective texture view size, and that is at most as large as the
		/// respective max size, and whose aspect ratio matches with the specified value.If such size
		/// doesn't exist, choose the largest one that is at most as large as the respective max size,
		/// and whose aspect ratio matches with the specified value.
		/// </summary>
		/// <param name="choices">The list of sizes that the camera supports for the intended output class</param>
		/// <param name="textureViewWidth">The width of the texture view relative to sensor coordinate</param>
		/// <param name="textureViewHeight">The height of the texture view relative to sensor coordinate</param>
		/// <param name="maxWidth">The maximum width that can be chosen</param>
		/// <param name="maxHeight">The maximum height that can be chosen</param>
		/// <param name="aspectRatio">The aspect ratio</param>
		/// <returns>The optimal Size, or an arbitrary one if none were big enough</returns>
		private static Android.Util.Size ChooseOptimalSize(Android.Util.Size[] choices,
															int textureViewWidth,
															int textureViewHeight,
															int maxWidth,
															int maxHeight,
															Android.Util.Size aspectRatio)
		{
			// Collect the supported resolutions that are at least as big as the preview Surface
			var bigEnough = new List<Android.Util.Size>();
			// Collect the supported resolutions that are smaller than the preview Surface
			var notBigEnough = new List<Android.Util.Size>();
			var width = aspectRatio.Width;
			var height = aspectRatio.Height;
			foreach (Android.Util.Size option in choices)
			{
				if (option.Width <= maxWidth && option.Height <= maxHeight &&
						option.Height == option.Width * height / width)
				{
					if (option.Width >= textureViewWidth &&
						option.Height >= textureViewHeight)
					{
						bigEnough.Add(option);
					}
					else
					{
						notBigEnough.Add(option);
					}
				}
			}

			// Pick the smallest of those big enough. If there is no one big enough, pick the
			// largest of those not big enough.
			if (bigEnough.Any())
			{
				return (Android.Util.Size)Collections.Min(bigEnough, new CompareSizesByArea());
			}
			if (notBigEnough.Any())
			{
				return (Android.Util.Size)Collections.Max(notBigEnough, new CompareSizesByArea());
			}

			Log.Error(Tag, "Couldn't find any suitable preview size");
			return choices[0];
		}
		#endregion
	}
}