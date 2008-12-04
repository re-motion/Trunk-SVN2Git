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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context
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
      _mcObject = new MixinContext (MixinKind.Extending, typeof (object), MemberVisibility.Private);
      _mcString = new MixinContext (MixinKind.Extending, typeof (string), MemberVisibility.Private);
      _mcList = new MixinContext (MixinKind.Extending, typeof (List<int>), MemberVisibility.Private);
      _mcGeneric = new MixinContext (MixinKind.Extending, typeof (DerivedGenericMixin<object>), MemberVisibility.Private);
      _mcDerived = new MixinContext (MixinKind.Extending, typeof (DerivedNullMixin), MemberVisibility.Private);
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
