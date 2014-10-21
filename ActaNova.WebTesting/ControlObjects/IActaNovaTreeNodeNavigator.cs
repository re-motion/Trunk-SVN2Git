using System;
using JetBrains.Annotations;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface which must be supported by all node-like control objects.
  /// </summary>
  public interface IActaNovaTreeNodeNavigator
  {
    ActaNovaTreeNodeControlObject GetNode ([NotNull] string itemID);
    ActaNovaTreeNodeControlObject GetNode (int index);
    ActaNovaTreeNodeControlObject GetNodeByHtmlID ([NotNull] string htmlID);
    ActaNovaTreeNodeControlObject GetNodeByText ([NotNull] string text);
  }
}