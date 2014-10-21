using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  public class BocTreeViewNodeControlObject : BocControlObject, IBocTreeViewNodeNavigator
  {
    private readonly WebTreeViewNodeControlObject _webTreeViewNode;

    [UsedImplicitly]
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
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

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
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var webTreeViewNode = _webTreeViewNode.GetNodeByHtmlID (htmlID);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject GetNodeByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

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

    public BocTreeViewNodeControlObject Select ([CanBeNull] IActionBehavior actionBehavior = null)
    {
      var webTreeViewNode = _webTreeViewNode.Select(actionBehavior);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      return _webTreeViewNode.OpenContextMenu();
    }
  }
}