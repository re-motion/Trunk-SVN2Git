// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class ServiceLocatorScopeTest
  {
    private IServiceLocator _locator1;
    private IServiceLocator _locator2;

    [SetUp]
    public void SetUp ()
    {
      _locator1 = MockRepository.GenerateStub<IServiceLocator>();
      _locator2 = MockRepository.GenerateStub<IServiceLocator>();

      ServiceLocator.SetLocatorProvider (null);
    }

    [TearDown]
    public void TearDown ()
    {
      ServiceLocator.SetLocatorProvider (null);
    }

    [Test]
    public void Initialization_AndDispose_InitialLocatorSet ()
    {
      ServiceLocator.SetLocatorProvider (() => _locator1);
      Assert.That (ServiceLocator.Current, Is.SameAs (_locator1));

      using (new ServiceLocatorScope (_locator2))
      {
        Assert.That (ServiceLocator.Current, Is.SameAs (_locator2));
      }

      Assert.That (ServiceLocator.Current, Is.SameAs (_locator1));
    }

    [Test]
    public void Initialization_AndDispose_InitialLocatorNull ()
    {
      ServiceLocator.SetLocatorProvider (() => null);
      Assert.That (ServiceLocator.Current, Is.Null);

      using (new ServiceLocatorScope (_locator2))
      {
        Assert.That (ServiceLocator.Current, Is.SameAs (_locator2));
      }

      Assert.That (ServiceLocator.Current, Is.Null);
    }

    [Test]
    public void Initialization_AndDispose_InitialProviderNull ()
    {
      Assert.That (() => ServiceLocator.Current, Throws.TypeOf<NullReferenceException>());

      using (new ServiceLocatorScope (_locator2))
      {
        Assert.That (ServiceLocator.Current, Is.SameAs (_locator2));
      }

      Assert.That (() => ServiceLocator.Current, Throws.TypeOf<NullReferenceException>());
    }

    [Test]
    public void Initialization_AndDispose_SetNull ()
    {
      ServiceLocator.SetLocatorProvider (() => _locator1);
      Assert.That (ServiceLocator.Current, Is.SameAs (_locator1));

      using (new ServiceLocatorScope (null))
      {
        Assert.That (ServiceLocator.Current, Is.Null);
      }

      Assert.That (ServiceLocator.Current, Is.SameAs (_locator1));
    }
  }
}