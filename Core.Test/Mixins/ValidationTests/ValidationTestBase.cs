using NUnit.Framework;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.ValidationTests
{
  public class ValidationTestBase
  {
    public bool HasFailure (string ruleName, IValidationLog log)
    {
      return GetFailureRule (ruleName, log) != null;
    }

    public IValidationRule GetFailureRule (string ruleName, IValidationLog log)
    {
      foreach (ValidationResult result in log.GetResults())
      {
        foreach (ValidationResultItem item in result.Failures)
        {
          if (item.Rule.RuleName == ruleName)
            return item.Rule;
        }
      }
      return null;
    }

    public bool HasWarning (string ruleName, IValidationLog log)
    {
      foreach (ValidationResult result in log.GetResults())
      {
        foreach (ValidationResultItem item in result.Warnings)
        {
          if (item.Rule.RuleName == ruleName)
            return true;
        }
      }
      return false;
    }

    public void AssertSuccess (IValidationLog log)
    {
      Assert.AreEqual (0, log.GetNumberOfFailures ());
      Assert.AreEqual (0, log.GetNumberOfWarnings ());
      Assert.AreEqual (0, log.GetNumberOfUnexpectedExceptions ());
    }
  }
}