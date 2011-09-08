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
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocReferenceValueImplementation
{
  [TestFixture]
  public class BusinessObjectServiceContextTest
  {
    private IBusinessObjectDataSource _dataSourceStub;
    private IBusinessObjectClass _dataSourceBusinessObjectClassStub;
    private IBusinessObject _businessObjectStub;
    private IBusinessObjectClass _businessObjectBusinessObjectClassStub;
    private IBusinessObjectWithIdentity _businessObjectWithIdentityStub;
    private IBusinessObjectProperty _propertyStub;

    [SetUp]
    public void SetUp ()
    {
      _dataSourceBusinessObjectClassStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      _dataSourceBusinessObjectClassStub.Stub (stub => stub.Identifier).Return ("DataSourceBusinessObjectClass");

      _dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      _dataSourceStub.Stub (stub => stub.BusinessObjectClass).Return (_dataSourceBusinessObjectClassStub);

      _businessObjectBusinessObjectClassStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      _businessObjectBusinessObjectClassStub.Stub (stub => stub.Identifier).Return ("BusinessObjectBusinessObjectClass");

      _businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      _businessObjectStub.Stub (stub => stub.BusinessObjectClass).Return (_businessObjectBusinessObjectClassStub);

      _businessObjectWithIdentityStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      _businessObjectWithIdentityStub.Stub (stub => stub.UniqueIdentifier).Return ("BusinessObjectIdentifier");

      _propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      _propertyStub.Stub (stub => stub.Identifier).Return ("BusinessObjectProperty");
    }

    [Test]
    public void Create_DataSourceNull_SetsBusinessObjectClassNull ()
    {
      var serviceContext = BusinessObjectServiceContext.Create (null, null);

      Assert.That (serviceContext.BusinessObjectClass, Is.Null);
    }

    [Test]
    public void Create_DataSourceNull_SetsBusinessObjectIdentifierNull ()
    {
      var serviceContext = BusinessObjectServiceContext.Create (null, null);

      Assert.That (serviceContext.BusinessObjectIdentifier, Is.Null);
    }

    [Test]
    public void Create_DataSourceSet_BusinessObjectNull_SetsBusinessObjectClass_FromDataSource_BusinessObjectClass ()
    {
      var serviceContext = BusinessObjectServiceContext.Create (_dataSourceStub, null);

      Assert.That (serviceContext.BusinessObjectClass, Is.EqualTo ("DataSourceBusinessObjectClass"));
    }

    [Test]
    public void Create_BusinessObjectNotNull_SetsBusinessObjectClass_FromDataSource_BusinessObject_BusinessObjectClass ()
    {
      _dataSourceStub.BusinessObject = _businessObjectStub;
      var serviceContext = BusinessObjectServiceContext.Create (_dataSourceStub, null);

      Assert.That (serviceContext.BusinessObjectClass, Is.EqualTo ("BusinessObjectBusinessObjectClass"));
    }

    [Test]
    public void Create_PropertyNull_SetsBusinessObjectPropertyNull ()
    {
      var serviceContext = BusinessObjectServiceContext.Create (null, null);

      Assert.That (serviceContext.BusinessObjectIdentifier, Is.Null);
    }

    [Test]
    public void Create_PropertySet_SetsBusinessObjectProperty ()
    {
      var serviceContext = BusinessObjectServiceContext.Create (null, _propertyStub);

      Assert.That (serviceContext.BusinessObjectProperty, Is.EqualTo ("BusinessObjectProperty"));
    }

    [Test]
    public void Create_BusinessObjectNotNullAndHasNoIdentity_SetsBusinessObjectIdentifierNull ()
    {
      _dataSourceStub.BusinessObject = _businessObjectStub;
      var serviceContext = BusinessObjectServiceContext.Create (_dataSourceStub, null);

      Assert.That (serviceContext.BusinessObjectIdentifier, Is.Null);
    }

    [Test]
    public void Create_BusinessObjectNotNullAndHasIdentity_SetsBusinessObjectIdentifier ()
    {
      _dataSourceStub.BusinessObject = _businessObjectWithIdentityStub;
      var serviceContext = BusinessObjectServiceContext.Create (_dataSourceStub, null);

      Assert.That (serviceContext.BusinessObjectIdentifier, Is.EqualTo ("BusinessObjectIdentifier"));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
       _dataSourceStub.BusinessObject = _businessObjectWithIdentityStub;
      var serviceContext = BusinessObjectServiceContext.Create (_dataSourceStub, _propertyStub);
      var deserialized = Development.UnitTesting.Serializer.SerializeAndDeserialize (serviceContext);
      Assert.That (deserialized.BusinessObjectClass, Is.EqualTo (serviceContext.BusinessObjectClass));
      Assert.That (deserialized.BusinessObjectProperty, Is.EqualTo (serviceContext.BusinessObjectProperty));
      Assert.That (deserialized.BusinessObjectIdentifier, Is.EqualTo (serviceContext.BusinessObjectIdentifier));
    }
  }
}