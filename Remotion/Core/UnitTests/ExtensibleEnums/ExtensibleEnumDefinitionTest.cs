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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumDefinitionTest
  {
    private Color _red;
    private Color _green;
    private Color _blue;

    [SetUp]
    public void SetUp ()
    {
      _red = new Color ("Red");
      _green = new Color ("Green");
      _blue = new Color ("Blue");
    }

    [Test]
    public void GetValues ()
    {
      var definition = CreateDefinition (_red, _green, _blue);
      var valueInstances = definition.GetValues();
      
      Assert.That (valueInstances, Is.EquivalentTo (new[] { _red, _green, _blue }));
    }

    [Test]
    public void GetValues_CachesValues ()
    {
      var valueDiscoveryServiceMock = MockRepository.GenerateMock<IExtensibleEnumValueDiscoveryService> ();
      valueDiscoveryServiceMock
          .Expect (mock => mock.GetValues (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything))
          .Return (new[] { _red })
          .Repeat.Once ();
      valueDiscoveryServiceMock.Replay ();

      var extensibleEnumDefinition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceMock);
      var values1 = extensibleEnumDefinition.GetValues ();
      var values2 = extensibleEnumDefinition.GetValues ();

      valueDiscoveryServiceMock.VerifyAllExpectations ();
      Assert.That (values1, Is.SameAs (values2));
    }

    [Test]
    public void GetValues_PassesExtensibleEnumValuesInstance_ToExtensibleEnumValueDiscoveryService ()
    {
      var valueDiscoveryServiceMock = MockRepository.GenerateMock<IExtensibleEnumValueDiscoveryService> ();
      valueDiscoveryServiceMock.Stub (mock => mock.GetValues (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything)).Return (new[] { _red });

      var extensibleEnumDefinition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceMock);
      extensibleEnumDefinition.GetValues ();

      valueDiscoveryServiceMock.AssertWasCalled (mock => mock.GetValues (extensibleEnumDefinition));
    }

    [Test]
    public void GetValues_DefaultOrder_IsAlphabetic ()
    {
      var definition = CreateDefinition (_red, _blue, _green);

      var values = definition.GetValues ();

      Assert.That (values, Is.EqualTo (new[] { _blue, _green, _red }));
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage = 
        "Extensible enum 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' defines two values with ID 'Red'.")]
    public void GetValues_DuplicateIDs ()
    {
      var definition = CreateDefinition (_red, _red);
      definition.GetValues ();
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage =
        "Extensible enum 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' does not define any values.")]
    public void GetValues_NoValues ()
    {
      var definition = CreateDefinition ();
      definition.GetValues ();
    }

    [Test]
    public void GetValueByID ()
    {
      var definition = CreateDefinition (_red, _green);
      var value = definition.GetValueByID ("Red");

      Assert.That (value, Is.EqualTo (_red));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), 
        ExpectedMessage = "The extensible enum type 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' does not define a value called '?'.")]
    public void GetValueByID_WrongIDThrows ()
    {
      var definition = CreateDefinition (_red, _green);
      definition.GetValueByID ("?");
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage =
        "Extensible enum 'Remotion.UnitTests.ExtensibleEnums.TestDomain.Color' defines two values with ID 'Red'.")]
    public void GetValueByID_DuplicateIDs ()
    {
      var definition = CreateDefinition (_red, _red);
      definition.GetValueByID ("ID");
    }

    [Test]
    public void TryGetValueByID ()
    {
      var definition = CreateDefinition (_red, _green);

      Color result;
      var success = definition.TryGetValueByID ("Red", out result);

      var expected = Color.Values.Red ();
      Assert.That (success, Is.True);
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void TryGetValueByID_WrongID ()
    {
      var definition = CreateDefinition (_red, _green);

      Color result;
      var success = definition.TryGetValueByID ("?", out result);

      Assert.That (success, Is.False);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetValues_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      ReadOnlyCollection<IExtensibleEnum> value = ((IExtensibleEnumDefinition) definition).GetValues ();
      Assert.That (value, Is.EqualTo (definition.GetValues ()));
    }

    [Test]
    public void GetValueByID_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      IExtensibleEnum value = ((IExtensibleEnumDefinition) definition).GetValueByID ("Red");
      Assert.That (value, Is.SameAs (definition.GetValueByID ("Red")));
    }

    [Test]
    public void TryGetValueByID_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      IExtensibleEnum value;
      bool success = ((IExtensibleEnumDefinition) definition).TryGetValueByID ("Red", out value);

      Color expectedValue;
      bool expectedSuccess = definition.TryGetValueByID ("Red", out expectedValue);

      Assert.That (success, Is.EqualTo (expectedSuccess));
      Assert.That (value, Is.SameAs (expectedValue));
    }

    private ExtensibleEnumDefinition<Color> CreateDefinition (params Color[] colors)
    {
      var valueDiscoveryServiceStub = MockRepository.GenerateStub<IExtensibleEnumValueDiscoveryService> ();
      var definition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceStub);

      valueDiscoveryServiceStub.Stub (stub => stub.GetValues (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything)).Return (colors);
      return definition;
    }
  }
}
