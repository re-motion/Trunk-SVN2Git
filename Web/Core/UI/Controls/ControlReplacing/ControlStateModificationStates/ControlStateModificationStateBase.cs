using System;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  public abstract class ControlStateModificationStateBase : IControlStateModificationState
  {
    private readonly ControlReplacer _replacer;

    protected ControlStateModificationStateBase (ControlReplacer replacer)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);

      _replacer = replacer;
    }

    public ControlReplacer Replacer
    {
      get { return _replacer; }
    }

    public abstract void LoadControlState (object savedState);
  }
}