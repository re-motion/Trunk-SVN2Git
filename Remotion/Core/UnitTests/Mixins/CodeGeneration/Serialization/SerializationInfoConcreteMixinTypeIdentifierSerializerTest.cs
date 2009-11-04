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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.Serialization
{
  [TestFixture]
  public class SerializationInfoConcreteMixinTypeIdentifierSerializerTest
  {
    private MethodInfo _simpleMethod;
    private MethodInfo _genericMethod;

    private SerializationInfo _serializationInfo;
    private SerializationInfoConcreteMixinTypeIdentifierSerializer _serializer;

    [SetUp]
    public void SetUp ()
    {
      _simpleMethod = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _genericMethod = typeof (BaseType7).GetMethod ("One");

      _serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      _serializer = new SerializationInfoConcreteMixinTypeIdentifierSerializer (_serializationInfo, "identifier");
    }

    [Test]
    public void AddMixinType ()
    {
      _serializer.AddMixinType (typeof (BT1Mixin1));
      Assert.That (_serializationInfo.GetString ("identifier.MixinType"), Is.EqualTo (typeof (BT1Mixin1).AssemblyQualifiedName));
    }

    [Test]
    public void AddOverriders ()
    {
      _serializer.AddOverriders (new HashSet<MethodInfo> { _simpleMethod });

      Assert.That (_serializationInfo.GetInt32 ("identifier.Overriders.Count"), Is.EqualTo (1));
      Assert.That (_serializationInfo.GetString ("identifier.Overriders[0].DeclaringType"), Is.EqualTo (typeof (BaseType1).AssemblyQualifiedName));
      Assert.That (_serializationInfo.GetString ("identifier.Overriders[0].Name"), Is.EqualTo ("VirtualMethod"));
      Assert.That (_serializationInfo.GetString ("identifier.Overriders[0].Signature"), Is.EqualTo ("System.String VirtualMethod()"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddOverriders_ClosedGeneric ()
    {
      _serializer.AddOverriders (new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod (typeof (int)) });
    }

    [Test]
    public void AddOverridden ()
    {
      _serializer.AddOverridden (new HashSet<MethodInfo> { _simpleMethod });

      Assert.That (_serializationInfo.GetInt32 ("identifier.Overridden.Count"), Is.EqualTo (1));
      Assert.That (_serializationInfo.GetString ("identifier.Overridden[0].DeclaringType"), Is.EqualTo (typeof (BaseType1).AssemblyQualifiedName));
      Assert.That (_serializationInfo.GetString ("identifier.Overridden[0].Name"), Is.EqualTo ("VirtualMethod"));
      Assert.That (_serializationInfo.GetString ("identifier.Overridden[0].Signature"), Is.EqualTo ("System.String VirtualMethod()"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddOverridden_ClosedGeneric ()
    {
      _serializer.AddOverridden (new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod (typeof (int)) });
    }
  }
}
