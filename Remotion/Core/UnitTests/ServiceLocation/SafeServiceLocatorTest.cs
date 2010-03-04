// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class SafeServiceLocatorTest
  {
    private ServiceLocatorProvider _serviceLocatorProviderBackup;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      _serviceLocatorProviderBackup = (ServiceLocatorProvider) PrivateInvoke.GetNonPublicStaticField (typeof (ServiceLocator), "currentProvider");
    }

    [Test]
    public void GetCurrent_WithLocatorProvider()
    {
      var serviceLocatorStub = MockRepository.GenerateStub <IServiceLocator>();
      ServiceLocator.SetLocatorProvider (() => serviceLocatorStub);

      Assert.That (SafeServiceLocator.Current, Is.SameAs (serviceLocatorStub));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_ReturnsNullServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider (null);

      Assert.That (SafeServiceLocator.Current, Is.SameAs (NullServiceLocator.Instance));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_SetsServiceLocatorCurrent ()
    {
      ServiceLocator.SetLocatorProvider (null);

      Dev.Null = SafeServiceLocator.Current;
      Assert.That (ServiceLocator.Current, Is.SameAs (NullServiceLocator.Instance));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_ReturnsNullServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider (() => null);

      Assert.That (SafeServiceLocator.Current, Is.SameAs (NullServiceLocator.Instance));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_DoesNotSetServiceLocatorCurrent ()
    {
      ServiceLocator.SetLocatorProvider (() => null);

      Dev.Null = SafeServiceLocator.Current;
      Assert.That (ServiceLocator.Current, Is.Null);
    }

    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (ServiceLocator), "currentProvider", _serviceLocatorProviderBackup);
    }
  }
}