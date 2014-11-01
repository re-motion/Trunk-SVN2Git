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
    public BocTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _webTreeViewNode = new WebTreeViewNodeControlObject (context);
    }

    internal BocTreeViewNodeControlObject ([NotNull] WebTreeViewNodeControlObject webTreeViewNode)
        : base (webTreeViewNode.Context)
    {
      _webTreeViewNode = webTreeViewNode;
    }

    public string GetText ()
    {
      return _webTreeViewNode.GetText();
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

    [Obsolete ("BocTreeView nodes cannot be selected using a full HTML ID.", true)]
    public BocTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

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

    public BocTreeViewNodeControlObject Select ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var webTreeViewNode = _webTreeViewNode.Select(completionDetection);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      return _webTreeViewNode.OpenContextMenu();
    }
  }
}