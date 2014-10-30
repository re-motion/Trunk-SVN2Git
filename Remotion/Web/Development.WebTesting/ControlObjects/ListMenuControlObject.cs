using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:Remotion.Web.UI.Controls.ListMenu"/>.
  /// </summary>
  public class ListMenuControlObject : RemotionControlObject, IClickableItemsControlObject
  {
    public ListMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject ClickItem (string itemID, ICompletionDetection completionDetection = null)
    {
      var item = Scope.FindDMA ("span.listMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (item);
    }

    public UnspecifiedPageObject ClickItem (int index, ICompletionDetection completionDetection = null)
    {
      var item = FindChild ((index - 1).ToString());
      return ClickItem (item, completionDetection);
    }

    public UnspecifiedPageObject ClickItemByHtmlID (string htmlID, ICompletionDetection completionDetection = null)
    {
      var item = Scope.FindId (htmlID);
      return ClickItem (item, completionDetection);
    }

    public UnspecifiedPageObject ClickItemByText (string text, ICompletionDetection completionDetection = null)
    {
      var item = Scope.FindDMA ("span.listMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (item, completionDetection);
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, ICompletionDetection completionDetection = null)
    {
      var anchorScope = item.FindLink();

      var actualCompletionDetection = DetermineActualCompletionDetection (anchorScope, completionDetection);
      anchorScope.ClickAndWait (Context, actualCompletionDetection);
      return UnspecifiedPage();
    }
  }
}