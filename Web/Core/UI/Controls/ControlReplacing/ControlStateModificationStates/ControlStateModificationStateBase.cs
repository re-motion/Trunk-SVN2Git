using System;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  public abstract class ControlStateModificationStateBase : IControlStateModificationState
  {
    private readonly ControlReplacer _replacer;
    private readonly IInternalControlMemberCaller _memberCaller;

    protected ControlStateModificationStateBase (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      _replacer = replacer;
      _memberCaller = memberCaller;
    }

    public abstract void LoadControlState (object savedState);

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