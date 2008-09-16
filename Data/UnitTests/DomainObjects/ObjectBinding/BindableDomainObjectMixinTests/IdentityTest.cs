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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class IdentityTest : ObjectBindingBaseTest
  {
    [Test]
    public void BindableDomainObjectsHaveIdentity ()
    {
      BindableSampleDomainObject domainObject = BindableSampleDomainObject.NewObject ();
      Assert.IsTrue (domainObject is IBusinessObjectWithIdentity);
    }

    [Test]
    public void BindableDomainObjectClassesHaveIdentity ()
    {
      BindableSampleDomainObject domainObject = BindableSampleDomainObject.NewObject ();
      Assert.IsTrue (((IBusinessObjectWithIdentity)domainObject).BusinessObjectClass is IBusinessObjectClassWithIdentity);
    }
    
    [Test]
    public void UniqueIdentifier ()
    {
      BindableSampleDomainObject domainObject = BindableSampleDomainObject.NewObject ();
      Assert.AreEqual (domainObject.ID.ToString (), ((IBusinessObjectWithIdentity) domainObject).UniqueIdentifier);
    }

    [Test]
    public void GetFromUniqueIdentifier ()
    {
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (IGetObjectService), new BindableDomainObjectGetObjectService());
      BindableSampleDomainObject original = BindableSampleDomainObject.NewObject ();
      BindableObjectClassWithIdentity boClass =
          (BindableObjectClassWithIdentity) BindableObjectProvider.GetBindableObjectClass (typeof (BindableSampleDomainObject));
      Assert.AreSame (original, boClass.GetObject (original.ID.ToString ()));
    }
  }
}
