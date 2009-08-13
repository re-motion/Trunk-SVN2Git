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

namespace Remotion.UnitTests.Mixins.CodeGeneration.Serialization
{
  [TestFixture]
  public class SerializationInfoConcreteMixinTypeIdentifierSerializerTest
  {
    private MethodInfo _simpleExternalMethod;
    private MethodInfo _simpleMethodOnMixinType;
    private MethodInfo _genericMethod;

    private SerializationInfo _serializationInfo;
    private SerializationInfoConcreteMixinTypeIdentifierSerializer _serializer;

    [SetUp]
    public void SetUp ()
    {
      _simpleExternalMethod = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _simpleMethodOnMixinType = typeof (BT1Mixin1).GetMethod ("VirtualMethod");
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
    public void AddExternalOverriders ()
    {
      _serializer.AddExternalOverriders (new HashSet<MethodInfo> { _simpleExternalMethod });

      Assert.That (_serializationInfo.GetInt32 ("identifier.ExternalOverriders.Count"), Is.EqualTo (1));
      Assert.That (_serializationInfo.GetString ("identifier.ExternalOverriders[0].DeclaringType"), Is.EqualTo (typeof (BaseType1).AssemblyQualifiedName));
      Assert.That (_serializationInfo.GetString ("identifier.ExternalOverriders[0].Name"), Is.EqualTo ("VirtualMethod"));
      Assert.That (_serializationInfo.GetString ("identifier.ExternalOverriders[0].Signature"), Is.EqualTo ("System.String VirtualMethod()"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddExternalOverriders_ClosedGeneric ()
    {
      _serializer.AddExternalOverriders (new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod (typeof (int)) });
    }

    [Test]
    public void AddWrappedProtectedMembers ()
    {
      _serializer.AddWrappedProtectedMembers (new HashSet<MethodInfo> { _simpleMethodOnMixinType });

      Assert.That (_serializationInfo.GetInt32 ("identifier.WrappedProtectedMembers.Count"), Is.EqualTo (1));
      Assert.That (_serializationInfo.GetString ("identifier.WrappedProtectedMembers[0].DeclaringType"), Is.EqualTo (typeof (BT1Mixin1).AssemblyQualifiedName));
      Assert.That (_serializationInfo.GetString ("identifier.WrappedProtectedMembers[0].Name"), Is.EqualTo ("VirtualMethod"));
      Assert.That (_serializationInfo.GetString ("identifier.WrappedProtectedMembers[0].Signature"), Is.EqualTo ("System.String VirtualMethod()"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddWrappedProtectedMembers_ClosedGeneric ()
    {
      _serializer.AddWrappedProtectedMembers (new HashSet<MethodInfo> { _genericMethod.MakeGenericMethod (typeof (int)) });
    }
  }
}