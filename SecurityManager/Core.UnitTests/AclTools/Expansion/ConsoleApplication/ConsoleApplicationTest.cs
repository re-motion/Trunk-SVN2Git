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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;
using Remotion.Text.CommandLine;
using Rhino.Mocks;
using NUnitText = NUnit.Framework.SyntaxHelpers.Text;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.ConsoleApplication
{
  [TestFixture]
  public class ConsoleApplicationTest
  {
    [Test]
    public void CommandLineSwitchShowUsageTest ()
    {
      var args = new[] { "/?" };

      var waitMock = MockRepository.GenerateMock<IWait> ();
      waitMock.Expect (mock => mock.Wait ());

      waitMock.Replay ();

      var stringWriterOut = new StringWriter ();
      var stringWriterError = new StringWriter ();
      var consoleApplication =
        new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (stringWriterError, stringWriterOut, 80, waitMock);
      consoleApplication.Main (args);

      var outResult = stringWriterOut.ToString ();
      var errorResult = stringWriterError.ToString ();
      //To.ConsoleLine.e (() => outResult);
      //To.ConsoleLine.e (() => errorResult);

      waitMock.VerifyAllExpectations ();
      Assert.That (outResult, NUnitText.Contains ("Application Usage:"));
      Assert.That (outResult, NUnitText.Contains ("/? [/stringArg:string_arg_sample] [/flagArg] [{/?}]"));
      Assert.That (outResult, NUnitText.Contains ("/stringArg  stringArg description."));
      Assert.That (outResult, NUnitText.Contains ("/flagArg    flagArg description."));
      Assert.That (outResult, NUnitText.Contains ("/?          Show usage"));
      Assert.That (outResult, NUnitText.Contains ("Press any-key..."));

      Assert.That (errorResult, Is.EqualTo (""));

    }


    [Test]
    public void StringArgFlagArgTest ()
    {
      var args = new[] { "/stringArg:someText /flagArg+" };

      var stringWriterOut = new StringWriter ();
      var stringWriterError = new StringWriter ();
      //var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (stringWriterError, stringWriterOut, 80, consoleApplicationMock);

      var waitMock = MockRepository.GenerateMock<IWait> ();

      //ConsoleApplicationTestSettings consoleApplicationTestSettingsExpected = new ConsoleApplicationTestSettings();
      //consoleApplicationTestSettingsExpected.StringArg = "someText";
      //consoleApplicationTestSettingsExpected.FlagArg = true;

      //var consoleApplicationMock = 
      //  MockRepository.GenerateMock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>> (
      //    stringWriterError, stringWriterOut, 80, waitMock
      //  );

      //var consoleApplicationMock =
      //  new MockRepository ().PartialMock<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>> (
      //    stringWriterError, stringWriterOut, 80, waitMock
      //  );

      var consoleApplication =
        new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
          stringWriterError, stringWriterOut, 80, waitMock
        );


      //consoleApplicationMock.Expect (mock =>
      //  //PrivateInvoke.InvokeNonPublicMethod (mock, "RunApplication", Arg < ConsoleApplicationTestSettings >.Is.Equal(consoleApplicationTestSettingsExpected))); //.Return (0);
      //  //PrivateInvoke.InvokeNonPublicMethod (mock, "RunApplication", Arg<ConsoleApplicationTestSettings>.Is.Anything)).Return (0);
      //  //PrivateInvoke.InvokeNonPublicMethod (mock, "RunApplication", Arg<ConsoleApplicationTestSettings>.Matches(p => (p.StringArg == "someText" && p.FlagArg == true)))).Return (0);
      //  mock.RunApplication(Arg<ConsoleApplicationTestSettings>.Matches(p => (p.StringArg == "someText" && p.FlagArg == true)))).Return (0);

      //consoleApplicationMock.Replay ();

      consoleApplication.Main (args);

      var outResult = stringWriterOut.ToString ();
      var errorResult = stringWriterError.ToString ();
      To.ConsoleLine.e (() => outResult);
      To.ConsoleLine.e (() => errorResult);

      //consoleApplicationMock.VerifyAllExpectations ();
      Assert.That (errorResult, Is.EqualTo (""));
    }


    [Test]
    public void CommandLineSwitchErrorTest ()
    {
      var args = new[] { "/UNKNOWN_ARGUMENT" };

      var stringWriterOut = new StringWriter ();
      var stringWriterError = new StringWriter ();
      //var consoleApplication = new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (stringWriterError, stringWriterOut, 80, consoleApplicationMock);

      var waitMock = MockRepository.GenerateMock<IWait> ();

      //var consoleApplicationMock = 
      //  MockRepository.GenerateStub<ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings>> (
      //    stringWriterError, stringWriterOut, 80, waitMock
      //  );

      var consoleApplication =
        new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (
          stringWriterError, stringWriterOut, 80, waitMock
        );


      //consoleApplicationMock.Expect (mock =>
      //  PrivateInvoke.InvokeNonPublicMethod (mock, "ParseCommandLineArguments", args, 0)).Return (1);

      //int resultDummy = 0;
      //consoleApplicationMock.Expect (mock => mock.ParseCommandLineArguments (args, ref resultDummy)).Expect(null);

      //consoleApplicationMock.Expect (mock =>
      //  PrivateInvoke.InvokeNonPublicMethod (mock, "RunApplication", consoleApplicationTestSettingsExpected)).Return (0);

      //consoleApplication.Replay ();

      consoleApplication.Main (args);

      var errorResult = stringWriterError.ToString ();
      //To.ConsoleLine.e (() => outResult);
      To.ConsoleLine.e (() => errorResult);

      //waitMock.VerifyAllExpectations ();
      Assert.That (errorResult, NUnitText.StartsWith (@"Argument /UNKNOWN_ARGUMENT: invalid argument name"));
    }

  }




  public class ConsoleApplicationTestApplicationRunner : IApplicationRunner<ConsoleApplicationTestSettings>
  {
    public void Run (ConsoleApplicationTestSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
    }
  }

  public class ConsoleApplicationTestSettings : ConsoleApplicationSettings
  {
    [CommandLineStringArgument ("stringArg", true, Placeholder = "string_arg_sample", Description = "stringArg description.")]
    public string StringArg;

    [CommandLineFlagArgument ("flagArg", false, Description = "flagArg description.")]
    public bool FlagArg;
  }
}