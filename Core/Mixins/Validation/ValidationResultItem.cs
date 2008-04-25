using System;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  [Serializable]
  public struct ValidationResultItem : IDefaultValidationResultItem
  {
    private readonly IValidationRule _rule;

    public ValidationResultItem (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      _rule = rule;
    }

    public IValidationRule Rule
    {
      get { return _rule; }
    }

    public string Message
    {
      get { return Rule.Message; }
    }
  }
}