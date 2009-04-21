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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectServiceFactoryTest
  {
    private IBusinessObjectServiceFactory _serviceFactory;
    private IBusinessObjectProviderWithIdentity _provider;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _provider = MockRepository.GenerateStub<IBusinessObjectProviderWithIdentity>();
    }

    [Test]
    public void GetService_FromIBindableObjectGlobalizationService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IBindableObjectGlobalizationService)),
          Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetService_FromIBusinessObjectStringFormatterService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IBusinessObjectStringFormatterService)),
          Is.InstanceOfType (typeof (BusinessObjectStringFormatterService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (_serviceFactory.CreateService (_provider, typeof (IGetObjectService)), Is.Null);
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsService ()
    {
      Assert.That (_serviceFactory.CreateService (_provider, typeof (ISearchAvailableObjectsService)), Is.Null);
    }
  }
}
