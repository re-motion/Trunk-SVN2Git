// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using System.Linq;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
{
  [TestFixture]
  public class AttributeClassContextSerializationTest
  {
    private AttributeClassContextSerializer _serializer;
    private AttributeClassContextDeserializer _deserializer;
    private AttributeClassContextDeserializer _invalidDeserializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new AttributeClassContextSerializer ();
      _deserializer = new AttributeClassContextDeserializer (_serializer.Values);
      _invalidDeserializer = new AttributeClassContextDeserializer (new object[] { 1, 2, 3 });
    }

    [Test]
    public void AddClassType()
    {
      _serializer.AddClassType (typeof (DateTime));
      Assert.That (_deserializer.GetClassType (), Is.EqualTo (typeof (DateTime)));
    }

    [Test]
    public void AddMixins ()
    {
      var mixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (string));
      var mixinContext2 = MixinContextObjectMother.Create (mixinType: typeof (object));
      _serializer.AddMixins (new[] {mixinContext1, mixinContext2});
      Assert.That (_deserializer.GetMixins().ToArray(), Is.EqualTo (new[] { mixinContext1, mixinContext2 }));
    }

    [Test]
    public void AddComposedInterfaces ()
    {
      _serializer.AddComposedInterfaces (new[] {typeof (int), typeof (string)});
      Assert.That (_deserializer.GetComposedInterfaces(), Is.EqualTo (new[] {typeof (int), typeof (string)}));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected an array with 3 elements.\r\nParameter name: values")]
    public void Deserializer_InvalidArray()
    {
      new AttributeClassContextDeserializer (new[] { "x" });
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type' at index 0 in the values array, but found 'System.Int32'.")]
    public void GetClassType_Invalid()
    {
      _invalidDeserializer.GetClassType ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Object[]' at index 1 in the values array, but found 'System.Int32'.")]
    public void GetMixins_Invalid ()
    {
      _invalidDeserializer.GetMixins ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type[]' at index 2 in the values array, but found 'System.Int32'.")]
    public void GetComposedInterfaces_Invalid ()
    {
      _invalidDeserializer.GetComposedInterfaces();
    }
  }
}
