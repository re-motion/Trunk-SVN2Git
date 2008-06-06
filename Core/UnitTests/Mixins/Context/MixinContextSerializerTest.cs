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
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class MixinContextSerializerTest
  {
    private SerializationInfo _serializationInfo;

    [SetUp]
    public void SetUp ()
    {
      _serializationInfo = new SerializationInfo (typeof (MixinContext), new FormatterConverter ());
     
    }

    [Test]
    public void Serialize_MixinKind ()
    {
      MixinContext context1 = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1));
      MixinContext context2 = new MixinContext (MixinKind.Used, typeof (BT1Mixin1));
      MixinContext deserializedContext1 = SerializeAndDeserialize (context1, "key1");
      MixinContext deserializedContext2 = SerializeAndDeserialize (context2, "key2");
      Assert.That (deserializedContext1.MixinKind, Is.EqualTo (MixinKind.Extending));
      Assert.That (deserializedContext2.MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void Serialize_MixinType ()
    {
      MixinContext context = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1));
      MixinContext deserializedContext = SerializeAndDeserialize(context, "key");
      Assert.That (deserializedContext.MixinType, Is.EqualTo (typeof (BT1Mixin1)));
    }

    [Test]
    public void Serialize_ExplicitDependencies ()
    {
      MixinContext contextWithoutDependencies = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1));
      MixinContext contextWithDependencies = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), typeof (BT1Mixin2), typeof (BT2Mixin1));
      MixinContext deserializedContextWithoutDependencies = SerializeAndDeserialize (contextWithoutDependencies, "key1");
      MixinContext deserializedContextWithDependencies = SerializeAndDeserialize (contextWithDependencies, "key2");
      Assert.That (deserializedContextWithoutDependencies.ExplicitDependencies, Is.Empty);
      Assert.That (deserializedContextWithDependencies.ExplicitDependencies, Is.EquivalentTo(new Type[] {typeof (BT1Mixin2), typeof (BT2Mixin1)}));
    }
    
    private MixinContext SerializeAndDeserialize (MixinContext context, string key)
    {
      MixinContextSerializer.SerializeIntoFlatStructure (context, key, _serializationInfo);
      return MixinContextSerializer.DeserializeFromFlatStructure (key, _serializationInfo);
    }
  }
}
