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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectTest : ObjectBindingBaseTest
  {
    [DBTable]
    [TestDomain]
    public class SampleBindableDomainObject : BindableDomainObject
    {
    }

    [DBTable]
    [TestDomain]
    [Serializable]
    public class SampleBindableDomainObjectWithOverriddenDisplayName : BindableDomainObject
    {
      private int _test;

      [StorageClassNone]
      public int Test
      {
        get { return _test; }
        set { _test = value; }
      }

      public override string DisplayName
      {
        get { return "CustomName"; }
      }
    }

    [Test]
    public void BindableDomainObjectIsDomainObject ()
    {
      Assert.IsTrue (typeof (DomainObject).IsAssignableFrom (typeof (SampleBindableDomainObject)));
    }

    [Test]
    public void BindableDomainObjectAddsMixin ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (SampleBindableDomainObject), typeof (BindableDomainObjectMixin)));
    }

    [Test]
    public void DefaultDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObject)).With();
      Assert.AreEqual (Utilities.TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObject)), businessObject.DisplayName);
    }

    [Test]
    public void OverriddenDisplayName ()
    {
      IBusinessObject businessObject = (IBusinessObject) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();
      Assert.AreNotEqual (
          Utilities.TypeUtility.GetPartialAssemblyQualifiedName (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)),
          businessObject.DisplayName);
      Assert.AreEqual ("CustomName", businessObject.DisplayName);
    }

    [Test]
    public void VerifyInterfaceImplementation ()
    {
      IBusinessObjectWithIdentity businessObject =
          (SampleBindableDomainObjectWithOverriddenDisplayName) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();
      IBusinessObjectWithIdentity businessObjectMixin = Mixin.Get<BindableDomainObjectMixin> (businessObject);

      Assert.AreSame (businessObjectMixin.BusinessObjectClass, businessObject.BusinessObjectClass);
      Assert.AreEqual (businessObjectMixin.DisplayName, businessObject.DisplayName);
      Assert.AreEqual (businessObjectMixin.DisplayNameSafe, businessObject.DisplayNameSafe);
      businessObject.SetProperty ("Test", 1);
      Assert.AreEqual (1, businessObject.GetProperty ("Test"));
      Assert.AreEqual (1, businessObject.GetProperty (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Test")));
      Assert.AreEqual ("001", businessObject.GetPropertyString (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Test"), "000"));
      Assert.AreEqual ("1", businessObject.GetPropertyString ("Test"));
      Assert.AreEqual (businessObjectMixin.UniqueIdentifier, businessObject.UniqueIdentifier);
      businessObject.SetProperty (businessObjectMixin.BusinessObjectClass.GetPropertyDefinition ("Test"), 2);
      Assert.AreEqual (2, businessObject.GetProperty ("Test"));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      SampleBindableDomainObjectWithOverriddenDisplayName domainObject =
          (SampleBindableDomainObjectWithOverriddenDisplayName) RepositoryAccessor.NewObject (typeof (SampleBindableDomainObjectWithOverriddenDisplayName)).With();

      Serializer.SerializeAndDeserialize (domainObject);
    }

    [Test]
    public void GetProviderForBindableObjectType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObject));

      Assert.That (provider, Is.Not.Null);
      Assert.That (provider, Is.InstanceOfType (typeof (BindableDomainObjectProvider)));
      Assert.That (provider, Is.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableDomainObjectProviderAttribute))));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (BindableObjectProviderAttribute))));
    }
  }
}
