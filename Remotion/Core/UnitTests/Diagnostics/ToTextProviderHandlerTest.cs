// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextProviderHandlerTest
  {
    private readonly ISimpleLogger log = SimpleLogger.CreateForConsole (false);

    [Test]
    [Ignore]
    public void ToTextProviderTypeHandlerNoPrimitivesAndTypesTest ()
    {
      var handler = new ToTextProviderAutomaticObjectToTextHandler ();

      AssertToTextHandledStatus (handler, 123, false);
      AssertToTextHandledStatus (handler, 123.456, false);

      var testObj = new object();

      AssertToTextHandledStatus (handler, testObj.GetType (), false);
      AssertToTextHandledStatus (handler, testObj, true);


    }



    public void AssertToTextHandledStatus (IToTextProviderHandler handler, Object obj, bool handled)
    {
      var parameters = ToTextProviderTest.CreateToTextParameters (obj); // TODO: Derive tests from common base class instead
      parameters.Settings.UseAutomaticObjectToText = true;
      var feedback = new ToTextProviderHandlerFeedback ();
      handler.ToTextIfTypeMatches (parameters, feedback);
      Assert.That (feedback.Handled, Is.EqualTo (handled));
    }


    public void Log (string s)
    {
      log.It (s);
    }

    public void LogVariables (params object[] parameterArray)
    {
      foreach (var obj in parameterArray)
      {
        log.Item (" " + obj);
      }
    }




  }




}
