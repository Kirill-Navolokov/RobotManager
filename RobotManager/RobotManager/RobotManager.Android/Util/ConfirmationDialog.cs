using Android.App;
using Android.OS;

namespace RobotManager.Droid.Util
{
	public class ConfirmationDialog : DialogFragment
	{
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			return new AlertDialog.Builder(Activity)
				.SetMessage("Permiso"/*Resource.String.request_permission*/)
				.SetPositiveButton(Android.Resource.String.Ok, new PositiveClickListener(this))
				.SetNegativeButton(Android.Resource.String.Cancel, new NegativeClickListener(this))
				.Create();
		}
	}
}