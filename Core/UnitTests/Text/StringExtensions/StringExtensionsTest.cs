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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Text.StringExtensions;

namespace Remotion.UnitTests.Text.StringExtensions
{
  [TestFixture]
  public class StringExtensionsTest
  {
    private const string _testString = "b A kkk CDC";

    [Test]
    public void LeftUntilCharTest ()
    {
      Assert.That ("".LeftUntilChar ('c'), Is.EqualTo (""));
      Assert.That ("S".LeftUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("Sabc".LeftUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("aSbc".LeftUntilChar ('S'), Is.EqualTo ("a"));
      Assert.That ("abSc".LeftUntilChar ('S'), Is.EqualTo ("ab"));
      Assert.That ("abcS".LeftUntilChar ('S'), Is.EqualTo ("abc"));
      Assert.That ("xyzzz".LeftUntilChar ('z'), Is.EqualTo ("xy"));
      Assert.That ("abcdefg".LeftUntilChar ('z'), Is.EqualTo ("abcdefg"));
    }

    [Test]
    public void LeftUntilCharUpperLowerCaseTest ()
    {
      Assert.That ("SaBcd".LeftUntilChar ('s'), Is.EqualTo ("SaBcd"));
      Assert.That ("sAbCD".LeftUntilChar ('S'), Is.EqualTo ("sAbCD"));
    }


    [Test]
    public void RightUntilCharTest ()
    {
      Assert.That ("".RightUntilChar ('c'), Is.EqualTo (""));
      Assert.That ("S".RightUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("abcS".RightUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("aSbc".RightUntilChar ('S'), Is.EqualTo ("bc"));
      Assert.That ("abSc".RightUntilChar ('S'), Is.EqualTo ("c"));
      Assert.That ("Sabc".RightUntilChar ('S'), Is.EqualTo ("abc"));
      Assert.That ("zzzxy".RightUntilChar ('z'), Is.EqualTo ("xy"));
      Assert.That ("abcdefg".RightUntilChar ('z'), Is.EqualTo ("abcdefg"));
    }

    [Test]
    public void RightUntilCharUpperLowerCaseTest ()
    {
      Assert.That ("SaBcd".RightUntilChar ('s'), Is.EqualTo ("SaBcd"));
      Assert.That ("sAbCD".RightUntilChar ('S'), Is.EqualTo ("sAbCD"));
    }
  }
}