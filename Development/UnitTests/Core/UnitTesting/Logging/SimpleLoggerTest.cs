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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Logging
{
  [TestFixture]
  public class SimpleLoggerTest
  {
    [Test]
    public void LogTest1 ()
    {
      var textWriter = MockRepository.GenerateStub<TextWriter>();
      var toTextProvider = MockRepository.GenerateStub<ToTextProvider> ();

      SimpleLogger log = TypesafeActivator.CreateInstance<SimpleLogger> (BindingFlags.NonPublic | BindingFlags.Instance).With (textWriter, toTextProvider);

      log.It ("abc");
      textWriter.AssertWasCalled (tw => tw.WriteLine ("abc"));
    }

    [Test]
    public void LogTest2 ()
    {
      var textWriter = MockRepository.GenerateStub<TextWriter> ();
      var toTextProvider = MockRepository.GenerateStub<ToTextProvider> ();

      SimpleLogger log = TypesafeActivator.CreateInstance<SimpleLogger> (BindingFlags.NonPublic | BindingFlags.Instance).With (textWriter, toTextProvider);

      var obj = new object ();
      log.It (obj);
      textWriter.AssertWasCalled (tw => tw.WriteLine (toTextProvider.ToTextString (obj)));
    }

    [Test]
    public void LogTest3 ()
    {
      var textWriter = MockRepository.GenerateStub<TextWriter> ();
      var toTextProvider = MockRepository.GenerateStub<ToTextProvider> ();

      SimpleLogger log = TypesafeActivator.CreateInstance<SimpleLogger> (BindingFlags.NonPublic | BindingFlags.Instance).With (textWriter, toTextProvider);

      var obj = new object ();
      log.It (obj);
      toTextProvider.AssertWasCalled (ttp => ttp.ToTextString (obj));
    }

    [Test]
    public void LogTest1NonAaa ()
    {
      MockRepository mocks = new MockRepository ();
      
      var textWriter = mocks.DynamicMock<TextWriter> ();
      var toTextProvider = mocks.DynamicMock<ToTextProvider> ();
      //Expect.Call (delegate { textWriter.WriteLine ("abc"); });
      Expect.Call (() => textWriter.WriteLine ("abc"));
      
      mocks.ReplayAll ();

      SimpleLogger log = TypesafeActivator.CreateInstance<SimpleLogger> (BindingFlags.NonPublic | BindingFlags.Instance).With (textWriter, toTextProvider);
      log.It ("abc");
      
      mocks.VerifyAll ();
    }


  }
}