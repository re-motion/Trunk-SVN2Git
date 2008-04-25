using System;

namespace Remotion.Mixins.Validation
{
  [AttributeUsage (AttributeTargets.Method)]
  public class DelegateRuleDescriptionAttribute : Attribute
  {
    private string _ruleName = null;
    private string _message = null;

    public DelegateRuleDescriptionAttribute ()
    {
    }


    public string RuleName
    {
      get { return _ruleName; }
      set { _ruleName = value; }
    }

    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }
  }
}