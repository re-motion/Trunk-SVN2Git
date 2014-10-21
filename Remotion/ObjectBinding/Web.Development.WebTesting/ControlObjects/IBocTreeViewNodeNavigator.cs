using System;
using JetBrains.Annotations;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface which must be supported by all node-like control objects.
  /// </summary>
  public interface IBocTreeViewNodeNavigator
  {
    BocTreeViewNodeControlObject GetNode ([NotNull] string itemID);
    BocTreeViewNodeControlObject GetNode (int index);
    BocTreeViewNodeControlObject GetNodeByHtmlID ([NotNull] string htmlID);
    BocTreeViewNodeControlObject GetNodeByText ([NotNull] string text);
  }
}