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
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.UnitTests.Mixins.SampleTypes;
using System.Linq;

namespace Remotion.UnitTests.Mixins.Context.Serialization
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
      var mixinContext = new MixinContext (MixinKind.Used, typeof (BT1Mixin1), MemberVisibility.Public, new[] {typeof (int), typeof (string)});
      _serializer.AddMixins (new[] {mixinContext});
      Assert.That (_deserializer.GetMixins().ToArray(), Is.EqualTo (new[] { mixinContext }));
    }

    [Test]
    public void AddCompleteInterfaces ()
    {
      _serializer.AddCompleteInterfaces (new[] {typeof (int), typeof (string)});
      Assert.That (_deserializer.GetCompleteInterfaces(), Is.EqualTo (new[] {typeof (int), typeof (string)}));
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
        ExpectedMessage = "Expected value of type 'System.Object[][]' at index 1 in the values array, but found 'System.Int32'.")]
    public void GetMixins_Invalid ()
    {
      _invalidDeserializer.GetMixins ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type[]' at index 2 in the values array, but found 'System.Int32'.")]
    public void GetCompleteInterfaces_Invalid ()
    {
      _invalidDeserializer.GetCompleteInterfaces();
    }
  }
}