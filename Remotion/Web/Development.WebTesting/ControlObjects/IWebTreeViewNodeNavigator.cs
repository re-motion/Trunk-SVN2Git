using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface which must be supported by all control objects holding <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/> nodes.
  /// </summary>
  public interface IWebTreeViewNodeNavigator
  {
    WebTreeViewNodeControlObject GetNode ([NotNull] string itemID);
    WebTreeViewNodeControlObject GetNode (int index);
    WebTreeViewNodeControlObject GetNodeByHtmlID ([NotNull] string htmlID);
    WebTreeViewNodeControlObject GetNodeByText ([NotNull] string text);
  }
}