using System;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  public class ActaNovaTreeNodeControlObject : ActaNovaMainFrameControlObject, IActaNovaTreeNodeNavigator
  {
    private readonly BocTreeViewNodeControlObject _bocTreeViewNode;

    [UsedImplicitly]
    public ActaNovaTreeNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _bocTreeViewNode = new BocTreeViewNodeControlObject (context);
    }

    internal ActaNovaTreeNodeControlObject ([NotNull] BocTreeViewNodeControlObject bocTreeViewNode)
        : base (bocTreeViewNode.Context)
    {
      _bocTreeViewNode = bocTreeViewNode;
    }

    public string GetText ()
    {
      return _bocTreeViewNode.GetText();
    }

    public ActaNovaTreeNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var bocTreeViewNode = _bocTreeViewNode.GetNode (itemID);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject GetNode (int index)
    {
      var bocTreeViewNode = _bocTreeViewNode.GetNode (index);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    [Obsolete ("ActaNovaTree nodes cannot be selected using a full HTML ID.", true)]
    public ActaNovaTreeNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var bocTreeViewNode = _bocTreeViewNode.GetNodeByHtmlID (htmlID);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject GetNodeByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var bocTreeViewNode = _bocTreeViewNode.GetNodeByText (text);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject Expand ()
    {
      var bocTreeViewNode = _bocTreeViewNode.Expand();
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public ActaNovaTreeNodeControlObject Collapse ()
    {
      var bocTreeViewNode = _bocTreeViewNode.Collapse();
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    public UnspecifiedPageObject Select ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var actualActionBehavior = completionDetection ?? Continue.When (WaitForActaNova.OuterInnerOuterUpdate);
      _bocTreeViewNode.Select(actualActionBehavior);

      return UnspecifiedPage();
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      return _bocTreeViewNode.OpenContextMenu();
    }
  }
}