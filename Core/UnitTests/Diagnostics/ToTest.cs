/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;
using Remotion.Diagnostics.ToText;

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
      public override void ToText (ToTextTest t, IToTextBuilderBase toTextBuilder)
      {
        toTextBuilder.s ("handled by ToTextTestToTextSpecificTypeHandler");
      }
    }

    public interface IToTextInterfaceTest {}
    public class ToTextInterfaceTest : IToTextInterfaceTest  { }
    [ToTextSpecificHandler]
    public class IToTextInterfaceTestToTextSpecificTypeHandler : ToTextSpecificInterfaceHandler<IToTextInterfaceTest>
    {
      public override void ToText (IToTextInterfaceTest t, IToTextBuilderBase toTextBuilder)
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
    public void ToConsoleTest ()
    {
      To.Console.s ("ToConsoleTest");
    }

    [Test]
    public void ToErrorTest ()
    {
      To.Error.s ("ToErrorTest");
    }

    [Test]
    [Ignore]
    public void ToTempLogTest ()
    {
      var s = @"  line1
line2   
line3";
      To.TempLog.s ("ToTempLogTest").sEsc (s).s (s).e (x => s).Flush ();
      Log (To.TempPath);
    }

    [Test]
    [Ignore]
    public void ToTempLogXmlTest ()
    {
      var magicNumber = 123.456;
      var age = 98765;
      var street = "Dampfschiffstraße 14";
      To.TempLogXml.Begin ();
      To.TempLogXml.s ("ToTempLogXmlTest").e (x => magicNumber).e (x => age).e (x => street).Flush ();
      To.TempLogXml.End ();
      Log (To.TempPath);
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