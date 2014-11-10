using System;
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
  public class DownLevelDmsControlObject : WebFormsControlObject
  {
    public DownLevelDmsControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject UploadFile ([NotNull] string filePath, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filePath", filePath);

      var scope = GetIFrameScope();

      var fileScope = scope.FindCss ("input.HtmlUpload");
      fileScope.SendKeysFixed (filePath);

      var uploadButton = scope.FindCss ("input.UploadButton");
      uploadButton.ClickAndWait (Context, GetActualCompletionDetector (completionDetection));
      return UnspecifiedPage();
    }

    private ElementScope GetIFrameScope ()
    {
      return Scope.FindFrame ("").FindCss ("body.UploadPage");
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      return Continue.When (Wxe.PostBackCompleted);
    }
  }
}