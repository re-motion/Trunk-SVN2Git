using System;
using Coypu;
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
  public class ActaNovaTreeNodeControlObject
      : ActaNovaMainFrameControlObject,
          IControlObjectWithNodes<ActaNovaTreeNodeControlObject>,
          IFluentControlObjectWithNodes<ActaNovaTreeNodeControlObject>,
          IControlObjectWithText
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
      ArgumentUtility.CheckNotNull ("bocTreeViewNode", bocTreeViewNode);

      _bocTreeViewNode = bocTreeViewNode;
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return _bocTreeViewNode.GetText();
    }

    /// <summary>
    /// Returns whether the node is currently selected.
    /// </summary>
    public bool IsSelected ()
    {
      return _bocTreeViewNode.IsSelected();
    }

    /// <summary>
    /// Returns the number of child nodes.
    /// </summary>
    public int GetNumberOfChildren ()
    {
      return _bocTreeViewNode.GetNumberOfChildren();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<ActaNovaTreeNodeControlObject> GetNode ()
    {
      return this;
    }

    /// <inheritdoc/>
    public ActaNovaTreeNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    /// <inheritdoc/>
    ActaNovaTreeNodeControlObject IFluentControlObjectWithNodes<ActaNovaTreeNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var bocTreeViewNode = _bocTreeViewNode.GetNode (itemID);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    /// <inheritdoc/>
    ActaNovaTreeNodeControlObject IFluentControlObjectWithNodes<ActaNovaTreeNodeControlObject>.WithIndex (int index)
    {
      var bocTreeViewNode = _bocTreeViewNode.GetNode().WithIndex (index);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    /// <inheritdoc/>
    ActaNovaTreeNodeControlObject IFluentControlObjectWithNodes<ActaNovaTreeNodeControlObject>.WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var bocTreeViewNode = _bocTreeViewNode.GetNode().WithDisplayText (displayText);
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    /// <summary>
    /// Expands the node.
    /// </summary>
    public ActaNovaTreeNodeControlObject Expand ()
    {
      var bocTreeViewNode = _bocTreeViewNode.Expand();
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    /// <summary>
    /// Collapses the node.
    /// </summary>
    public ActaNovaTreeNodeControlObject Collapse ()
    {
      var bocTreeViewNode = _bocTreeViewNode.Collapse();
      return new ActaNovaTreeNodeControlObject (bocTreeViewNode);
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the following page.
    /// </summary>
    /// <remarks>
    /// This API is not symmetrical to <see cref="WebTreeViewNodeControlObject"/>'s and <see cref="BocTreeViewNodeControlObject"/>'s Select/Click
    /// methods - it behaves like Click.
    /// </remarks>
    public UnspecifiedPageObject Select ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);
      return _bocTreeViewNode.Click (actualActionOptions);
    }

    /// <summary>
    /// Opens the node's context menu.
    /// </summary>
    public ContextMenuControlObject OpenContextMenu ()
    {
      return _bocTreeViewNode.OpenContextMenu();
    }

    /// <inheritdoc/>
    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
    {
      if (IsSelected())
        return Wxe.PostBackCompleted;

      return base.GetDefaultCompletionDetectionStrategy (scope);
    }
  }
}