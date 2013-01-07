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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
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
      _serializer = new SerializationInfoClassContextSerializer (_info, "C1");
      _deserializer = new SerializationInfoClassContextDeserializer (_info, "C1");
    }

    [Test]
    public void AddClassType ()
    {
      _serializer.AddClassType (typeof (DateTime));
      Assert.That (_info.GetString ("C1.ClassType.AssemblyQualifiedName"), Is.EqualTo (typeof (DateTime).AssemblyQualifiedName));
      Assert.That (_deserializer.GetClassType (), Is.SameAs (typeof (DateTime)));
    }

    [Test]
    public void AddMixins ()
    {
      var mixinContext1 = MixinContextObjectMother.Create (mixinKind: MixinKind.Extending);
      var mixinContext2 = MixinContextObjectMother.Create (mixinKind: MixinKind.Used);
      var mixinContexts = new[] { mixinContext1, mixinContext2 };
      _serializer.AddMixins (mixinContexts);

      Assert.That (_info.GetInt32 ("C1.Mixins.Count"), Is.EqualTo (2));
      Assert.That (_info.GetValue ("C1.Mixins[0].MixinKind", typeof (MixinKind)), Is.EqualTo (MixinKind.Extending));
      Assert.That (_info.GetValue ("C1.Mixins[1].MixinKind", typeof (MixinKind)), Is.EqualTo (MixinKind.Used));
      Assert.That (_deserializer.GetMixins ().ToArray(), Is.EqualTo (mixinContexts));
    }

    [Test]
    public void AddComposedInterfaces ()
    {
      _serializer.AddComposedInterfaces (new[] {typeof (int), typeof (string)});
      Assert.That (_info.GetValue ("C1.ComposedInterfaces.AssemblyQualifiedNames", typeof (string[])), 
          Is.EqualTo (new[] {typeof (int).AssemblyQualifiedName, typeof (string).AssemblyQualifiedName}));
      Assert.That (_deserializer.GetComposedInterfaces().ToArray(), Is.EqualTo (new[] {typeof (int), typeof (string) }));
    }
  }
}
