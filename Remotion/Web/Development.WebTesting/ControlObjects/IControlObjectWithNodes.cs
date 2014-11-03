using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface which must be supported by all control objects holding <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/> nodes.
  /// </summary>
  public interface IControlObjectWithNodes<TNodeControlObject>
      where TNodeControlObject : ControlObject
  {
    IControlObjectWithNodes<TNodeControlObject> GetNode ();
    TNodeControlObject GetNode ([NotNull] string itemID);

    TNodeControlObject WithItemID ([NotNull] string itemID);
    TNodeControlObject WithIndex (int index);
    TNodeControlObject WithText ([NotNull] string text);
  }
}