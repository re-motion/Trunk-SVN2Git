// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.Validation
{
  public class ValidationTestBase
  {
    public bool HasFailure (string ruleName, ValidationLogData log)
    {
      return log.GetResults().SelectMany (result => result.Failures).Any (item => item.RuleName == ruleName);
    }

    public bool HasWarning (string ruleName, ValidationLogData log)
    {
      return log.GetResults().SelectMany (result => result.Warnings).Any (item => item.RuleName == ruleName);
    }

    public void AssertSuccess (ValidationLogData log)
    {
      try
      {
        Assert.AreEqual (0, log.GetNumberOfFailures ());
        Assert.AreEqual (0, log.GetNumberOfWarnings ());
        Assert.AreEqual (0, log.GetNumberOfUnexpectedExceptions ());
      }
      catch (AssertionException)
      {
        ConsoleDumper.DumpValidationResults (log.GetResults ());
        throw;
      }
    }
  }
}
