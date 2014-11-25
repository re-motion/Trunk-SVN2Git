using System;
using System.IO;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the down-level DMS control.
  /// </summary>
  public class ActaNovaDownLevelDmsControlObject : WebFormsControlObject
  {
    public ActaNovaDownLevelDmsControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Uploads the file given by <paramref name="filePath"/> to the server using the down-level DMS control.
    /// </summary>
    public UnspecifiedPageObject UploadFile (
        [NotNull] string filePath,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filePath", filePath);

      var fullFilePath = Path.GetFullPath (filePath);

      var scope = GetIframeScope();

      var fileScope = scope.FindCss ("input.HtmlUpload");
      // Note: do not use SendKeysFixed here, it does not work with file input tags in IE => we do not support "special characters" in file paths.
      fileScope.SendKeys (fullFilePath);

      var uploadButton = scope.FindCss ("input.UploadButton");
      uploadButton.ClickAndWait (Context, GetActualCompletionDetector (completionDetection), modalDialogHandler);
      return UnspecifiedPage();
    }

    private ElementScope GetIframeScope ()
    {
      return Scope.FindFrame ("").FindCss ("body.UploadPage");
    }
  }
}