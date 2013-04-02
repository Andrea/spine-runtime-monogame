#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Demo.Spine
{
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		private static AnimationDemo game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			game = new AnimationDemo();
			game.Run();
		}
	}
}
