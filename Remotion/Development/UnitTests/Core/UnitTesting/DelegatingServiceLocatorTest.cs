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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class DelegatingServiceLocatorTest
  {
    private IServiceLocator _innerLocatorMock;

    private DelegatingServiceLocator _locator;

    [SetUp]
    public void SetUp ()
    {
      _innerLocatorMock = MockRepository.GenerateStrictMock<IServiceLocator>();

      _locator = new DelegatingServiceLocator (_innerLocatorMock);
    }

    [Test]
    public void GetInstance_Delegates ()
    {
      var type = typeof (int);
      var fakeInstance = new object();
      _innerLocatorMock.Expect (mock => mock.GetInstance (type, "blub")).Return (fakeInstance);

      var result = _locator.GetInstance (type, "blub");

      _innerLocatorMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeInstance));
    }

    [Test]
    public void GetInstance_UsesRegisteredCreator ()
    {
      var type = typeof (int);
      var instance = new object();
      _locator.Register (type, () => instance);

      var result = _locator.GetInstance (type, "blub");

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    public void GetAllInstance_Delegates ()
    {
      var type = typeof (int);
      var fakeInstances = new object[] { "one", 2, "three" };
      _innerLocatorMock.Expect (mock => mock.GetAllInstances (type)).Return (fakeInstances);

      var result = _locator.GetAllInstances (type);

      _innerLocatorMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeInstances));
    }

    [Test]
    public void GetAllInstance_UsesRegisteredCreator ()
    {
      var type = typeof (int);
      var instances = new object[] { "one", 2, "three" };
      _locator.RegisterAll (type, () => instances);

      var result = _locator.GetAllInstances (type);

      Assert.That (result, Is.SameAs (instances));
    }

    [Test]
    public void Registrations_Single_MultipleInstances_AreSeparate ()
    {
      var type1 = typeof (int);
      var type2 = typeof (IDisposable);
      var instance = new object();
      var instances = new object[] { "one", 2, "three" };
      _locator.Register (type1, () => instance);
      _locator.RegisterAll (type2, () => instances);

      var fakeInstance = new object();
      var fakeInstances = new object[] { "one", 2, "three" };
      _innerLocatorMock.Expect (mock => mock.GetInstance (type2, "blub")).Return (fakeInstance);
      _innerLocatorMock.Expect (mock => mock.GetAllInstances (type1)).Return (fakeInstances);

      var result1 = _locator.GetInstance (type2, "blub");
      var result2 = _locator.GetAllInstances (type1);

      _innerLocatorMock.VerifyAllExpectations();
      Assert.That (result1, Is.SameAs (fakeInstance));
      Assert.That (result2, Is.SameAs (fakeInstances));
    }
  }
}