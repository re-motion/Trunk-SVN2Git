using System;

namespace Remotion.Utilities
{
	/// <summary>
	/// Provides a default implementation of the <see cref="IConsoleManager"/> interface for the console represented by the <see cref="Console"/>
	/// class.
	/// </summary>
	public class DefaultConsoleManager : IConsoleManager
	{
		/// <summary>
		/// The single instance of this class.
		/// </summary>
		public static readonly DefaultConsoleManager Instance = new DefaultConsoleManager();

		private DefaultConsoleManager ()
		{
		}

		/// <summary>
		/// Gets or sets the foreground color of the console.
		/// </summary>
		/// <value>The console's foreground color. Returns <see cref="ConsoleColor.Gray"/> if there is no console attached to the current process.</value>
		public ConsoleColor ForegroundColor
		{
			get { return Console.ForegroundColor; }
			set { Console.ForegroundColor = value; }
		}

		/// <summary>
		/// Gets or sets the background color of the console.
		/// </summary>
		/// <value>The console's background color. Returns <see cref="ConsoleColor.Black"/> if there is no console attached to the current process.</value>
		public ConsoleColor BackgroundColor
		{
			get { return Console.BackgroundColor; }
			set { Console.BackgroundColor = value; }
		}
	}
}