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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectDefaultValueStrategyTest : ObjectBindingBaseTest
  {
    [Test]
    public void IsDefaultDefault_StateIsNotNew ()
    {
      var instance = SampleBindableDomainObject.NewObject ();

      ClientTransactionMock.Delete (instance);

      var property = GetProperty (instance);
      var strategy = new BindableDomainObjectDefaultValueStrategy();
      
      var result = strategy.IsDefaultValue (instance, property);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsDefaultDefault_StateIsNewAndPropertyHasBeenTouched ()
    {
      var instance = SampleBindableDomainObject.NewObject ();

      var property = GetProperty (instance);
      var strategy = new BindableDomainObjectDefaultValueStrategy ();
      instance.Name = "Test";
      
      var result = strategy.IsDefaultValue (instance, property);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsDefaultDefault_StateIsNewAndPropertyHasNotBeenTouched ()
    {
      var instance = SampleBindableDomainObject.NewObject ();

      var property = GetProperty (instance);
      var strategy = new BindableDomainObjectDefaultValueStrategy ();
      
      var result = strategy.IsDefaultValue (instance, property);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsDefaultDefault_StateIsNewAndPropertyCannotBeResolved ()
    {
      var strategy = new BindableDomainObjectDefaultValueStrategy ();
      var instance = SampleBindableDomainObject.NewObject ();

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (bool));
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeof (bool));
      propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeof (bool));
      var property = CreateProperty (propertyInformationStub);

      var result = strategy.IsDefaultValue (instance, property);

      Assert.That (result, Is.False);
    }

    private PropertyBase GetProperty (IBusinessObject instance)
    {
      return (PropertyBase) instance.BusinessObjectClass.GetPropertyDefinition ("Name");
    }

    private BooleanProperty CreateProperty (IPropertyInformation propertyInformation)
    {
      var businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();
      return new BooleanProperty (GetPropertyParameters (propertyInformation, businessObjectProvider));
    }

    private BindableObjectProvider CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ()
    {
      return new BindableObjectProvider (BindableObjectMetadataFactory.Create (), MockRepository.GenerateStub<IBusinessObjectServiceFactory> ());
    }

    private PropertyBase.Parameters GetPropertyParameters (IPropertyInformation property, BindableObjectProvider provider)
    {
      PropertyReflector reflector = PropertyReflector.Create (property, provider);
      return (PropertyBase.Parameters) PrivateInvoke.InvokeNonPublicMethod (
          reflector, typeof (PropertyReflector), "CreateParameters", GetUnderlyingType (reflector));
    }

    private Type GetUnderlyingType (PropertyReflector reflector)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (reflector, typeof (PropertyReflector), "GetUnderlyingType");
    }
  }
}