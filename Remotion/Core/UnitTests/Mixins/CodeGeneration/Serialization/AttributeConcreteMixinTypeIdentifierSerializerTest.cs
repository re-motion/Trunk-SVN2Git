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

namespace Remotion.UnitTests.Mixins.CodeGeneration.Serialization
{
  [TestFixture]
  public class AttributeConcreteMixinTypeIdentifierSerializerTest
  {
    private MethodInfo _simpleExternalMethod;
    private MethodInfo _simpleMethodOnMixinType;

    private AttributeConcreteMixinTypeIdentifierSerializer _serializer;

    [SetUp]
    public void SetUp ()
    {
      _simpleExternalMethod = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _simpleMethodOnMixinType = typeof (BT1Mixin1).GetMethod ("VirtualMethod");

      _serializer = new AttributeConcreteMixinTypeIdentifierSerializer ();
    }

    [Test]
    public void AddMixinType ()
    {
      _serializer.AddMixinType (typeof (BT1Mixin1));
      Assert.That (_serializer.Values[0], Is.SameAs (typeof (BT1Mixin1)));
    }

    [Test]
    public void AddExternalOverriders ()
    {
      _serializer.AddExternalOverriders (new HashSet<MethodInfo> { _simpleExternalMethod });

      Assert.That (_serializer.Values[1].GetType (), Is.EqualTo (typeof (object[])));
      Assert.That (((object[]) _serializer.Values[1]).Length, Is.EqualTo (1));
      Assert.That (((object[]) ((object[]) _serializer.Values[1])[0]).Length, Is.EqualTo (2));
      Assert.That (((object[]) ((object[]) _serializer.Values[1])[0])[0], Is.SameAs (typeof (BaseType1)));
      Assert.That (((object[]) ((object[]) _serializer.Values[1])[0])[1], Is.EqualTo (_simpleExternalMethod.MetadataToken));
    }

    [Test]
    public void AddWrappedProtectedMembers ()
    {
      _serializer.AddWrappedProtectedMembers (new HashSet<MethodInfo> { _simpleMethodOnMixinType });

      Assert.That (_serializer.Values[2].GetType (), Is.EqualTo (typeof (int[])));
      Assert.That (((int[]) _serializer.Values[2]).Length, Is.EqualTo (1));
      Assert.That (((int[]) _serializer.Values[2])[0], Is.EqualTo (_simpleMethodOnMixinType.MetadataToken));
    }
  }
}