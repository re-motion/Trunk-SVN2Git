using System;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Allows controls to receive a call after the <see cref="Control.Load"/> event.
/// </summary>
public interface ISupportsPostLoadControl: IControl
{
  /// <summary>
  ///   This method may be called after the <see cref="Control.Load"/> event.
  /// </summary>
  void OnPostLoad();
}

}
