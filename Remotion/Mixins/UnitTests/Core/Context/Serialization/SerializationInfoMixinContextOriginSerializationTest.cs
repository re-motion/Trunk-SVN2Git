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

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
{
  [TestFixture]
  public class SerializationInfoMixinContextOriginSerializationTest
  {
    private SerializationInfo _info;
    private SerializationInfoMixinContextOriginSerializer _serializer;
    private SerializationInfoMixinContextOriginDeserializer _deserializer;

    [SetUp]
    public void SetUp()
    {
      _info = new SerializationInfo (typeof (MixinContext), new FormatterConverter());
      _serializer = new SerializationInfoMixinContextOriginSerializer (_info, "M1");
      _deserializer = new SerializationInfoMixinContextOriginDeserializer (_info, "M1");
    }

    [Test]
    public void AddKind ()
    {
      _serializer.AddKind ("some kind");
      Assert.That (_info.GetString ("M1.Kind"), Is.EqualTo ("some kind"));
      Assert.That (_deserializer.GetKind(), Is.EqualTo ("some kind"));
    }

    [Test]
    public void AddAssembly ()
    {
      var someAssembly = GetType().Assembly;
      _serializer.AddAssembly (someAssembly);
      Assert.That (_info.GetString ("M1.Assembly.FullName"), Is.EqualTo (someAssembly.FullName));
      Assert.That (_deserializer.GetAssembly(), Is.EqualTo (someAssembly));
    }

    [Test]
    public void AddLocation ()
    {
      _serializer.AddLocation ("some location");
      Assert.That (_info.GetString ("M1.Location"), Is.EqualTo ("some location"));
      Assert.That (_deserializer.GetLocation(), Is.EqualTo ("some location"));
    }
  }
}
