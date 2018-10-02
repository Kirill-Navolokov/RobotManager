using Android.Graphics;
using Android.Views;
using RobotManager.Droid.CustomRenderer;

namespace RobotManager.Droid.Listeneres
{
	public class TextureViewSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
	{
		public CameraRenderer Parent { get; private set; }

		public TextureViewSurfaceTextureListener(CameraRenderer parent)
		{
			Parent = parent;
		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
		{
			Parent.OpenCamera(width, height);
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			return true;
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{
			Parent.ConfigureTransform(width, height);
		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{
		}
	}
}