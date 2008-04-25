using System;
namespace Remotion.Utilities
{
	/// <summary>
	/// Provides an API for manipulating the Console. This abstracts the <see cref="Console"/> class and thus enables unit testing of
	/// console-dependent code even when there is no console as well as use of multiple consoles at the same time.
	/// </summary>
	public interface IConsoleManager
	{
		/// <summary>
		/// Gets or sets the foreground color of the console.
		/// </summary>
		/// <value>The console's foreground color.</value>
		ConsoleColor ForegroundColor { get; set; }

		/// <summary>
		/// Gets or sets the background color of the console.
		/// </summary>
		/// <value>The console's background color.</value>
		ConsoleColor BackgroundColor { get; set; }
	}
}