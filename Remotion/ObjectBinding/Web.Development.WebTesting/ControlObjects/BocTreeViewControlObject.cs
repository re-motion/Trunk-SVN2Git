using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocTreeViewControlObject : BocControlObject, IControlObjectWithNodes<BocTreeViewNodeControlObject>
  {
    private readonly BocTreeViewNodeControlObject _metaRootNode;

    public BocTreeViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _metaRootNode = new BocTreeViewNodeControlObject (context);
    }

    public BocTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode().WithIndex (1);
    }

    public IControlObjectWithNodes<BocTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    public BocTreeViewNodeControlObject GetNode (string itemID)
    {
      return GetNode().WithItemID (itemID);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _metaRootNode.GetNode (itemID);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithIndex (int index)
    {
      return _metaRootNode.GetNode().WithIndex (index);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return _metaRootNode.GetNode().WithText (text);
    }
  }
}