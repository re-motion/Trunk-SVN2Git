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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Implementation;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceLocatorTest
  {
    private IServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = new DefaultServiceLocator();
    }

    [Test]
    public void GetService_TypeWithoutConcreteImplementatioAttribute ()
    {
      var result = _serviceLocator.GetService (typeof (string));

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ActivationException),
      ExpectedMessage = "The requested service does not have the ConcreteImplementationAttribute applied.")]
    public void GetInstance_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      _serviceLocator.GetInstance (typeof (string));
    }

    [Test]
    public void GetService_TypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetService (typeof (ITestDefaultServiceLocatorAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));
    }

    [Test]
    public void GetInstance_TypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestDefaultServiceLocatorAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));
    }

    [Test]
    public void GetInstance_GetInstancesFromCache ()
    {
      var testableSerciceLocator = new TestableDefaultServiceLocator ();
      testableSerciceLocator.GetInstance (typeof (ITestDefaultServiceLocatorAttributeType));

      Assert.That (testableSerciceLocator.GetInstance (typeof (ITestDefaultServiceLocatorAttributeType)), Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));

      Func<object> instanceCreator;
      Assert.That (testableSerciceLocator.Cache.TryGetValue (typeof (ITestDefaultServiceLocatorAttributeType), out instanceCreator), Is.True);
      Assert.That (((Func<object>) instanceCreator ()) (), Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));
    }

    [Test]
    public void GetInstanceWithKeyParamete_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestDefaultServiceLocatorAttributeType), "Test");

      Assert.That (result, Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));
    }

    [Test]
    public void GetAllInstances ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (ITestDefaultServiceLocatorAttributeType));

      Assert.That (result.ToArray ().Length, Is.EqualTo (1));
      Assert.That (result.ToArray ()[0], Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));
    }

    [Test]
    public void GetAllInstances_ConreteImplementationAttributeIsNotDefined ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (IServiceLocator));

      Assert.That (result.ToArray ().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetInstanceWithGenericType ()
    {
      var result = _serviceLocator.GetInstance<ITestDefaultServiceLocatorAttributeType> ();

      Assert.That (result, Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));
    }

    [Test]
    public void GetInstanceWithGenericTypeAndKeyParameter_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance<ITestDefaultServiceLocatorAttributeType> ("Test");

      Assert.That (result, Is.TypeOf (typeof (TestDefaultServiceLocatorAttributeType)));
    }

    [Test]
    public void GetAllInstancesWithGenericType_ConcreteImplementationAttributeIsNotDefined ()
    {
      var result = _serviceLocator.GetAllInstances<IServiceLocator> ();

      Assert.That (result.ToArray ().Length, Is.EqualTo (0));
    }

  }

  [ConcreteImplementation ("Remotion.UnitTests.ServiceLocation.TestDefaultServiceLocatorAttributeType, Remotion.UnitTests, Version = <version>")]
  internal interface ITestDefaultServiceLocatorAttributeType
  {

  }

  public class TestDefaultServiceLocatorAttributeType : ITestDefaultServiceLocatorAttributeType
  {
    public TestDefaultServiceLocatorAttributeType ()
    {
      
    } 
  }
}