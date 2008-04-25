using System;
using System.Reflection;

namespace Remotion.Web.ExecutionEngine
{

/// <summary>
/// If-Then-Else block.
/// </summary>
[Serializable]
public abstract class WxeIf: WxeStep
{
  WxeStepList _stepList = null; // represents Then or Else step list, depending on result of If()

  public override void Execute (WxeContext context)
  {
    Type type = this.GetType();
    if (_stepList == null)
    {
      MethodInfo ifMethod = type.GetMethod (
        "If", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[0], null);
      if (ifMethod == null || ifMethod.ReturnType != typeof (bool))
        throw new WxeException ("If-block " + type.FullName + " does not define method \"bool If()\".");

      bool result = (bool) ifMethod.Invoke (this, new object[0]);
      if (result)
      {
        _stepList = GetResultList ("Then");
        if (_stepList == null)
          throw new WxeException ("If-block " + type.FullName + " does not define nested class \"Then\".");
      }
      else
      {
        _stepList = GetResultList ("Else");
      }
    }

    if (_stepList != null)
    {
      _stepList.Execute (context);
    }
  }

  private WxeStepList GetResultList (string name)
  {
    Type type = this.GetType();
    Type stepListType = type.GetNestedType (name, BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
    if (stepListType == null)
      return null;
    if (! typeof (WxeStepList).IsAssignableFrom (stepListType))
      throw new WxeException ("Type " + stepListType.FullName + " must be derived from WxeStepList.");

    WxeStepList resultList = (WxeStepList) System.Activator.CreateInstance (stepListType);
    resultList.SetParentStep (this);
    return resultList;
  }

  public override WxeStep ExecutingStep
  {
    get
    {
      if (_stepList == null)
        return null;
      else 
        return _stepList.ExecutingStep;
    }
  }

  protected override void AbortRecursive()
  {
    base.AbortRecursive();
    if (_stepList != null)
      _stepList.Abort();
  }
}

}
