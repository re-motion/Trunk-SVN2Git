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
using Remotion.ObjectBinding.UnitTests.Core.BusinessObjectReferenceDataSourceBaseTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectReferenceDataSourceBaseTests
{
  [TestFixture]
  public class LoadValue
  {
    private IBusinessObjectReferenceProperty _referencePropertyStub;
    private IBusinessObjectDataSource _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      _referencedDataSourceStub.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      _referencePropertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
    }

    [Test]
    public void SetsBusinessObject_LoadsValueFromBoundObject ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
    }

    [Test]
    public void SetsBusinessObject_Clears_HasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub))
          .Return (MockRepository.GenerateStub<IBusinessObject>());

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void ReferencedDataSource_Null_DoesNotSetBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (null, _referencePropertyStub);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      referenceDataSource.BusinessObject = expectedValue;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedDataSource_BusinessObject_Null_DoesNotSetBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject = null;
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      referenceDataSource.BusinessObject = expectedValue;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedProperty_Null_DoesNotSetBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, null);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      referenceDataSource.BusinessObject = expectedValue;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void LoadsValuesForBoundControls ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (referencedObject);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundControl>();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundControl>();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (secondControlMock);

      firstControlMock.Expect(mock => mock.LoadValue (false))
          .WhenCalled (mi=>Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject)));
      secondControlMock.Expect(mock => mock.LoadValue (false))
          .WhenCalled (mi=>Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject)));

      referenceDataSource.LoadValue (false);

      firstControlMock.VerifyAllExpectations();
      secondControlMock.VerifyAllExpectations();
    }

    [Test]
    public void SetsDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (expectedValue));
    }

    [Test]
    public void SetsDefaultValue_Clears_HasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      var expectedValue = MockRepository.GenerateStub<IBusinessObject> ();
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject)).Return (expectedValue);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void DataSourceMode_Read_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Read;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.CreateDefaultValue (Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void DataSourceMode_Search_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Search;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.CreateDefaultValue (Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void SupportsDefaultValue_False_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.CreateDefaultValue (Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void ReferenceProperty_ReturnsValue_False_DoesNotSetDefaultValue ()
    {
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub))
          .Return (MockRepository.GenerateStub<IBusinessObject>());
      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.CreateDefaultValue (_referencedDataSourceStub.BusinessObject))
          .Return (MockRepository.GenerateStub<IBusinessObject> ());

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.Mode = DataSourceMode.Edit;

      referenceDataSource.LoadValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Not.Null);
    }
  }
}