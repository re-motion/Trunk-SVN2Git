using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  [UsedImplicitly]
  public class WebTreeViewControlObject : RemotionControlObject, IControlObjectWithNodes<WebTreeViewNodeControlObject>
  {
    private readonly WebTreeViewNodeControlObject _metaRootNode;

    public WebTreeViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _metaRootNode = new WebTreeViewNodeControlObject (context);
    }

    public WebTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode().WithIndex (1);
    }

    public IControlObjectWithNodes<WebTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      return GetNode().WithItemID (itemID);
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _metaRootNode.GetNode (itemID);
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithIndex (int index)
    {
      return _metaRootNode.GetNode().WithIndex (index);
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithHtmlID (string htmlID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      return _metaRootNode.GetNode (htmlID);
    }

    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return _metaRootNode.GetNode().WithText (text);
    }
  }
}