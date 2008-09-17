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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;
using Remotion.UnitTests.Diagnostics.TestDomain;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class IToTextHandlerTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (true);



    [Test]
    public void IToTextHandler ()
    {
      var toTextProvider = GetTextProvider();
      var testSimple = new TestSimpleIToTextHandler();
      string result = toTextProvider.ToTextString (testSimple);
      log.It (result);
      Assert.That (result, Is.EqualTo ("(daInt=2468,theName=\"Kal-El\")"));
    }

    //[Test]
    //public void RegisteredHandlerOverIToTextHandler ()
    //{
    //  var toTextProvider = GetTextProvider();
    //  toTextProvider.RegisterSpecificTypeHandler<TestSimple> ((x, ttb) => ttb.s ("TestSimple...").e (x.Int).comma.e (x.Name).s ("and out!"));
    //  var testSimple = new TestSimple();
    //  string result = toTextProvider.ToTextString (testSimple);
    //  log.It (result);
    //  Assert.That (result, Is.EqualTo ("TestSimple...2468,\"Kal-El\"and out!"));
    //}

    public static ToTextProvider GetTextProvider ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.Settings.UseAutomaticObjectToText = false;
      toTextProvider.Settings.UseAutomaticStringEnclosing = true;
      toTextProvider.Settings.UseAutomaticCharEnclosing = true;
      return toTextProvider;
    }
  }
}