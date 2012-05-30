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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class MixinContextCollectionTest
  {
    private MixinContextCollection _collection;
    private MixinContextCollection _genericCollection;

    private MixinContext _mcObject;
    private MixinContext _mcString;
    private MixinContext _mcList;
    private MixinContext _mcGeneric;
    private MixinContext _mcDerived;

    [SetUp]
    public void SetUp ()
    {
      _mcObject = MixinContextObjectMother.Create (mixinType: typeof (object));
      _mcString = MixinContextObjectMother.Create (mixinType: typeof (string));
      _mcList = MixinContextObjectMother.Create (mixinType: typeof (List<int>));
      _mcGeneric = MixinContextObjectMother.Create (mixinType: typeof (DerivedGenericMixin<object>));
      _mcDerived = MixinContextObjectMother.Create (mixinType: typeof (DerivedNullMixin));
      _collection = new MixinContextCollection (new[] { _mcObject, _mcString, _mcList, _mcDerived });
      _genericCollection = new MixinContextCollection (new[] { _mcGeneric });
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      Assert.IsTrue (_collection.ContainsAssignableMixin (typeof (object)));
      Assert.IsTrue (_collection.ContainsAssignableMixin (typeof (string)));
      Assert.IsTrue (_collection.ContainsAssignableMixin (typeof (ICollection<int>)));
      Assert.IsFalse (_collection.ContainsAssignableMixin (typeof (ICollection<string>)));
      Assert.IsFalse (_collection.ContainsAssignableMixin (typeof (double)));
      Assert.IsFalse (_collection.ContainsAssignableMixin (typeof (MixinContextCollectionTest)));
    }

    [Test]
    public void ContainsOverrideForMixin ()
    {
      Assert.IsTrue (_collection.ContainsOverrideForMixin (typeof (NullMixin))); // supertype
      Assert.IsFalse (_collection.ContainsOverrideForMixin (typeof (DerivedDerivedNullMixin))); // subtype
      Assert.IsTrue (_collection.ContainsOverrideForMixin (typeof (object))); // same
      Assert.IsFalse (_collection.ContainsOverrideForMixin (typeof (int))); // completely unrelated

    }

    [Test]
    public void ContainsOverrideForMixin_DerivedAndSpecialized ()
    {
      Assert.IsTrue (_genericCollection.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<>))); // supertype, unspecialized
      Assert.IsTrue (_genericCollection.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<object>))); // supertype, same type parameters
      Assert.IsTrue (_genericCollection.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<string>))); // supertype, different type parameters
      Assert.IsTrue (_genericCollection.ContainsOverrideForMixin (typeof (DerivedGenericMixin<>))); // same, unspecialized
      Assert.IsTrue (_genericCollection.ContainsOverrideForMixin (typeof (DerivedGenericMixin<object>))); // same, same type parameters
      Assert.IsTrue (_genericCollection.ContainsOverrideForMixin (typeof (DerivedGenericMixin<string>))); // same, different type parameters
    }
  }
}
