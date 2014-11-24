using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Represents the ActaNova popup window, asking the user for his/her signature.
  /// </summary>
  public class ActaNovaSignaturePopupWindowPageObject : ActaNovaPopupWindowPageObject
  {
    public ActaNovaSignaturePopupWindowPageObject ([NotNull] PageObjectContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Performs the signature.
    /// </summary>
    /// <param name="password">The password to use.</param>
    /// <param name="annotation">The annotation to use.</param>
    public ActaNovaMainPageObject Sign ([NotNull] string password, [CanBeNull] string annotation = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("password", password);

      return Sign ("OK", password, annotation);
    }

    /// <summary>
    /// Cancels the signature process.
    /// </summary>
    public void Cancel ()
    {
      Perform ("Cancel");
    }

    /// <summary>
    /// Adds the signature to the command batch.
    /// </summary>
    /// <param name="password">The password to use.</param>
    /// <param name="annotation">The annotation to use.</param>
    public ActaNovaMainPageObject BatchSign ([NotNull] string password, [CanBeNull] string annotation = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("password", password);

      return Sign ("Batch", password, annotation);
    }

    private ActaNovaMainPageObject Sign(string command, string password, string annotation)
    {
      var passwordTextValue = GetControl (new HtmlIDControlSelectionCommand<BocTextValueControlObject> (new BocTextValueSelector(), "PasswordField"));
      passwordTextValue.FillWith (password, FinishInput.Promptly);

      if(annotation != null)
      {
        var annotationTextValue =
            GetControl (new HtmlIDControlSelectionCommand<BocTextValueControlObject> (new BocTextValueSelector(), "SignatureAnnotationField"));
        annotationTextValue.FillWith (annotation, FinishInput.Promptly);
      }

      Perform (command);

      return new ActaNovaMainPageObject(Context.ParentContext.ParentContext);
    }
  }
}