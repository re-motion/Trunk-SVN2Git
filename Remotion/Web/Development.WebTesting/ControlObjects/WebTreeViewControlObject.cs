using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewControlObject : RemotionControlObject, IWebTreeViewNodeNavigator
  {
    // Note: Functionality is integartion tested via BocTreeViewControlObject in BocTreeViewControlObjectTest.

    private readonly WebTreeViewNodeControlObject _metaRootNode;

    public WebTreeViewControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
      _metaRootNode = new WebTreeViewNodeControlObject (id, context);
    }

    public WebTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode (1);
    }

    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      return _metaRootNode.GetNode (itemID);
    }

    public WebTreeViewNodeControlObject GetNode (int index)
    {
      return _metaRootNode.GetNode (index);
    }

    public WebTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      return _metaRootNode.GetNode (htmlID);
    }

    public WebTreeViewNodeControlObject GetNodeByText (string text)
    {
      return _metaRootNode.GetNodeByText (text);
    }
  }

  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewNodeControlObject : RemotionControlObject, IWebTreeViewNodeNavigator
  {
    public WebTreeViewNodeControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.ItemID, itemID);
      nodeScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?
      return new WebTreeViewNodeControlObject (ID, Context.CloneForScope (nodeScope));
    }

    public WebTreeViewNodeControlObject GetNode (int index)
    {
      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.IndexInCollection, index.ToString());
      nodeScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?
      return new WebTreeViewNodeControlObject (ID, Context.CloneForScope (nodeScope));
    }

    public WebTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      throw new NotSupportedException ("BocTreeView nodes cannot be selected using the full HTML ID.");
    }

    public WebTreeViewNodeControlObject GetNodeByText (string text)
    {
      var nodeScope = Scope.FindDMA ("ul li", DiagnosticMetadataAttributes.Text, text);
      nodeScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?
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

    public WebTreeViewNodeControlObject Select ()
    {
      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      selectAnchorScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
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

  /// <summary>
  /// Interface which must be supported by all control objects holding <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/> nodes.
  /// </summary>
  public interface IWebTreeViewNodeNavigator
  {
    WebTreeViewNodeControlObject GetNode (string itemID);
    WebTreeViewNodeControlObject GetNode (int index);
    WebTreeViewNodeControlObject GetNodeByHtmlID (string htmlID);
    WebTreeViewNodeControlObject GetNodeByText (string text);
  }
}