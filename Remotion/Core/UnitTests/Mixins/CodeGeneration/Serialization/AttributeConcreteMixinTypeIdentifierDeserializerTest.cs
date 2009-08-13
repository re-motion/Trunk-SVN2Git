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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;
using System.Linq;

namespace Remotion.UnitTests.Mixins.CodeGeneration.Serialization
{
  [TestFixture]
  public class AttributeConcreteMixinTypeIdentifierDeserializerTest
  {
    private MethodInfo _simpleExternalMethod;
    private MethodInfo _genericExternalMethod;
    private MethodInfo _externalMethodOnGenericClosedWithReferenceType;
    private MethodInfo _externalMethodOnGenericClosedWithValueType;
    private MethodInfo _simpleMethodOnMixinType;
    private MethodInfo _simpleMethodOnMixinBaseType;
    private AttributeConcreteMixinTypeIdentifierSerializer _serializer;
    private AttributeConcreteMixinTypeIdentifierDeserializer _deserializer;

    [SetUp]
    public void SetUp ()
    {
      _simpleExternalMethod = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _genericExternalMethod = typeof (BaseType7).GetMethod ("One");
      _externalMethodOnGenericClosedWithReferenceType = typeof (GenericClassWithAllKindsOfMembers<string>).GetMethod ("Method");
      _externalMethodOnGenericClosedWithValueType = typeof (GenericClassWithAllKindsOfMembers<int>).GetMethod ("Method");
      _simpleMethodOnMixinType = typeof (BT1Mixin1).GetMethod ("VirtualMethod");
      _simpleMethodOnMixinBaseType = typeof (object).GetMethod ("ToString");

      _serializer = new AttributeConcreteMixinTypeIdentifierSerializer ();
      _deserializer = new AttributeConcreteMixinTypeIdentifierDeserializer (_serializer.Values);
    }

    [Test]
    public void GetMixinType ()
    {
      _serializer.AddMixinType (typeof (BT1Mixin1));
      Assert.That (_deserializer.GetMixinType (), Is.SameAs (typeof (BT1Mixin1)));
    }

    [Test]
    public void GetExternalOverriders ()
    {
      _serializer.AddExternalOverriders (new HashSet<MethodInfo> { _simpleExternalMethod });
      Assert.That (_deserializer.GetExternalOverriders ().ToArray(), Is.EquivalentTo (new[] { _simpleExternalMethod }));
    }

    [Test]
    public void GetExternalOverriders_GenericMethod ()
    {
      _serializer.AddExternalOverriders (new HashSet<MethodInfo> { _genericExternalMethod });
      Assert.That (_deserializer.GetExternalOverriders ().ToArray (), Is.EquivalentTo (new[] { _genericExternalMethod }));
    }

    [Test]
    public void GetExternalOverriders_MethodOnClosedGenericType_ReferenceType ()
    {
      _serializer.AddExternalOverriders (new HashSet<MethodInfo> { _externalMethodOnGenericClosedWithReferenceType });
      Assert.That (_deserializer.GetExternalOverriders ().ToArray (), Is.EquivalentTo (new[] { _externalMethodOnGenericClosedWithReferenceType }));
    }

    [Test]
    public void GetExternalOverriders_MethodOnClosedGenericType_ValueType ()
    {
      _serializer.AddExternalOverriders (new HashSet<MethodInfo> { _externalMethodOnGenericClosedWithValueType });
      Assert.That (_deserializer.GetExternalOverriders ().ToArray (), Is.EquivalentTo (new[] { _externalMethodOnGenericClosedWithValueType }));
    }

    [Test]
    public void GetWrappedProtectedMembers ()
    {
      _serializer.AddWrappedProtectedMembers (new HashSet<MethodInfo> { _simpleMethodOnMixinType });
      Assert.That (_deserializer.GetWrappedProtectedMembers ().ToArray (), Is.EquivalentTo (new[] { _simpleMethodOnMixinType }));
    }

    [Test]
    public void GetWrappedProtectedMembers_MethodOnBaseType ()
    {
      _serializer.AddWrappedProtectedMembers (new HashSet<MethodInfo> { _simpleMethodOnMixinBaseType });
      Assert.That (_deserializer.GetWrappedProtectedMembers ().ToArray (), Is.EquivalentTo (new[] { _simpleMethodOnMixinBaseType }));
    }
  }
}