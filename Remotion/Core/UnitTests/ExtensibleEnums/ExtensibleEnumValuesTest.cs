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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumValuesTest
  {
    private ExtensibleEnumValues<Color> _extensibleEnumValues;

    [SetUp]
    public void SetUp ()
    {
      _extensibleEnumValues = new ExtensibleEnumValues<Color> ();
    }

    [Test]
    public void GetValues ()
    {
      var valueInstances = _extensibleEnumValues.GetValues();

      Assert.That (valueInstances, Is.EquivalentTo (new[] { Color.Values.Red(), Color.Values.Green(), Color.Values.RedMetallic() }));
    }

    [Test]
    public void GetValues_PassesValuesInstanceToService ()
    {
      _extensibleEnumValues.GetValues();
      
      Assert.That (ColorExtensions.LastValues, Is.SameAs (_extensibleEnumValues));
    }

    [Test]
    public void GetValues_CachesValues ()
    {
      var values1 = _extensibleEnumValues.GetValues ();
      var values2 = _extensibleEnumValues.GetValues ();

      Assert.That (values1.Count, Is.EqualTo (3));
      Assert.That (values1[0], Is.SameAs (values2[0]));
      Assert.That (values1[1], Is.SameAs (values2[1]));
      Assert.That (values1[2], Is.SameAs (values2[2]));
    }

    [Test]
    public void GetValueByID ()
    {
      var value = _extensibleEnumValues.GetValueByID ("Red");

      var expected = Color.Values.Red();
      Assert.That (value, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), 
        ExpectedMessage = "The extensible enum type 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' does not define a value called '?'.")]
    public void GetValueByID_WrongIDThrows ()
    {
      _extensibleEnumValues.GetValueByID ("?");
    }

    [Test]
    public void TryGetValueByID ()
    {
      Color result;
      var success = _extensibleEnumValues.TryGetValueByID ("Red", out result);

      var expected = Color.Values.Red ();
      Assert.That (success, Is.True);
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void TryGetValueByID_WrongIDThrows ()
    {
      Color result;
      var success = _extensibleEnumValues.TryGetValueByID ("?", out result);

      Assert.That (success, Is.False);
      Assert.That (result, Is.Null);
    }
  }
}