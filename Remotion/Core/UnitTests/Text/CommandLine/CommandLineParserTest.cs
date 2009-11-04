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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Text.CommandLine;

namespace Remotion.UnitTests.Text.CommandLine
{

public enum IncrementalTestOptions { no, nor, normal, anything };
public enum TestOption { yes, no, almost };
public enum TestMode 
{
  [CommandLineMode ("m1", Description = "Primary mode")]
  Mode1, 
  [CommandLineMode ("m2", Description = "Secondary mode")]
  Mode2 
};


[TestFixture]
public class CommandLineParserTest
{
  private CommandLineParser CreateParser (
      out CommandLineStringArgument argSourceDir, 
      out CommandLineStringArgument argDestinationDir, 
      out CommandLineFlagArgument argCopyBinary,
      out CommandLineEnumArgument argEnumOption)
  {
    CommandLineParser parser = new CommandLineParser();

    argSourceDir = new CommandLineStringArgument (true);
    argSourceDir.Placeholder = "source-directory";
    argSourceDir.Description = "Directory to copy from";
    parser.Arguments.Add (argSourceDir);

    argDestinationDir = new CommandLineStringArgument (true);
    argDestinationDir.Placeholder = "destination-directory";
    argDestinationDir.Description = "This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to.";
    parser.Arguments.Add (argDestinationDir);

    argCopyBinary = new CommandLineFlagArgument ("b", true);
    argCopyBinary.Description = "binary copy on (+, default) or off (-)";
    parser.Arguments.Add (argCopyBinary);

    argEnumOption = new CommandLineEnumArgument ("rep", true, typeof (TestOption));
    argEnumOption.Description = "replace target";
    parser.Arguments.Add (argEnumOption);

    CommandLineModeArgument modeGroup = new CommandLineModeArgument (true, typeof (TestMode));
    foreach (CommandLineModeFlagArgument flag in modeGroup.Parts)
      parser.Arguments.Add (flag);
    parser.Arguments.Add (modeGroup);

    return parser;
  }

  private CommandLineParser CreateParser()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    return CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);
  }

  [Test]
  public void TestParsingSucceed ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Re:y" });

    Assert.AreEqual ("source", argSourceDir.Value);
    Assert.AreEqual ("dest", argDestinationDir.Value);
    Assert.AreEqual (false, argCopyBinary.Value);
    Assert.AreEqual (true, argEnumOption.HasValue);
    Assert.AreEqual (TestOption.yes, argEnumOption.Value);
  }

  [Test]
  public void TestParsingLeaveOutOptional ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source"} );

    Assert.AreEqual ("source", argSourceDir.Value);
    Assert.AreEqual (null, argDestinationDir.Value);
    Assert.AreEqual (true, argCopyBinary.Value);
    Assert.AreEqual (false, argEnumOption.HasValue);
  }

  [Test]
  [ExpectedException (typeof (MissingRequiredCommandLineParameterException))]
  public void TestParsingLeaveOutRequired ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);
    argEnumOption.IsOptional = false;

    parser.Parse (new string[] {
        "source"} );
  }

  [Test]
  [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
  public void TestParsingCaseSensitiveFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IsCaseSensitive = true;

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Re:y" });
  }

  [Test]
  [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
  public void TestParsingNotIncrementalFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IncrementalNameValidation = false;

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/b-",
        "/re:y" });
  }

  [Test]
  public void TestParsingNotIncrementalSucceed ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Rep:y" });

    Assert.AreEqual ("source", argSourceDir.Value);
    Assert.AreEqual ("dest", argDestinationDir.Value);
    Assert.AreEqual (false, argCopyBinary.Value);
    Assert.AreEqual (TestOption.yes, argEnumOption.Value);
  }

  [Test]
  [ExpectedException (typeof (InvalidNumberOfCommandLineArgumentsException))]
  public void TestParsingTooManyPositionalFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IncrementalNameValidation = false;

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "another"} );
  }

  [Test]
  public void TestSynopsis ()
  {
    CommandLineParser parser = CreateParser();
    string synopsis = parser.GetAsciiSynopsis ("app.exe", 80);
    
    string expectedResult = 
        "app.exe [source-directory [destination-directory]] [/b-] [/rep:{yes|no|almost}]" 
        + "\n[{/m1|/m2}]"
        + "\n"
        + "\n  source-directory       Directory to copy from" 
        + "\n  destination-directory  This is the directory to copy to. This is the directory" 
        + "\n                         to copy to. This is the directory to copy to. This is" 
        + "\n                         the directory to copy to. This is the directory to copy" 
        + "\n                         to. This is the directory to copy to. This is the" 
        + "\n                         directory to copy to. This is the directory to copy to." 
        + "\n  /b                     binary copy on (+, default) or off (-)" 
        + "\n  /rep                   replace target"
        + "\n  /m1                    Primary mode"
        + "\n  /m2                    Secondary mode";
    Assert.AreEqual (expectedResult, synopsis);
  }

  [Test]
  public void TestEnumValues ()
  {
    CommandLineEnumArgument enumArg;
    
    enumArg = new CommandLineEnumArgument (false, typeof (TestOption));
    PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "yes");
    Assert.AreEqual (TestOption.yes, (TestOption) enumArg.Value);

    enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));
    PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "no");
    Assert.AreEqual (IncrementalTestOptions.no, (IncrementalTestOptions) enumArg.Value);
  }

  [Test]
  public void TestInt32Values ()
  {
    CommandLineInt32Argument intArg;

    intArg = new CommandLineInt32Argument (true);
    PrivateInvoke.InvokeNonPublicMethod (intArg, "SetStringValue", "32");
    Assert.AreEqual (32, intArg.Value);

    intArg = new CommandLineInt32Argument (true);
    PrivateInvoke.InvokeNonPublicMethod (intArg, "SetStringValue", " ");
    Assert.AreEqual (null, intArg.Value);
  }

  [Test]
  [ExpectedException (typeof (InvalidCommandLineArgumentValueException))]
  public void TestEnumAmbiguous ()
  {
    try
    {
      CommandLineEnumArgument enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));
      PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "n");
    }
    catch (InvalidCommandLineArgumentValueException e)
    {
      Assert.IsTrue (e.Message.IndexOf ("Ambiguous") >= 0);
      throw e;
    }
  }

  [Test]
  [ExpectedException (typeof (InvalidCommandLineArgumentValueException))]
  public void TestEnumInvalid ()
  {
    try
    {
      CommandLineEnumArgument enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));
      PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "invalidvalue");
    }
    catch (InvalidCommandLineArgumentValueException e)
    {
      Assert.IsTrue (e.Message.IndexOf ("Use one of") >= 0);
      throw e;
    }
  }
}

}
