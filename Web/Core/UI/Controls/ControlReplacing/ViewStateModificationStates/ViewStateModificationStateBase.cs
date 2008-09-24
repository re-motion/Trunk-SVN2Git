using System;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  public abstract class ViewStateModificationStateBase : IViewStateModificationState
  {
    private readonly ControlReplacer _replacer;
    private readonly IInternalControlMemberCaller _memberCaller;

    protected ViewStateModificationStateBase (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      _replacer = replacer;
      _memberCaller = memberCaller;
    }

    public abstract void LoadViewState (object savedState);
    public abstract void AddedControlBegin ();
    public abstract void AddedControlCompleted ();

    public ControlReplacer Replacer
    {
      get { return _replacer; }
    }

    public IInternalControlMemberCaller MemberCaller
    {
      get { return _memberCaller; }
    }
  }
}