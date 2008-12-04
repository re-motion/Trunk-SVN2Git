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
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Text.StringExtensions;
using Remotion.Utilities.ConsoleApplication;
using Rhino.Mocks;
using NUnitText = NUnit.Framework.SyntaxHelpers.Text;

namespace Remotion.UnitTests.Utilities.ConsoleApplication
{
  [TestFixture]
  public class ConsoleApplicationTest
  {
    [Test]
    public void CommandLineSwitchShowUsageTest ()
    {
      var args = new[] { "/?" };

      var waitStub = MockRepository.GenerateMock<IWaiter> ();

      var stringWriterOut = new StringWriter ();
      var stringWriterError = new StringWriter ();
      var consoleApplication =
          new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (stringWriterError, stringWriterOut, 80, waitStub);
      consoleApplication.Main (args);

      var outResult = stringWriterOut.ToString ();
      var errorResult = stringWriterError.ToString ();

      Assert.That (outResult, NUnitText.Contains ("Application Usage:"));
      Assert.That (outResult, NUnitText.Contains ("[/stringArg:string_arg_sample] [/flagArg] [{/?}]"));
      Assert.That (outResult, NUnitText.Contains ("/stringArg  stringArg description."));
      Assert.That (outResult, NUnitText.Contains ("/flagArg    flagArg description."));
      Assert.That (outResult, NUnitText.Contains ("/?          Show usage"));
      Assert.That (outResult, NUnitText.Contains (Process.GetCurrentProcess ().MainModule.FileName.RightUntilChar ('\\')));
      
      Assert.That (errorResult, Is.EqualTo (""));

    }



    [Test]
    public void StringArgFlagArgTest ()
    {
      var args = new[] { "/stringArg:someText /flagArg+" };

      var stringWriterOut = new StringWriter ();
      var stringWriterError = new StringWriter ();

      var waitMock = MockRepository.GenerateMock<IWaiter> ();

      var consoleApplication =
          new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
              stringWriterError, stringWriterOut, 80, waitMock
              );


      consoleApplication.Main (args);

      var errorResult = stringWriterError.ToString ();

      Assert.That (errorResult, Is.EqualTo (""));
    }


    [Test]
    public void UnknownCommandLineSwitchTest ()
    {
      var args = new[] { "/UNKNOWN_ARGUMENT" };

      var stringWriterOut = new StringWriter ();
      var stringWriterError = new StringWriter ();

      var waitMock = MockRepository.GenerateMock<IWaiter> ();

      var consoleApplication =
          new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
              stringWriterError, stringWriterOut, 80, waitMock
              );


      consoleApplication.Main (args);

      var errorResult = stringWriterError.ToString ();
      Assert.That (errorResult, NUnitText.Contains (@"An error occured: Argument /UNKNOWN_ARGUMENT: invalid argument name"));
    }



    [Test]
    public void WaitForKeypressTest ()
    {
      var args = new[] { "/wait+" };

      var waitMock = MockRepository.GenerateMock<IWaiter> ();
      waitMock.Expect (mock => mock.Wait ());
      waitMock.Replay ();

      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
          TextWriter.Null, TextWriter.Null, 80, waitMock);
      
      consoleApplication.Main (args);

      Assert.That (consoleApplication.Settings.WaitForKeypress, Is.True);
      waitMock.VerifyAllExpectations ();
    }

    [Test]
    public void BufferWidthTest ()
    {
      const int bufferWidth = 37;

      var waitMock = MockRepository.GenerateStub<IWaiter> ();

      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
          TextWriter.Null, TextWriter.Null, bufferWidth, waitMock);

      Assert.That (consoleApplication.BufferWidth, Is.EqualTo(bufferWidth));
    }


    [Test]
    public void DefaultCtorTest ()
    {
      var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> ();
      Assert.That (PrivateInvoke.GetNonPublicField(consoleApplication,"_errorWriter"), Is.EqualTo (System.Console.Error));
      Assert.That (PrivateInvoke.GetNonPublicField(consoleApplication,"_logWriter"), Is.EqualTo (System.Console.Out));
      Assert.That (consoleApplication.BufferWidth, Is.EqualTo (80));
      Assert.That (PrivateInvoke.GetNonPublicField (consoleApplication, "_waitAtEnd"), Is.TypeOf (typeof (ConsoleKeypressWaiter)));
    }


    [Test]
    public void RunApplicationTextWriterArgumentPassingTest ()
    {
      var stringWriterOut = new StringWriter ();
      var stringWriterError = new StringWriter ();

      var waitStub = MockRepository.GenerateStub<IWaiter> ();

      var mocks = new MockRepository ();
      var applicationRunnerMock = mocks.DynamicMock<IApplicationRunner<ConsoleApplicationTestSettings>> ();
      var consoleApplicationMock =
          mocks.PartialMock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>> (
              stringWriterError, stringWriterOut, 80, waitStub);


      consoleApplicationMock.Expect (x => x.CreateApplication ()).Return (applicationRunnerMock);
      applicationRunnerMock.Expect (x => x.Run (Arg<ConsoleApplicationTestSettings>.Is.Anything,
                                                Arg<TextWriter>.Is.Equal (stringWriterError), Arg<TextWriter>.Is.Equal (stringWriterOut)));

      consoleApplicationMock.Replay ();
      applicationRunnerMock.Replay ();

      consoleApplicationMock.Main (new string[0]);

      applicationRunnerMock.VerifyAllExpectations ();
      consoleApplicationMock.VerifyAllExpectations ();
    }

    [Test]
    public void RunApplicationExeptionTest ()
    {
      var stringWriterError = new StringWriter ();
      var stringWriterOut = TextWriter.Null;

      var waitStub = MockRepository.GenerateStub<IWaiter> ();

      var mocks = new MockRepository ();
      var applicationRunnerMock = mocks.DynamicMock<IApplicationRunner<ConsoleApplicationTestSettings>> ();
      var consoleApplicationMock =
          mocks.PartialMock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>> (
              stringWriterError, stringWriterOut, 80, waitStub);

      consoleApplicationMock.Expect (x => x.CreateApplication ()).Return (applicationRunnerMock);
      applicationRunnerMock.Expect (x => x.Run (Arg<ConsoleApplicationTestSettings>.Is.Anything,
                                                Arg<TextWriter>.Is.Anything, Arg<TextWriter>.Is.Anything)).Throw(new Exception("The valve just came loose..."));

      consoleApplicationMock.Replay ();
      applicationRunnerMock.Replay ();

      consoleApplicationMock.Main (new string[0]);

      applicationRunnerMock.VerifyAllExpectations ();
      consoleApplicationMock.VerifyAllExpectations ();

      var result = stringWriterError.ToString();
      Assert.That (result, NUnitText.StartsWith ("Execution aborted. Exception stack:System.Exception: The valve just came loose..."));
    }
  }
}