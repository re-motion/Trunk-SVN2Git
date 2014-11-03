using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:Remotion.Web.UI.Controls.ListMenu"/>.
  /// </summary>
  public class ListMenuControlObject : RemotionControlObject, IControlObjectWithSelectableItems
  {
    public ListMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public IControlObjectWithSelectableItems SelectItem ()
    {
      return this;
    }

    public UnspecifiedPageObject SelectItem (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectItem().WithItemID (itemID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithItemID (string itemID, ICompletionDetection completionDetection)
    {
      var item = Scope.FindDMA ("span.listMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (item);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithIndex (int index, ICompletionDetection completionDetection)
    {
      var item = Scope.FindChild ((index - 1).ToString());
      return ClickItem (item, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithHtmlID (string htmlID, ICompletionDetection completionDetection)
    {
      var item = Scope.FindId (htmlID);
      return ClickItem (item, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithText (string text, ICompletionDetection completionDetection)
    {
      var item = Scope.FindDMA ("span.listMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (item, completionDetection);
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, ICompletionDetection completionDetection = null)
    {
      var anchorScope = item.FindLink();

      var actualCompletionDetector = GetActualCompletionDetector (anchorScope, completionDetection);
      anchorScope.ClickAndWait (Context, actualCompletionDetector);
      return UnspecifiedPage();
    }
  }
}