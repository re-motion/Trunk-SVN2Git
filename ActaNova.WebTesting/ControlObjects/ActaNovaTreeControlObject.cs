using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova tree.
  /// </summary>
  public class ActaNovaTreeControlObject : ActaNovaControlObject, IActaNovaTreeNodeNavigator
  {
    private readonly ActaNovaTreeNodeControlObject _metaRootNode;

    public ActaNovaTreeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _metaRootNode = new ActaNovaTreeNodeControlObject (context);
    }

    public ActaNovaTreeNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _metaRootNode.GetNode (itemID);
    }

    public ActaNovaTreeNodeControlObject GetNode (int index)
    {
      return _metaRootNode.GetNode (index);
    }

    [Obsolete ("ActaNovaTree nodes cannot be selected using a full HTML ID.", true)]
    public ActaNovaTreeNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      return _metaRootNode.GetNodeByHtmlID (htmlID);
    }

    public ActaNovaTreeNodeControlObject GetNodeByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return _metaRootNode.GetNodeByText (text);
    }
  }
}