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

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class DisableableWriterTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (true);


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
    [Ignore]
    public void WriteDelayedAsPrefixTest2 ()
    {
      var textWriterMock = MockRepository.GenerateStub<TextWriter>();
      //var textWriterMock = MockRepository.GenerateStub<StringWriter> ();

      //var disableableWriter = new DisableableWriter (textWriterMock);
      var disableableWriter = TypesafeActivator.CreateInstance<DisableableWriter> (BindingFlags.Public | BindingFlags.Instance).With (textWriterMock);


      //Console.WriteLine ("AAAAAAAAAAAAAAAAAAAAAAAA: " + textWriterMock.GetHashCode());
      //Assert.That (disableableWriter.Enabled, Is.True);
      disableableWriter.Write ("abc");
      //textWriterMock.Write ("abc");
      textWriterMock.AssertWasCalled (tw => tw.Write ("abc"));
      //disableableWriter.WriteDelayedAsPrefix (";");
    }

    [Test]
    [Ignore]
    public void WriteDelayedAsPrefixTest2a ()
    {
      var textWriterMock = MockRepository.GenerateStub<TextWriter> ();
      var disableableWriter = new DisableableWriter (textWriterMock);
      disableableWriter.Write ("abc");
      textWriterMock.AssertWasCalled (tw => tw.Write ("abc"));
    }


    [Test]
    [Ignore]
    public void WriteDelayedAsPrefixTest3 ()
    {
      MockRepository mocks = new MockRepository ();

      var textWriterMock = mocks.DynamicMock<TextWriter> ();
      var disableableWriter = new DisableableWriter (textWriterMock);
      Expect.Call (() => textWriterMock.Write ("abc"));

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
  }
}