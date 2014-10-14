using System;
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

    public void TakeDesktopScreenshot ([NotNull] string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      var fullFilePath = GetFullScreenshotFilePath (fileName);
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


    public void TakeBrowserScreenshot ([NotNull] string fileName, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);
      ArgumentUtility.CheckNotNull ("browser", browser);

      var fullFilePath = GetFullScreenshotFilePath (fileName);
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

    private string GetFullScreenshotFilePath (string fileName)
    {
      var filePath = Path.Combine (_screenshotDirectory, fileName + ".png");
      return Path.GetFullPath (filePath);
    }
  }
}