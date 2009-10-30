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
using Remotion.ExtensibleEnums;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumValuesTest
  {
    [Test]
    public void GetValues ()
    {
      var extensibleEnumValues = new ExtensibleEnumValues<Color>();
      var valueInstances = extensibleEnumValues.GetValues();

      Assert.That (valueInstances, Is.EquivalentTo (new[] { Color.Values.Red(), Color.Values.Green(), Color.Values.RedMetallic() }));
    }

    [Test]
    public void GetValues_PassesValuesInstanceToService ()
    {
      var extensibleEnumValues = new ExtensibleEnumValues<Color> ();
      extensibleEnumValues.GetValues();
      
      Assert.That (ColorExtensions.LastValues, Is.SameAs (extensibleEnumValues));
    }

    [Test]
    public void GetValues_CachesValues ()
    {
      var extensibleEnumValues = new ExtensibleEnumValues<Color> ();
      var values1 = extensibleEnumValues.GetValues ();
      var values2 = extensibleEnumValues.GetValues ();

      Assert.That (values1.Count, Is.EqualTo (3));
      Assert.That (values1[0], Is.SameAs (values2[0]));
      Assert.That (values1[1], Is.SameAs (values2[1]));
      Assert.That (values1[2], Is.SameAs (values2[2]));
    }
  }
}