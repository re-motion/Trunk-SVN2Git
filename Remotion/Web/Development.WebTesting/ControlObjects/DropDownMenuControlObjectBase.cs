using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Base class for all control objects representing a <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public abstract class DropDownMenuControlObjectBase : RemotionControlObject, IClickableItemsControlObject
  {
    protected DropDownMenuControlObjectBase ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    protected abstract void OpenDropDownMenu ();

    public UnspecifiedPageObject ClickItem (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindDMA ("li.DropDownMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (scope, completionDetection);
    }

    public UnspecifiedPageObject ClickItem (int index, ICompletionDetection completionDetection = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindXPath (string.Format ("li[{0}]", index));
      return ClickItem (scope, completionDetection);
    }

    public UnspecifiedPageObject ClickItemByHtmlID (string htmlID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindId (htmlID);
      return ClickItem (scope, completionDetection);
    }

    public UnspecifiedPageObject ClickItemByText (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindDMA ("li.DropDownMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (scope, completionDetection);
    }

    private ElementScope GetDropDownMenuScope ()
    {
      OpenDropDownMenu();

      var dropDownMenuOptionsScope = Context.RootScope.FindCss ("ul.DropDownMenuOptions");
      return dropDownMenuOptionsScope;
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetector = GetActualCompletionDetector (item, completionDetection);

      var anchorScope = item.FindLink();
      anchorScope.ClickAndWait (Context, actualCompletionDetector);
      return UnspecifiedPage();
    }
  }
}