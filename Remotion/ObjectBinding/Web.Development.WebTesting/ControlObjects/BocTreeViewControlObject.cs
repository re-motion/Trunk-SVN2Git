using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  public class BocTreeViewControlObject : BocControlObject, IBocTreeViewNodeNavigator
  {
    private readonly WebTreeViewControlObject _webTreeView;

    public BocTreeViewControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
      _webTreeView = new WebTreeViewControlObject (id, context);
    }

    public BocTreeViewNodeControlObject GetRootNode ()
    {
      var webTreeViewNode = _webTreeView.GetRootNode();
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNode (string itemID)
    {
      var webTreeViewNode = _webTreeView.GetNode (itemID);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNode (int index)
    {
      var webTreeViewNode = _webTreeView.GetNode (index);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      var webTreeViewNode = _webTreeView.GetNode (htmlID);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNodeByText (string text)
    {
      var webTreeViewNode = _webTreeView.GetNodeByText (text);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }
  }

  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  public class BocTreeViewNodeControlObject : BocControlObject, IBocTreeViewNodeNavigator
  {
    private readonly WebTreeViewNodeControlObject _webTreeViewNode;

    public BocTreeViewNodeControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _webTreeViewNode = new WebTreeViewNodeControlObject (id, context);
    }

    internal BocTreeViewNodeControlObject ([NotNull] WebTreeViewNodeControlObject webTreeViewNode)
        : base (webTreeViewNode.ID, webTreeViewNode.Context)
    {
      _webTreeViewNode = webTreeViewNode;
    }

    public BocTreeViewNodeControlObject GetNode (string itemID)
    {
      var webTreeViewNode = _webTreeViewNode.GetNode (itemID);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNode (int index)
    {
      var webTreeViewNode = _webTreeViewNode.GetNode (index);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      var webTreeViewNode = _webTreeViewNode.GetNodeByHtmlID (htmlID);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNodeByText (string text)
    {
      var webTreeViewNode = _webTreeViewNode.GetNodeByText (text);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject Expand ()
    {
      var webTreeViewNode = _webTreeViewNode.Expand();
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject Collapse ()
    {
      var webTreeViewNode = _webTreeViewNode.Collapse();
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject Select ()
    {
      var webTreeViewNode = _webTreeViewNode.Select();
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      return _webTreeViewNode.OpenContextMenu();
    }
  }

  /// <summary>
  /// Interface which must be supported by all node-like control objects.
  /// </summary>
  public interface IBocTreeViewNodeNavigator
  {
    BocTreeViewNodeControlObject GetNode (string itemID);
    BocTreeViewNodeControlObject GetNode (int index);
    BocTreeViewNodeControlObject GetNodeByHtmlID (string htmlID);
    BocTreeViewNodeControlObject GetNodeByText (string text);
  }
}