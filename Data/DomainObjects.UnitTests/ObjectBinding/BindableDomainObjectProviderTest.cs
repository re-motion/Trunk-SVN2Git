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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectProviderTest
  {
    [Test]
    public void Instantiate_WithDefaultValues ()
    {
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider();
      Assert.IsInstanceOfType (typeof (BindableDomainObjectMetadataFactory), provider.MetadataFactory);
      Assert.IsInstanceOfType (typeof (BindableObjectServiceFactory), provider.ServiceFactory);
    }

    [Test]
    public void Instantiate_WithMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (BindableDomainObjectMetadataFactory)).AddMixin<MixinStub> ().EnterScope ())
      {
        BindableDomainObjectProvider provider = new BindableDomainObjectProvider ();
        Assert.That (provider.MetadataFactory, Is.InstanceOfType (typeof (BindableDomainObjectMetadataFactory)));
        Assert.That (provider.MetadataFactory, Is.InstanceOfType (typeof (IMixinTarget)));
        Assert.That (provider.ServiceFactory, Is.InstanceOfType (typeof (BindableObjectServiceFactory)));
      }
    }

    [Test]
    public void Instantiate_WithCustomValues ()
    {
      IMetadataFactory metadataFactoryStub = MockRepository.GenerateStub<IMetadataFactory>();
      IBusinessObjectServiceFactory serviceFactoryStub = MockRepository.GenerateStub<IBusinessObjectServiceFactory>();
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider (metadataFactoryStub, serviceFactoryStub);
      
      Assert.AreSame (metadataFactoryStub, provider.MetadataFactory);
      Assert.AreSame (serviceFactoryStub, provider.ServiceFactory);
    }
  }
}
