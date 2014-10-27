using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocTreeViewControlObject : BocControlObject, IBocTreeViewNodeNavigator
  {
    private readonly BocTreeViewNodeControlObject _metaRootNode;

    public BocTreeViewControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
      _metaRootNode = new BocTreeViewNodeControlObject (id, context);
    }

    public BocTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode (1);
    }

    public BocTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _metaRootNode.GetNode (itemID);
    }

    public BocTreeViewNodeControlObject GetNode (int index)
    {
      return _metaRootNode.GetNode (index);
    }

    [Obsolete ("BocTreeView nodes cannot be selected using a full HTML ID.", true)]
    public BocTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      // Method declaration exists for symmetry reasons only.

      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      return _metaRootNode.GetNodeByHtmlID (htmlID);
    }

    public BocTreeViewNodeControlObject GetNodeByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return _metaRootNode.GetNodeByText (text);
    }
  }
}