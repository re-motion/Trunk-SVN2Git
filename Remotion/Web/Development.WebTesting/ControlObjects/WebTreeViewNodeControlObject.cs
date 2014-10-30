using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewNodeControlObject : RemotionControlObject, IWebTreeViewNodeNavigator
  {
    public WebTreeViewNodeControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      return Scope[DiagnosticMetadataAttributes.Text];
    }

    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.ItemID, itemID);
      return new WebTreeViewNodeControlObject (ID, Context.CloneForScope (nodeScope));
    }

    public WebTreeViewNodeControlObject GetNode (int index)
    {
      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.IndexInCollection, index.ToString());
      return new WebTreeViewNodeControlObject (ID, Context.CloneForScope (nodeScope));
    }

    [Obsolete ("WebTreeView nodes cannot be selected using a full HTML ID.", true)]
    public WebTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      throw new NotSupportedException ("WebTreeView nodes cannot be selected using the full HTML ID.");
    }

    public WebTreeViewNodeControlObject GetNodeByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.Text, text);
      return new WebTreeViewNodeControlObject (ID, Context.CloneForScope (nodeScope));
    }

    public WebTreeViewNodeControlObject Expand ()
    {
      var expandAnchorScope = Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownExpandAnchor);
      expandAnchorScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
      return this;
    }

    public WebTreeViewNodeControlObject Collapse ()
    {
      var collapseAnchorScope = Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownCollapseAnchor);
      collapseAnchorScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
      return this;
    }

    public WebTreeViewNodeControlObject Select ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetection = completionDetection ?? Behavior.WaitFor (WaitFor.WxePostBack);

      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      selectAnchorScope.ClickAndWait (Context, actualCompletionDetection);
      return this;
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      return new ContextMenuControlObject (ID, Context.CloneForScope (selectAnchorScope));
    }

    private ElementScope GetWellKnownSelectAnchorScope ()
    {
      return Scope.FindDMA (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownSelectAnchor);
    }
  }
}