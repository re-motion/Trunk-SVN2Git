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
    public void CommandLineSwitcheShowUsageTest ()
    {
      var args = new [] {"/?"};

      var waitMock = MockRepository.GenerateMock<IWait> ();
      waitMock.Expect (mock => mock.Wait());

      waitMock.Replay();

      var stringWriterOut = new StringWriter();
      var stringWriterError = new StringWriter ();
      var consoleApplication =
        new ConsoleApplication<ConsoleApplicationTestApplicationRunner, ConsoleApplicationTestSettings> (stringWriterError, stringWriterOut, 80, waitMock);
      consoleApplication.Main (args);
      
      var outResult = stringWriterOut.ToString();
      var errorResult = stringWriterError.ToString ();
      To.ConsoleLine.e (() => outResult);
      To.ConsoleLine.e (() => errorResult);

      waitMock.VerifyAllExpectations();
      Assert.That (outResult,NUnitText.Contains ("Application Usage:"));
      Assert.That (outResult,NUnitText.Contains ("/? [/stringArg:string_arg_sample] [/flagArg] [{/?}]"));
      Assert.That (outResult,NUnitText.Contains ("/stringArg  stringArg description."));
      Assert.That (outResult,NUnitText.Contains ("/flagArg    flagArg description."));
      Assert.That (outResult,NUnitText.Contains ("/?          Show usage"));
      Assert.That (outResult, NUnitText.Contains ("Press any-key..."));

      Assert.That (errorResult, Is.EqualTo (""));

    }
  }


  internal class ConsoleApplicationTestApplicationRunner : IApplicationRunner<ConsoleApplicationTestSettings>
  {
    public void Run (ConsoleApplicationTestSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
    }
  }

  internal class ConsoleApplicationTestSettings : ConsoleApplicationSettings
  {
    [CommandLineStringArgument ("stringArg", true, Placeholder = "string_arg_sample", Description = "stringArg description.")]
    public string StringArg;

    [CommandLineFlagArgument ("flagArg", false, Description = "flagArg description.")]
    public bool UseMultipleFileOutput;
  }
}