using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewControlObject : RemotionControlObject, IWebTreeViewNodeNavigator
  {
    // Note: Functionality is integartion tested via BocTreeViewControlObject in BocTreeViewControlObjectTest.

    private readonly WebTreeViewNodeControlObject _metaRootNode;

    public WebTreeViewControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
      _metaRootNode = new WebTreeViewNodeControlObject (id, context);
    }

    public WebTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode (1);
    }

    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      return _metaRootNode.GetNode (itemID);
    }

    public WebTreeViewNodeControlObject GetNode (int index)
    {
      return _metaRootNode.GetNode (index);
    }

    public WebTreeViewNodeControlObject GetNodeByHtmlID (string htmlID)
    {
      return _metaRootNode.GetNode (htmlID);
    }

    public WebTreeViewNodeControlObject GetNodeByText (string text)
    {
      return _metaRootNode.GetNodeByText (text);
    }
  }
}