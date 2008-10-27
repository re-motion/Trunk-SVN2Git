/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using NUnit.Framework;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.Validation
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
