using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova tree.
  /// </summary>
  public class ActaNovaTreeControlObject : ActaNovaMainFrameControlObject, IControlObjectWithNodes<ActaNovaTreeNodeControlObject>
  {
    private readonly ActaNovaTreeNodeControlObject _metaRootNode;

    public ActaNovaTreeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _metaRootNode = new ActaNovaTreeNodeControlObject (context);
    }

    /// <inheritdoc/>
    public IControlObjectWithNodes<ActaNovaTreeNodeControlObject> GetNode ()
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
    ActaNovaTreeNodeControlObject IControlObjectWithNodes<ActaNovaTreeNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _metaRootNode.GetNode (itemID);
    }

    /// <inheritdoc/>
    ActaNovaTreeNodeControlObject IControlObjectWithNodes<ActaNovaTreeNodeControlObject>.WithIndex (int index)
    {
      return _metaRootNode.GetNode().WithIndex (index);
    }

    /// <inheritdoc/>
    ActaNovaTreeNodeControlObject IControlObjectWithNodes<ActaNovaTreeNodeControlObject>.WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      return _metaRootNode.GetNode().WithDisplayText (displayText);
    }
  }
}