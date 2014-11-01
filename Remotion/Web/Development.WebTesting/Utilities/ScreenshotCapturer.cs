﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  public class ScreenshotCapturer
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ScreenshotCapturer));
    private static Size s_screenSize;

    static ScreenshotCapturer ()
    {
      DetermineScreenSize();
    }

    private static void DetermineScreenSize ()
    {
      s_screenSize = new Size();

      foreach (var screen in Screen.AllScreens)
      {
        s_screenSize.Width += screen.Bounds.Width;
        if (screen.Bounds.Height > s_screenSize.Height)
          s_screenSize.Height = screen.Bounds.Height;
      }
    }

    private readonly string _screenshotDirectory;

    public ScreenshotCapturer ([NotNull] string screenshotDirectory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("screenshotDirectory", screenshotDirectory);

      _screenshotDirectory = screenshotDirectory;
      EnsureScreenshotDirectoryExists();
    }

    public void TakeDesktopScreenshot ([NotNull] string baseFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("baseFileName", baseFileName);

      var fullFilePath = ScreenshotCapturerFileNameGenerator.GetFullScreenshotFilePath (_screenshotDirectory, baseFileName, "Desktop", "png");
      s_log.InfoFormat ("Saving screenshot of desktop to '{0}'.", fullFilePath);

      try
      {
        // Todo RM-6297: Capture the mouse cursor as well.
        using (var bitmap = new Bitmap (s_screenSize.Width, s_screenSize.Height))
        {
          using (var gfx = Graphics.FromImage (bitmap))
          {
            gfx.CopyFromScreen (0, 0, 0, 0, s_screenSize);
            bitmap.Save (fullFilePath, ImageFormat.Png);
          }
        }
      }
      catch (Exception ex)
      {
        s_log.Error (string.Format ("Could not save screenshot to '{0}'.", fullFilePath), ex);
      }
    }


    public void TakeBrowserScreenshot ([NotNull] string baseFileName, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("baseFileName", baseFileName);
      ArgumentUtility.CheckNotNull ("browser", browser);

      var fullFilePath = ScreenshotCapturerFileNameGenerator.GetFullScreenshotFilePath (_screenshotDirectory, baseFileName, "Browser", "png");
      s_log.InfoFormat ("Saving screenshot of browser to '{0}'.", fullFilePath);

      try
      {
        browser.SaveScreenshot (fullFilePath, ImageFormat.Png);
      }
      catch (Exception ex)
      {
        s_log.Error (string.Format ("Could not save screenshot to '{0}'.", fullFilePath), ex);
      }
    }

    private void EnsureScreenshotDirectoryExists ()
    {
      Directory.CreateDirectory (_screenshotDirectory);
    }

    private static class ScreenshotCapturerFileNameGenerator
    {
      /// <summary>
      /// Combines the given <paramref name="screenshotDirectory"/> with the <paramref name="baseFileName"/>, the suffix <paramref name="suffix"/> and
      /// the file <paramref name="extension"/>. If the full file path would be longer than 260 characters, the <paramref name="baseFileName"/> is
      /// shortened accordingly.
      /// </summary>
      /// <exception cref="PathTooLongException">
      /// If the resulting file path would be longer than 260 characters (despite shortening of the <paramref name="baseFileName"/>).
      /// </exception>
      public static string GetFullScreenshotFilePath (string screenshotDirectory, string baseFileName, string suffix, string extension)
      {
        var sanitizedBaseFileName = SanitizeFileName (baseFileName);
        var filePath = GetFullScreenshotFilePathInternal (screenshotDirectory, sanitizedBaseFileName, suffix, extension);
        if (filePath.Length > 260)
        {
          var shortenedSanitizedBaseFileName = ShortenBaseFileName (filePath, sanitizedBaseFileName);
          filePath = GetFullScreenshotFilePathInternal (screenshotDirectory, shortenedSanitizedBaseFileName, suffix, extension);
        }

        return filePath;
      }

      /// <summary>
      /// Combines the given <paramref name="screenshotDirectory"/> with the <paramref name="baseFileName"/>, the suffix <paramref name="suffix"/> and
      /// the file <paramref name="extension"/>.
      /// </summary>
      private static string GetFullScreenshotFilePathInternal (string screenshotDirectory, string baseFileName, string suffix, string extension)
      {
        var fileName = string.Format ("{0}_{1}.{2}", baseFileName, suffix, extension);
        return Path.Combine (screenshotDirectory, fileName);
      }

      /// <summary>
      /// Removes all invalid file name characters from the given <paramref name="fileName"/>.
      /// </summary>
      private static string SanitizeFileName (string fileName)
      {
        foreach (var c in Path.GetInvalidFileNameChars())
          fileName = fileName.Replace (c, '_');

        return fileName;
      }

      /// <summary>
      /// Reduces the <paramref name="baseFileName"/> length such that the <paramref name="fullFilePath"/> is no longer than 260 characters. Throws an
      /// <see cref="PathTooLongException"/> if the <paramref name="baseFileName"/> would have to be reduced to zero characters.
      /// </summary>
      private static string ShortenBaseFileName (string fullFilePath, string baseFileName)
      {
        var overflow = fullFilePath.Length - 260;
        if (overflow > baseFileName.Length - 1)
        {
          throw new PathTooLongException (
              string.Format ("Could not save screenshot to '{0}', the file path is too long and cannot be reduced to 260 characters.", fullFilePath));
        }

        return baseFileName.Substring (0, baseFileName.Length - overflow);
      }
    }
  }
}