using System;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  /// <summary>
  /// The <see cref="ControlStateModificationStateBase"/> type is the base implementation of the <see cref="IControlStateModificationState"/> interface.
  /// </summary>
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

    public virtual void AddedControl (Control control, int index, Action<Control, int> baseCall)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("baseCall", baseCall);

      baseCall (control, index);
    }

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