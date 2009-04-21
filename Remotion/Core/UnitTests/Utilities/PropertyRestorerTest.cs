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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities.PropertyRestorer;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class PropertyRestorerTest
  {
    [Test]
    public void RestoreOnDisposeTest ()
    {
      var testClass = new PropertyRestorerTestClass ("abc");
      Assert.That (testClass.Text,Is.EqualTo("abc"));
      //using (new PropertyRestorer<PropertyRestorerTestClass, string> (testClass, Properties<PropertyRestorerTestClass>.Get (x => x.Text)))
      using (new PropertyRestorer<PropertyRestorerTestClass, string> (testClass, x => x.Text))
      {
        testClass.Text = "defghij";
        Assert.That (testClass.Text, Is.EqualTo ("defghij"));
      }
      Assert.That (testClass.Text, Is.EqualTo ("abc"));
    }

    [Test]
    public void PropertyRestorerMotherTest ()
    {
      var testClass = new PropertyRestorerTestClass ("abc");
      Assert.That (testClass.Text, Is.EqualTo ("abc"));
      using (PropertyRestorerMother.New(testClass, x => x.Text))
      {
        testClass.Text = "defghij";
        Assert.That (testClass.Text, Is.EqualTo ("defghij"));
      }
      Assert.That (testClass.Text, Is.EqualTo ("abc"));
    }
  }

  public class PropertyRestorerTestClass
  {
    public string Text { get; set; }

    public PropertyRestorerTestClass (string text)
    {
      Text = text;
    }
  }
}