using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using RobotManager.Droid.CustomRenderer;
using RobotManager.Droid.Views;
using RobotManager.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Button = Android.Widget.Button;
using Camera = Android.Hardware.Camera;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(ManagementPage), typeof(CameraRenderer))]
namespace RobotManager.Droid.CustomRenderer
{
	public class CameraRenderer : PageRenderer, TextureView.ISurfaceTextureListener
	{
		private Camera _camera;
		private Button _takePhotoButton;
		private Button _toggleFlashButton;
		private Button _switchCameraButton;
		private View _view;

		private Activity _activity;
		private CameraFacing _cameraType;
		private AutoFitTextureView _textureView;
		private SurfaceTexture _surfaceTexture;

		private bool _flashOn;

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

			try
			{
				SetupUserInterface();
				SetupEventHandlers();
				AddView(_view);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
			}
		}

		void SetupUserInterface()
		{
			_activity = this.Context as Activity;
			_view = _activity.LayoutInflater.Inflate(Resource.Layout.camera_layout, this, false);
			_cameraType = CameraFacing.Back;

			_textureView = _view.FindViewById<AutoFitTextureView>(Resource.Id.textureView);
			_textureView.SurfaceTextureListener = this;
		}

		void SetupEventHandlers()
		{
			_takePhotoButton = _view.FindViewById<global::Android.Widget.Button>(Resource.Id.takePhotoButton);
			_takePhotoButton.Click += TakePhotoButtonTapped;

			_switchCameraButton = _view.FindViewById<global::Android.Widget.Button>(Resource.Id.switchCameraButton);
			_switchCameraButton.Click += SwitchCameraButtonTapped;

			_toggleFlashButton = _view.FindViewById<global::Android.Widget.Button>(Resource.Id.toggleFlashButton);
			_toggleFlashButton.Click += ToggleFlashButtonTapped;
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
			var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

			_view.Measure(msw, msh);
			_view.Layout(0, 0, r - l, b - t);
		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{

		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
		{
			_camera = Camera.Open((int)_cameraType);
			_textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
			_surfaceTexture = surface;

			_camera.SetPreviewTexture(surface);
			PrepareAndStartCamera();
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			_camera.StopPreview();
			_camera.Release();
			return true;
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{
			PrepareAndStartCamera();
		}

		void PrepareAndStartCamera()
		{
			_camera.StopPreview();

			var display = _activity.WindowManager.DefaultDisplay;
			if (display.Rotation == SurfaceOrientation.Rotation0)
			{
				_camera.SetDisplayOrientation(90);
			}

			if (display.Rotation == SurfaceOrientation.Rotation270)
			{
				_camera.SetDisplayOrientation(180);
			}

			_camera.StartPreview();
		}

		void ToggleFlashButtonTapped(object sender, EventArgs e)
		{
			_flashOn = !_flashOn;
			if (_flashOn)
			{
				if (_cameraType == CameraFacing.Back)
				{
					_toggleFlashButton.SetBackgroundResource(Resource.Drawable.icon_flashButton);
					_cameraType = CameraFacing.Back;

					_camera.StopPreview();
					_camera.Release();
					_camera = Camera.Open((int)_cameraType);
					var parameters = _camera.GetParameters();
					parameters.FlashMode = Camera.Parameters.FlashModeTorch;
					_camera.SetParameters(parameters);
					_camera.SetPreviewTexture(_surfaceTexture);
					PrepareAndStartCamera();
				}
			}
			else
			{
				_toggleFlashButton.SetBackgroundResource(Resource.Drawable.icon_noFlashButton);
				_camera.StopPreview();
				_camera.Release();

				_camera = Camera.Open((int)_cameraType);
				var parameters = _camera.GetParameters();
				parameters.FlashMode = Camera.Parameters.FlashModeOff;
				_camera.SetParameters(parameters);
				_camera.SetPreviewTexture(_surfaceTexture);
				PrepareAndStartCamera();
			}
		}

		void SwitchCameraButtonTapped(object sender, EventArgs e)
		{
			if (_cameraType == CameraFacing.Front)
			{
				_cameraType = CameraFacing.Back;

				_camera.StopPreview();
				_camera.Release();
				_camera = Camera.Open((int)_cameraType);
				_camera.SetPreviewTexture(_surfaceTexture);
				PrepareAndStartCamera();
			}
			else
			{
				_cameraType = CameraFacing.Front;

				_camera.StopPreview();
				_camera.Release();
				_camera = Camera.Open((int)_cameraType);
				_camera.SetPreviewTexture(_surfaceTexture);
				PrepareAndStartCamera();
			}
		}

		async void TakePhotoButtonTapped(object sender, EventArgs e)
		{
			_camera.StopPreview();

			var image = _textureView.Bitmap;

			try
			{
				var absolutePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
				var folderPath = absolutePath + "/Camera";
				var filePath = System.IO.Path.Combine(folderPath, $"photo_{Guid.NewGuid()}.jpg");

				var fileStream = new FileStream(filePath, FileMode.Create);
				await image.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, fileStream);
				fileStream.Close();
				image.Recycle();

				var intent = new Intent(Intent.ActionMediaScannerScanFile);
				var file = new Java.IO.File(filePath);
				var uri = Android.Net.Uri.FromFile(file);
				intent.SetData(uri);
				MainActivity.Instance.SendBroadcast(intent);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"				", ex.Message);
			}

			_camera.StartPreview();
		}
	}
}