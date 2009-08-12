// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Linq;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.Serialization
{
  [TestFixture]
  public class ConcreteMixinTypeIdentifierSerializerTest
  {
    private MethodInfo _simpleExternalMethod;
    private MethodInfo _genericExternalMethod;
    private MethodInfo _externalMethodOnGenericClosedWithReferenceType;
    private MethodInfo _externalMethodOnGenericClosedWithValueType;
    private MethodInfo _simpleMethodOnMixinType;

    [SetUp]
    public void SetUp ()
    {
      _simpleExternalMethod = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _genericExternalMethod = typeof (BaseType7).GetMethod ("One");
      _externalMethodOnGenericClosedWithReferenceType = typeof (GenericClassWithAllKindsOfMembers<string>).GetMethod ("Method");
      _externalMethodOnGenericClosedWithValueType = typeof (GenericClassWithAllKindsOfMembers<int>).GetMethod ("Method");
      _simpleMethodOnMixinType = typeof (BT1Mixin1).GetMethod ("VirtualMethod");
    }

    [Test]
    public void Serialize_MixinType ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo>(),
          new HashSet<MethodInfo>());
      
      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      Assert.That (serializationInfo.GetString ("identifier.MixinType"), Is.EqualTo (typeof (BT1Mixin1).AssemblyQualifiedName));
    }

    [Test]
    public void Deserialize_MixinType ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo>(),
          new HashSet<MethodInfo>());

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      var deserializedResult = ConcreteMixinTypeIdentifierSerializer.Deserialize (serializationInfo, "identifier");
      Assert.That (deserializedResult.MixinType, Is.SameAs (typeof (BT1Mixin1)));
    }

    [Test]
    public void Serialize_ExternalOverriders ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _simpleExternalMethod },
          new HashSet<MethodInfo> ());

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      Assert.That (serializationInfo.GetInt32 ("identifier.ExternalOverriders.Count"), Is.EqualTo (1));
      Assert.That (serializationInfo.GetString ("identifier.ExternalOverriders[0].DeclaringType"), Is.EqualTo (typeof (BaseType1).AssemblyQualifiedName));
      Assert.That (serializationInfo.GetInt32 ("identifier.ExternalOverriders[0].MetadataToken"), Is.EqualTo (_simpleExternalMethod.MetadataToken));
    }

    [Test]
    public void Deserialize_ExternalOverriders ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _simpleExternalMethod },
          new HashSet<MethodInfo> ());

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      var deserializedIdentifier = ConcreteMixinTypeIdentifierSerializer.Deserialize (serializationInfo, "identifier");
      Assert.That (deserializedIdentifier.ExternalOverriders.ToArray(), Is.EquivalentTo (new[] { _simpleExternalMethod }));
    }

    [Test]
    public void Deserialize_ExternalOverriders_GenericMethod ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _genericExternalMethod },
          new HashSet<MethodInfo> ());

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      var deserializedIdentifier = ConcreteMixinTypeIdentifierSerializer.Deserialize (serializationInfo, "identifier");
      Assert.That (deserializedIdentifier.ExternalOverriders.ToArray (), Is.EquivalentTo (new[] { _genericExternalMethod }));
    }

    [Test]
    public void Deserialize_ExternalOverriders_MethodOnClosedGenericType_ReferenceType ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _externalMethodOnGenericClosedWithReferenceType },
          new HashSet<MethodInfo> ());

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      var deserializedIdentifier = ConcreteMixinTypeIdentifierSerializer.Deserialize (serializationInfo, "identifier");
      Assert.That (deserializedIdentifier.ExternalOverriders.ToArray (), Is.EquivalentTo (new[] { _externalMethodOnGenericClosedWithReferenceType }));
    }

    [Test]
    public void Deserialize_ExternalOverriders_MethodOnClosedGenericType_ValueType ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _externalMethodOnGenericClosedWithValueType },
          new HashSet<MethodInfo> ());

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      var deserializedIdentifier = ConcreteMixinTypeIdentifierSerializer.Deserialize (serializationInfo, "identifier");
      Assert.That (deserializedIdentifier.ExternalOverriders.ToArray (), Is.EquivalentTo (new[] { _externalMethodOnGenericClosedWithValueType }));
    }

    [Test]
    public void Serialize_WrappedMembers ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> (),
          new HashSet<MethodInfo> { _simpleMethodOnMixinType });

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      Assert.That (serializationInfo.GetInt32 ("identifier.WrappedProtectedMembers.Count"), Is.EqualTo (1));
      Assert.That (serializationInfo.GetInt32 ("identifier.WrappedProtectedMembers[0].MetadataToken"), 
                   Is.EqualTo (_simpleMethodOnMixinType.MetadataToken));
    }

    [Test]
    public void Deserialize_WrappedMembers ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> (),
          new HashSet<MethodInfo> { _simpleMethodOnMixinType });

      var serializationInfo = new SerializationInfo (typeof (ConcreteMixinTypeIdentifier), new FormatterConverter ());
      ConcreteMixinTypeIdentifierSerializer.Serialize (identifier, serializationInfo, "identifier");

      var deserializedIdentifier = ConcreteMixinTypeIdentifierSerializer.Deserialize (serializationInfo, "identifier");
      Assert.That (deserializedIdentifier.WrappedProtectedMembers.ToArray (), Is.EquivalentTo (new[] { _simpleMethodOnMixinType }));
    }


  }
}