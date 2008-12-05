// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
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
