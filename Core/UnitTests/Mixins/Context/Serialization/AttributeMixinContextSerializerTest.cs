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
using Remotion.Mixins.Context.Serialization;

namespace Remotion.UnitTests.Mixins.Context.Serialization
{
  [TestFixture]
  public class AttributeMixinContextSerializationTest
  {
    private AttributeMixinContextSerializer _serializer;
    private AttributeMixinContextDeserializer _deserializer;
    private AttributeMixinContextDeserializer _invalidDeserializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new AttributeMixinContextSerializer ();
      _deserializer = new AttributeMixinContextDeserializer (_serializer.Values);
      _invalidDeserializer = new AttributeMixinContextDeserializer (new object[] {1, 2, 3, 4});
    }

    [Test]
    public void AddMixinType()
    {
      _serializer.AddMixinType (typeof (DateTime));
      Assert.That (_deserializer.GetMixinType (), Is.EqualTo (typeof (DateTime)));
    }

    [Test]
    public void AddMixinKind()
    {
      _serializer.AddMixinKind (MixinKind.Used);
      Assert.That (_deserializer.GetMixinKind (), Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void AddIntroducedMemberVisibility ()
    {
      _serializer.AddIntroducedMemberVisibility (MemberVisibility.Public);
      Assert.That (_deserializer.GetIntroducedMemberVisibility(), Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void AddExplicitDependencies ()
    {
      _serializer.AddExplicitDependencies (new[] { typeof (int), typeof (string) });
      Assert.That (_deserializer.GetExplicitDependencies (), Is.EqualTo (new[] { typeof (int), typeof (string) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected an array with 4 elements.\r\nParameter name: values")]
    public void Deserializer_InvalidArray()
    {
      new AttributeMixinContextDeserializer (new[] { "x" });
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type' at index 0 in the values array, but found 'System.Int32'.")]
    public void GetMixinType_Invalid()
    {
      _invalidDeserializer.GetMixinType ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'Remotion.Mixins.MixinKind' at index 1 in the values array, but found 'System.Int32'.")]
    public void GetMixinKind_Invalid ()
    {
      _invalidDeserializer.GetMixinKind ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'Remotion.Mixins.MemberVisibility' at index 2 in the values array, but found 'System.Int32'.")]
    public void GetIntroducedMemberVisibility_Invalid ()
    {
      _invalidDeserializer.GetIntroducedMemberVisibility();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type[]' at index 3 in the values array, but found 'System.Int32'.")]
    public void GetExplicitDependencies_Invalid ()
    {
      _invalidDeserializer.GetExplicitDependencies ();
    }
  }
}