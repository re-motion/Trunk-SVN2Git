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
using System.Runtime.Serialization;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins;
using System.Linq;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.Serialization
{
  [TestFixture]
  public class SerializationInfoClassContextSerializationTest
  {
    private SerializationInfo _info;
    private SerializationInfoClassContextSerializer _serializer;
    private SerializationInfoClassContextDeserializer _deserializer;

    [SetUp]
    public void SetUp()
    {
      _info = new SerializationInfo (typeof (ClassContext), new FormatterConverter());
      _serializer = new SerializationInfoClassContextSerializer (_info);
      _deserializer = new SerializationInfoClassContextDeserializer (_info);
    }

    [Test]
    public void AddClassType ()
    {
      _serializer.AddClassType (typeof (DateTime));
      Assert.That (_info.GetString ("ClassType.AssemblyQualifiedName"), Is.EqualTo (typeof (DateTime).AssemblyQualifiedName));
      Assert.That (_deserializer.GetClassType (), Is.SameAs (typeof (DateTime)));
    }

    [Test]
    public void AddMixins ()
    {
      var mixinContext1 = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private, new[] { typeof (int), typeof (string) });
      var mixinContext2 = new MixinContext (MixinKind.Used, typeof (BT1Mixin2), MemberVisibility.Public, new[] { typeof (object), typeof (DateTime) });
      var mixinContexts = new[] { mixinContext1, mixinContext2 };
      _serializer.AddMixins (mixinContexts);

      Assert.That (_info.GetInt32 ("Mixins.Count"), Is.EqualTo (2));
      Assert.That (_info.GetValue ("Mixins[0].MixinKind", typeof (MixinKind)), Is.EqualTo (MixinKind.Extending));
      Assert.That (_info.GetValue ("Mixins[1].MixinKind", typeof (MixinKind)), Is.EqualTo (MixinKind.Used));
      Assert.That (_deserializer.GetMixins ().ToArray(), Is.EqualTo (mixinContexts));
    }

    [Test]
    public void AddCompleteInterfaces ()
    {
      _serializer.AddCompleteInterfaces (new[] {typeof (int), typeof (string)});
      Assert.That (_info.GetValue ("CompleteInterfaces.AssemblyQualifiedNames", typeof (string[])), 
          Is.EqualTo (new[] {typeof (int).AssemblyQualifiedName, typeof (string).AssemblyQualifiedName}));
      Assert.That (_deserializer.GetCompleteInterfaces().ToArray(), Is.EqualTo (new[] {typeof (int), typeof (string) }));
    }
  }
}