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
using System.Text;
using NUnit.Framework;
using Remotion.Text;

namespace Remotion.UnitTests.Text
{

[TestFixture]
public class MonospaceTextFormatTest
{
  [Test]
  public void TestSplitTextOnSeparator ()
  {
    AssertTextSplit ("12345 abcde",         10, "12345",        "abcde");
    AssertTextSplit ("1234567890 abcde",    10, "1234567890",   "abcde");
    AssertTextSplit ("12345678901 abcde",   10, "1234567890",   "1 abcde");
    AssertTextSplit ("1234 6789 bcde fghi", 10, "1234 6789",    "bcde fghi");
    AssertTextSplit ("1234567",             10, "1234567",      null);
    AssertTextSplit ("1234567890",          10, "1234567890",   null);
    AssertTextSplit ("12345678901",         10, "1234567890",   "1");
    AssertTextSplit ("",                     0, "",             null);
  }

  private void AssertTextSplit (string text, int splitAt, string expectedBefore, string expectedAfter)
  {
    string before;
    string after;
    MonospaceTextFormat.SplitTextOnSeparator (text, out before, out after, splitAt, new char[] {' '});
    Assert.AreEqual (expectedBefore, before);
    Assert.AreEqual (expectedAfter, after);
  }

  [Test]
  public void TestAppendIndentedText()
  {
    string label = "this is the label  ";
    string description = "the quick brown fox jumps over the lazy dog. THE (VERY QUICK) FOX JUMPS OVER THE LAZY DOG.";
    StringBuilder sb = new StringBuilder (label);
    MonospaceTextFormat.AppendIndentedText (sb, label.Length, 30, description);
    string expectedText = 
            "this is the label  the quick"
        + "\n                   brown fox"
        + "\n                   jumps over"
        + "\n                   the lazy"
        + "\n                   dog. THE"
        + "\n                   (VERY"
        + "\n                   QUICK) FOX"
        + "\n                   JUMPS OVER"
        + "\n                   THE LAZY"
        + "\n                   DOG.";
    Assert.AreEqual (expectedText, sb.ToString());
  }
}

}
