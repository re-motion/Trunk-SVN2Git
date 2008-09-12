using System;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.StateModificationStates
{
  public abstract class ViewStateModificationStateBase : IViewStateModificationState
  {
    private readonly ControlReplacer _replacer;

    protected ViewStateModificationStateBase (ControlReplacer replacer)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);

      _replacer = replacer;
    }

    public ControlReplacer Replacer
    {
      get { return _replacer; }
    }

    public abstract void LoadViewState ();
  }
}