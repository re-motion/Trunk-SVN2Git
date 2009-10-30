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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumDefinitionTest
  {
    private ExtensibleEnumDefinition<Color> _extensibleEnumDefinition;

    [SetUp]
    public void SetUp ()
    {
      _extensibleEnumDefinition = new ExtensibleEnumDefinition<Color> ();
    }

    [Test]
    public void GetValues ()
    {
      var valueInstances = _extensibleEnumDefinition.GetValues();

      Assert.That (valueInstances, Is.EquivalentTo (new[] { Color.Values.Red(), Color.Values.Green(), Color.Values.RedMetallic() }));
    }

    [Test]
    public void GetValues_CachesValues ()
    {
      var values1 = _extensibleEnumDefinition.GetValues ();
      var values2 = _extensibleEnumDefinition.GetValues ();

      Assert.That (values1.Count, Is.EqualTo (3));
      Assert.That (values1[0], Is.SameAs (values2[0]));
      Assert.That (values1[1], Is.SameAs (values2[1]));
      Assert.That (values1[2], Is.SameAs (values2[2]));
    }

    [Test]
    public void GetValues_PassesExtensibleEnumValuesInstance_ToExtensibleEnumValueDiscoveryService ()
    {
      ColorExtensions.LastCallArgument = null;
      _extensibleEnumDefinition.GetValues ();

      Assert.That (ColorExtensions.LastCallArgument, Is.SameAs (_extensibleEnumDefinition));
    }

    [Test]
    public void GetValueByID ()
    {
      var value = _extensibleEnumDefinition.GetValueByID ("Red");

      var expected = Color.Values.Red();
      Assert.That (value, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), 
        ExpectedMessage = "The extensible enum type 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' does not define a value called '?'.")]
    public void GetValueByID_WrongIDThrows ()
    {
      _extensibleEnumDefinition.GetValueByID ("?");
    }

    [Test]
    public void TryGetValueByID ()
    {
      Color result;
      var success = _extensibleEnumDefinition.TryGetValueByID ("Red", out result);

      var expected = Color.Values.Red ();
      Assert.That (success, Is.True);
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void TryGetValueByID_WrongIDThrows ()
    {
      Color result;
      var success = _extensibleEnumDefinition.TryGetValueByID ("?", out result);

      Assert.That (success, Is.False);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetValues_NonGeneric ()
    {
      ReadOnlyCollection<IExtensibleEnum> value = ((IExtensibleEnumDefinition) _extensibleEnumDefinition).GetValues ();
      Assert.That (value, Is.EqualTo (_extensibleEnumDefinition.GetValues ()));
    }

    [Test]
    public void GetValueByID_NonGeneric ()
    {
      IExtensibleEnum value = ((IExtensibleEnumDefinition) _extensibleEnumDefinition).GetValueByID ("Red");
      Assert.That (value, Is.SameAs (_extensibleEnumDefinition.GetValueByID ("Red")));
    }

    [Test]
    public void TryGetValueByID_NonGeneric ()
    {
      IExtensibleEnum value;
      bool success = ((IExtensibleEnumDefinition) _extensibleEnumDefinition).TryGetValueByID ("Red", out value);

      Color expectedValue;
      bool expectedSuccess = _extensibleEnumDefinition.TryGetValueByID ("Red", out expectedValue);

      Assert.That (success, Is.EqualTo (expectedSuccess));
      Assert.That (value, Is.SameAs (expectedValue));
    }
  }
}