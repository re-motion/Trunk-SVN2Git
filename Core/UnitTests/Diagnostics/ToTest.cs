// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq.Expressions;
using log4net.Core;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Logging;
using System.IO;


namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTest
  {
    // TODO: Store & replace the ToTextProvider To.Text uses and restore it after the tests.

    public class ToTextTest {}


    [ToTextSpecificHandler]
    public class ToTextTestToTextSpecificTypeHandler : ToTextSpecificTypeHandler<ToTextTest>
    {
      public override void ToText (ToTextTest accessControlEntry, IToTextBuilder toTextBuilder)
      {
        toTextBuilder.s ("handled by ToTextTestToTextSpecificTypeHandler");
      }
    }

    public interface IToTextInterfaceTest {}
    public class ToTextInterfaceTest : IToTextInterfaceTest  { }
    [ToTextSpecificHandler]
    public class IToTextInterfaceTestToTextSpecificTypeHandler : ToTextSpecificInterfaceHandler<IToTextInterfaceTest>
    {
      public override void ToText (IToTextInterfaceTest t, IToTextBuilder toTextBuilder)
      {
        toTextBuilder.s ("handled by IToTextInterfaceTestToTextSpecificTypeHandler");
      }
    }


    [Test]
    public void GetTypeHandlersTest ()
    {
      var handlerMap = To.GetTypeHandlers ();
      Assert.That (handlerMap.Count, Is.GreaterThan(1));

      IToTextSpecificTypeHandler simpleHandler;
      handlerMap.TryGetValue (typeof (ToTextTest), out simpleHandler);
      Assert.That (simpleHandler, Is.Not.Null);
      Assert.That (simpleHandler is ToTextTestToTextSpecificTypeHandler, Is.True);

      //foreach (var pair in handlerMap)
      //{
      //  Log (pair.Key + " " + pair.Value);
      //}
    }

    [Test]
    public void ToTextTestToTextSpecificTypeHandlerTest ()
    {
      Assert.That (To.Text (new ToTextTest ()), Is.EqualTo ("handled by ToTextTestToTextSpecificTypeHandler"));
    }

    [Test]
    public void IToTextInterfaceTestToTextSpecificTypeHandlerTest ()
    {
      var toTextProviderSettings = To.ToTextProvider.Settings;
      bool useInterfaceHandlers = toTextProviderSettings.UseInterfaceHandlers;
      try
      {
        toTextProviderSettings.UseInterfaceHandlers = true;
        Assert.That (To.Text (new ToTextInterfaceTest ()), Is.EqualTo ("handled by IToTextInterfaceTestToTextSpecificTypeHandler"));
      }
      finally
      {
        toTextProviderSettings.UseInterfaceHandlers = useInterfaceHandlers;
      }
    }


    [Test]
    [Ignore]
    public void ToConsoleTest ()
    {
      To.Console.s ("ToConsoleTest");
    }

    [Test]
    [Ignore]
    public void ToErrorTest ()
    {
      To.Error.s ("ToErrorTest");
    }

    [Test]
    [Explicit]
    public void ToTempLogTest ()
    {
      var s = @"  line1
line2   
line3";
      To.TempLog.s ("ToTempLogTest").sEsc (s).s (s).e (() => s).Flush ();
      Log (To.LogFileDirectory);
    }




    //[Test]
    //public void ToStreamTest ()
    //{
    //  var stringWriter = new StringWriter();
    //  To.Stream (stringWriter).s ("ToStreamTest");
    //  var result = stringWriter.ToString();
    //  Assert.That (result, Is.EqualTo ("ToStreamTest"));
    //}


    [Test]
    public void ToStringTest ()
    {
      var stringWriter = new StringWriter ();
      var toStringToTextBuilder = To.String.s ("ToStringTest");
      var result = toStringToTextBuilder.CheckAndConvertToString();
      Assert.That (result, Is.EqualTo ("ToStringTest"));
    }

    //private static readonly ILog s_log = LogManager.GetLogger (typeof (ToTest));

    [Test]
    [Ignore]
    public void ToILogTest ()
    {
      //var log = LogManager.GetLogger (typeof (ToTest));
      //log.Log (LogLevel.Debug, To.String.s ("ToILogTest"));

      // TODO: Implement test

      //var iLoggerMock = MockRepository.GenerateMock<ILogger> ();
      //var logMock = MockRepository.GenerateMock<Log4NetLog> (iLoggerMock);
      //logMock.Log (LogLevel.Fatal, To.String.s ("ToILogTest"));
      //logMock.AssertWasCalled (log => log.Log (LogLevel.Fatal, "ToILogTest"));
    }

    [Test]
    [Explicit]
    public void ToILogTest2 ()
    {
      // TODO: Implement test
      var log = LogManager.GetLogger (typeof (ToTest));
      //log.Log (LogLevel.Debug, log.IsDebugEnabled ? "" : To.String.s ("ToILogTest").CheckAndConvertToString());
      log.Log (LogLevel.Debug, ttb => ttb.sb ().e ("ToILogTest2").se ());
    }
   

    [Test]
    [Explicit]
    public void ToTempLogXmlTest ()
    {
      var magicNumber = 123.456;
      var age = 98765;
      var street = "Dampfschiffstraße 14";
      To.TempLogXml.Open ();
      To.TempLogXml.s ("ToTempLogXmlTest").e (() => magicNumber).e (() => age).e (() => street).Flush ();
      To.TempLogXml.Close ();
      Log (To.LogFileDirectory);
    }

    [Test]
    [Explicit]
    public void ToTempLogXmlCloseTest ()
    {
      // Note: AppDomain.ProcessExit does not get called when executing a test =>
      // functionality has been tested in console app.
      var magicNumber = 123.456;
      var age = 98765;
      var street = "Dampfschiffstraße 14";
      To.TempLogXml.s ("ToTempLogXmlTest").e (() => magicNumber).e (() => age).e (() => street);
      Log (To.LogFileDirectory);
    }

    [Test]
    [Explicit]
    public void ThankYouTest ()
    {
      var Thank = "You";
      To.Console.e (() => Thank);
    }





    public static void Log (string s)
    {
      Console.WriteLine (s);
    }

    public static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }
  }
}
