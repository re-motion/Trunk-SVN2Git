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
using NUnit.Framework;
using System.Runtime.Serialization;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using System.Linq;

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
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
      _serializer = new SerializationInfoMixinContextSerializer (_info, "M1");
      _deserializer = new SerializationInfoMixinContextDeserializer (_info, "M1");
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
    public void AddIntroducedMemberVisibility ()
    {
      _serializer.AddIntroducedMemberVisibility (MemberVisibility.Public);
      Assert.That (_info.GetValue ("M1.IntroducedMemberVisibility", typeof (MemberVisibility)), Is.EqualTo (MemberVisibility.Public));
      Assert.That (_deserializer.GetIntroducedMemberVisibility (), Is.EqualTo (MemberVisibility.Public));
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
    public void AddOrigin ()
    {
      var origin = MixinContextOriginObjectMother.Create();

      _serializer.AddOrigin (origin);

      Assert.That (_info.GetString ("M1.Origin.Kind"), Is.EqualTo (origin.Kind));
      Assert.That (_deserializer.GetOrigin(), Is.EqualTo (origin));
    }
  }
}
