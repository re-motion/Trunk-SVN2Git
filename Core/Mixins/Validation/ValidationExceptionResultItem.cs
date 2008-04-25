using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  [Serializable]
  public struct ValidationExceptionResultItem : IDefaultValidationResultItem
  {
    private IValidationRule _rule;
    private Exception _exception;

    public ValidationExceptionResultItem (IValidationRule rule, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      ArgumentUtility.CheckNotNull ("exception", exception);

      _rule = rule;
      _exception = exception;
    }

    public IValidationRule Rule
    {
      get { return _rule; }
    }

    public string Message
    {
      get { return _exception.ToString (); }
    }

    public Exception Exception
    {
      get { return _exception; }
    }
  }
}