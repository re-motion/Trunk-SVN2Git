using System;
using System.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{

/// <summary>
///   Calls <see cref="ISupportsPostLoadControl.OnPostLoad"/> on all controls that support the interface.
/// </summary>
/// <remarks>
///   Children are called after their parents.
/// </remarks>
public class PostLoadInvoker
{
  public static void InvokePostLoad (Control control)
  {
    if (control is ISupportsPostLoadControl)
      ((ISupportsPostLoadControl)control).OnPostLoad ();

    ControlCollection controls = control.Controls;
    for (int i = 0; i < controls.Count; ++i)
    {
      Control childControl = controls[i];
      InvokePostLoad (childControl);
    }
  }

  private Control _control;
  private bool _invoked;

  public PostLoadInvoker (Control control)
  {
    _control = control;
    _invoked = false;
  }

  public void EnsurePostLoadInvoked ()
  {
    if (! _invoked)
    {
      InvokePostLoad (_control);
      _invoked = true;
    }
  }

}

}
