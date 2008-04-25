using System;

namespace Remotion.Web.UI.Controls
{

/// <summary> Represents the method that handles an event raised by a <see cref="WebTreeNode"/>. </summary>
public delegate void WebTreeNodeEventHandler (object sender, WebTreeNodeEventArgs e);

/// <summary> Provides data for event raised by a <see cref="WebTreeNode"/>. </summary>
public class WebTreeNodeEventArgs: EventArgs
{
  private WebTreeNode _node;

  /// <summary> Initializes an instance. </summary>
  public WebTreeNodeEventArgs (WebTreeNode node)
  {
    _node = node;
  }

  /// <summary> The <see cref="WebTreeNode"/> that was clicked. </summary>
  public WebTreeNode Node
  {
    get { return _node; }
  }
}

/// <summary> Represents the method that handles the <c>Click</c> event raised when clicking on a tree node. </summary>
public delegate void WebTreeNodeClickEventHandler (object sender, WebTreeNodeClickEventArgs e);

/// <summary> Provides data for the <c>Click</c> event. </summary>
public class WebTreeNodeClickEventArgs: WebTreeNodeEventArgs
{
  private string[] _path;

  /// <summary> Initializes an instance. </summary>
  public WebTreeNodeClickEventArgs (WebTreeNode node, string[] path)
    : base (node)
  {
    _path = path;
  }

  /// <summary> The ID path for the clicked node. </summary>
  public string[] Path
  {
    get { return _path; }
  }
}

}
