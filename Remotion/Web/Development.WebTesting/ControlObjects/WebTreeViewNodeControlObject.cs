using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewNodeControlObject : RemotionControlObject, IControlObjectWithNodes<WebTreeViewNodeControlObject>
  {
    public WebTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      return Scope[DiagnosticMetadataAttributes.Text];
    }

    public IControlObjectWithNodes<WebTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.ItemID, itemID);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithIndex (int index)
    {
      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.IndexInCollection, index.ToString());
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    [Obsolete ("WebTreeView nodes cannot be selected using a full HTML ID.", true)]
    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithHtmlID (string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      throw new NotSupportedException ("WebTreeView nodes cannot be selected using the full HTML ID.");
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.Text, text);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    public WebTreeViewNodeControlObject Expand ()
    {
      var actualCompletionDetector = GetActualCompletionDetector (null);

      var expandAnchorScope = Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownExpandAnchor);
      expandAnchorScope.ClickAndWait (Context, actualCompletionDetector);
      return this;
    }

    public WebTreeViewNodeControlObject Collapse ()
    {
      var actualCompletionDetector = GetActualCompletionDetector (null);

      var collapseAnchorScope = Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownCollapseAnchor);
      collapseAnchorScope.ClickAndWait (Context, actualCompletionDetector);
      return this;
    }

    public WebTreeViewNodeControlObject Select ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);

      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      selectAnchorScope.ClickAndWait (Context, actualCompletionDetector);
      return this;
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      return new ContextMenuControlObject (Context.CloneForControl (selectAnchorScope));
    }

    private ElementScope GetWellKnownSelectAnchorScope ()
    {
      return Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownSelectAnchor);
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      return Continue.When (Wxe.PostBackCompleted);
    }
  }
}