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
    [Ignore] // Changed SimpleLogger.It to not use ToTextProvider.ToTextString (inefficient, use TextBuilder-log-stream)
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
    [Ignore]  // Changed SimpleLogger.It to not use ToTextProvider.ToTextString (inefficient, use TextBuilder-log-stream)
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
      Expect.Call (() => textWriter.WriteLine ("abc"));
      
      mocks.ReplayAll ();

      SimpleLogger log = TypesafeActivator.CreateInstance<SimpleLogger> (BindingFlags.NonPublic | BindingFlags.Instance).With (textWriter, toTextProvider);
      log.It ("abc");
      
      mocks.VerifyAll ();
    }


  }
}
