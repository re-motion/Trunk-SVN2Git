using System;

namespace Remotion.Web.UI.Controls.ListMenuImplementation
{
  /// <summary>
  /// Exposes <see cref="ListMenu"/> properties relevant to rendering.
  /// </summary>
  public interface IListMenu : IStyledControl
  {
    ListMenuLineBreaks LineBreaks { get; }
    WebMenuItemCollection MenuItems { get; }
    bool Enabled { get; }
    bool HasClientScript { get; }
    bool IsReadOnly { get; }
    string GetUpdateScriptReference (string getSelectionCount);
  }
}