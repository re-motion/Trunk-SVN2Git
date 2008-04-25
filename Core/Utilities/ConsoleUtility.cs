using System;

namespace Remotion.Utilities
{
	/// <summary>
	/// Provides utility methods for managing consoles.
	/// </summary>
	public static class ConsoleUtility
	{
		/// <summary>
		/// Represents a scope with a specific console color. Instantiate this scope via <see cref="ConsoleUtility.EnterColorScope"/> unless
		/// you need to specify a specific <see cref="IConsoleManager"/> implementation.
		/// </summary>
		public struct ColorScope : IDisposable
		{
			private readonly IConsoleManager _consoleManager;
			private ConsoleColor? _oldForegroundColor;
			private ConsoleColor? _oldBackgroundColor;

			public ColorScope (IConsoleManager consoleManager, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
			{
				ArgumentUtility.CheckNotNull ("consoleManager", consoleManager);

				_consoleManager = consoleManager;
				if (foregroundColor != null)
				{
					_oldForegroundColor = consoleManager.ForegroundColor;
					consoleManager.ForegroundColor = foregroundColor.Value;
				}
				else
					_oldForegroundColor = null;

				if (backgroundColor != null)
				{
					_oldBackgroundColor = consoleManager.BackgroundColor;
					consoleManager.BackgroundColor = backgroundColor.Value;
				}
				else
					_oldBackgroundColor = null;
			}

			public void Dispose ()
			{
				if (_oldForegroundColor != null)
				{
					_consoleManager.ForegroundColor = _oldForegroundColor.Value;
					_oldForegroundColor = null;
				}
				if (_oldBackgroundColor != null)
				{
					_consoleManager.BackgroundColor = _oldBackgroundColor.Value;
					_oldBackgroundColor = null;
				}
			}
		}

		/// <summary>
		/// Temporarily sets the foreground and background of the default console to the given <see cref="foregroundColor"/> and
		/// <see cref="backgroundColor"/>. The previous colors are restored when the returned object's <see cref="IDisposable.Dispose"/> method is 
		/// called, eg. in a <c>using</c> statement.
		/// </summary>
		/// <param name="foregroundColor">The color to temporarily assign as the console's foreground color. Pass <see langword="null"/> to leave the 
		/// foreground color untouched.</param>
		/// <param name="backgroundColor">The color to temporarily assign as the console's background color. Pass <see langword="null"/> to leave the 
		/// background color untouched.</param>
		/// <returns>A <see cref="IDisposable"/> object representing the scope for which the new colors stays valid.</returns>
		public static IDisposable EnterColorScope (ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
		{
			return new ColorScope (DefaultConsoleManager.Instance, foregroundColor, backgroundColor);
		}
	}
}