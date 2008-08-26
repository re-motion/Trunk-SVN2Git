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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.Logging;
using Remotion.Diagnostic;

namespace Remotion.UnitTests.Diagnostic
{
  [TestFixture]
  public class IToTextHandlerTest
  {
    private ISimpleLogger log = SimpleLogger.Create (true);

    public class TestSimple : IToTextHandler
    {
      public TestSimple ()
      {
        Name = "Kal-El";
        Int = 2468;
      }

      public string Name { get; set; }
      public int Int { get; set; }

      public override string ToString ()
      {
        return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
      }

      public void ToText (ToTextBuilder toTextBuilder)
      {
        toTextBuilder.sb().m ("daInt", Int).m ("theName", Name).se();
      }
    }

    [Test]
    public void IToTextHandler ()
    {
      var toTextProvider = GetTextProvider();
      var testSimple = new TestSimple();
      string result = toTextProvider.ToTextString (testSimple);
      log.It (result);
      Assert.That (result, Is.EqualTo ("(daInt=2468,theName=\"Kal-El\")"));
    }

    [Test]
    public void RegisteredHandlerOverIToTextHandler ()
    {
      var toTextProvider = GetTextProvider();
      toTextProvider.RegisterHandler<TestSimple> ((x, ttb) => ttb.s ("TestSimple...").tt (x.Int).comma.tt (x.Name).s ("and out!"));
      var testSimple = new TestSimple();
      string result = toTextProvider.ToTextString (testSimple);
      log.It (result);
      Assert.That (result, Is.EqualTo ("TestSimple...2468,\"Kal-El\"and out!"));
    }

    public static ToTextProvider GetTextProvider ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.UseAutomaticObjectToText = false;
      toTextProvider.UseAutomaticStringEnclosing = true;
      toTextProvider.UseAutomaticCharEnclosing = true;
      return toTextProvider;
    }
  }
}