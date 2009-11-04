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
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class DisableableWriterTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (false);


    [Test]
    public void DisableableWriterCtorTest ()
    {
      var textWriterMock = MockRepository.GenerateStub<TextWriter> ();
      var disableableWriter = new DisableableWriter (textWriterMock);
      Assert.That (disableableWriter.Enabled, Is.True);
      Assert.That (disableableWriter.TextWriter, Is.EqualTo (textWriterMock));
    }

    [Test]
    public void WriteDelayedAsPrefixTest ()
    {
      var disableableWriter = new DisableableWriter ();
      Assert.That (disableableWriter.DelayedPrefix, Is.Null);
      disableableWriter.WriteDelayedAsPrefix (";");
      Assert.That (disableableWriter.DelayedPrefix, Is.EqualTo (";"));
      Assert.That (disableableWriter.DelayedPrefix, Is.EqualTo (";"));
      disableableWriter.ClearDelayedPrefix ();
      Assert.That (disableableWriter.DelayedPrefix, Is.Null);
    }


    [Test]
    public void WriteDelayedAsPrefixTest2 ()
    {
      var textWriterMock = MockRepository.GenerateMock<TextWriter> ();
      var disableableWriter = new DisableableWriter (textWriterMock);
      disableableWriter.Write ("abc");
      textWriterMock.AssertWasCalled (tw => tw.Write ((object) "abc"));
    }


    [Test]
    public void WriteDelayedAsPrefixTest3 ()
    {
      MockRepository mocks = new MockRepository ();

      var textWriterMock = mocks.DynamicMock<TextWriter> ();
      var disableableWriter = new DisableableWriter (textWriterMock);
      Expect.Call (() => textWriterMock.Write ((object) "abc"));

      mocks.ReplayAll ();

      disableableWriter.Write ("abc");

      mocks.VerifyAll ();
    }

    [Test]
    public void WriteDelayedAsPrefixResultTest ()
    {
      var stringWriter = new StringWriter();
      var disableableWriter = new DisableableWriter (stringWriter);
      disableableWriter.Write ("abc");
      Assert.That (disableableWriter.ToString(), Is.EqualTo ("abc"));
      disableableWriter.WriteDelayedAsPrefix (";");
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc"));
      disableableWriter.Write ("defg");
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc;defg"));
      disableableWriter.WriteDelayedAsPrefix (";");
      disableableWriter.ClearDelayedPrefix();
      disableableWriter.Write ("hijklm");
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc;defghijklm"));
      disableableWriter.WriteDelayedAsPrefix ("-SEP>");
      disableableWriter.Write ("nopqrst");
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc;defghijklm-SEP>nopqrst"));
    }

    [Test]
    public void WriteDelayedAsPrefixClearedPrefixResultTest ()
    {
      var stringWriter = new StringWriter ();
      var disableableWriter = new DisableableWriter (stringWriter);
      disableableWriter.Write ("abc");
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc"));
      disableableWriter.WriteDelayedAsPrefix (";");
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc"));
      disableableWriter.Write ("defg");
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc;defg"));
      disableableWriter.Write ("hijklm");
      // After having been written, the delayed prefix must be cleared
      Assert.That (disableableWriter.ToString (), Is.EqualTo ("abc;defghijklm"));
    }
  }
}
