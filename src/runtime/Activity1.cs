using Android.App;
using Android.Content.PM;
using Android.OS;

namespace spine_runtime_monogame.Android
{
	[Activity(Label = "spine-runtime-monogame.Android"
		, MainLauncher = true
		, Icon = "@drawable/icon"
		, Theme = "@style/Theme.Splash"
		, AlwaysRetainTaskState = true
		, LaunchMode = LaunchMode.SingleInstance
		, ScreenOrientation = ScreenOrientation.SensorLandscape
		, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
	public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
		//	Game1.Activity = this;
		//	var g = new Game1();
		//	SetContentView(g.Window);
		//	g.Run();
		}
	}
}

