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
using Remotion.Text.CommandLine;

namespace Remotion.UnitTests.Text.CommandLine
{
  public class Arguments
  {
    [CommandLineStringArgument (true, Placeholder = "source-directory", Description = "Directory to copy from")]
    public string SourceDirectory;

    [CommandLineStringArgument (true, Placeholder = "destination-directory", Description = "This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to.")]
    public string DestinationDirectory;

    [CommandLineFlagArgument ("b", true, Description = "binary copy on (+, default) or off (-)")]
    public bool CopyBinary = true;

    [CommandLineEnumArgument ("rep", true)]
    public TestOption ReplaceTarget = TestOption.yes;

    [CommandLineModeArgument (true)]
    public TestMode Mode = TestMode.Mode1;
  }

  [TestFixture]
	public class CommandLineClassParserTest
	{
    [Test] 
    public void TestParser ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("sdir ddir /b- /rep:y", true);
      Assert.AreEqual ("sdir", arguments.SourceDirectory);
      Assert.AreEqual ("ddir", arguments.DestinationDirectory);
      Assert.AreEqual (false, arguments.CopyBinary);
      Assert.AreEqual (TestOption.yes, arguments.ReplaceTarget);
    }

    [Test] 
    public void TestModeArgDefault ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("", true);
      Assert.AreEqual (TestMode.Mode1, arguments.Mode);
    }

    [Test] 
    public void TestModeArgMode2 ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/m2", true);
      Assert.AreEqual (TestMode.Mode2, arguments.Mode);
    }

    [Test] 
    [ExpectedException (typeof (ConflictCommandLineParameterException))]
    public void TestModeArgConfict ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/m1 /m2", true);
    }

    [Test] 
    [ExpectedException (typeof (InvalidCommandLineArgumentValueException))]
    public void TestModeArgInvalidValue ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/m1+", true);
    }

    [Test]
    [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
    public void TestFlagArgInvalidValue ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/b~", true);
    }
    
    [Test] 
    public void TestOptional ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("", true);
      Assert.AreEqual (null, arguments.SourceDirectory);
      Assert.AreEqual (null, arguments.DestinationDirectory);
      Assert.AreEqual (true, arguments.CopyBinary);
      Assert.AreEqual (TestOption.yes, arguments.ReplaceTarget);
    }
  }
}
