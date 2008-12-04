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

namespace Remotion.UnitTests.Mixins.Context.Serialization
{
  [TestFixture]
  public class SerializationInfoMixinContextSerializationTest
  {
    private SerializationInfo _info;
    private SerializationInfoMixinContextSerializer _serializer;
    private SerializationInfoMixinContextDeserializer _deserializer;

    [SetUp]
    public void SetUp()
    {
      _info = new SerializationInfo (typeof (MixinContext), new FormatterConverter());
      _serializer = new SerializationInfoMixinContextSerializer (_info, "M1.");
      _deserializer = new SerializationInfoMixinContextDeserializer (_info, "M1.");
    }

    [Test]
    public void AddMixinType ()
    {
      _serializer.AddMixinType (typeof (DateTime));
      Assert.That (_info.GetString ("M1.MixinType.AssemblyQualifiedName"), Is.EqualTo (typeof (DateTime).AssemblyQualifiedName));
      Assert.That (_deserializer.GetMixinType (), Is.SameAs (typeof (DateTime)));
    }

    [Test]
    public void AddMixinKind ()
    {
      _serializer.AddMixinKind (MixinKind.Used);
      Assert.That (_info.GetValue ("M1.MixinKind", typeof (MixinKind)), Is.EqualTo (MixinKind.Used));
      Assert.That (_deserializer.GetMixinKind (), Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void AddExplicitDependencies ()
    {
      _serializer.AddExplicitDependencies (new[] {typeof (int), typeof (string)});
      Assert.That (_info.GetValue ("M1.ExplicitDependencies.AssemblyQualifiedNames", typeof (string[])), 
          Is.EqualTo (new[] {typeof (int).AssemblyQualifiedName, typeof (string).AssemblyQualifiedName}));
      Assert.That (_deserializer.GetExplicitDependencies ().ToArray(), Is.EqualTo (new[] {typeof (int), typeof (string) }));
    }

    [Test]
    public void AddIntroducedMemberVisibility()
    {
      _serializer.AddIntroducedMemberVisibility (MemberVisibility.Public);
      Assert.That (_info.GetValue ("M1.IntroducedMemberVisibility", typeof (MemberVisibility)), Is.EqualTo (MemberVisibility.Public));
      Assert.That (_deserializer.GetIntroducedMemberVisibility (), Is.EqualTo (MemberVisibility.Public));
    }
  }
}