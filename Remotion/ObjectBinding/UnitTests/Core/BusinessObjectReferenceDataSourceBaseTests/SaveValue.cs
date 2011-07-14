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
  public class SaveValue
  {
    private IBusinessObjectReferenceProperty _referencePropertyStub;
    private IBusinessObjectDataSource _referencedDataSourceStub;

    [SetUp]
    public void SetUp ()
    {
      _referencedDataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      _referencedDataSourceStub.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      _referencePropertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
      _referencePropertyStub.Stub (stub => stub.ReferenceClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
    }

    [Test]
    public void ReadsBusinessObject_SavesValueIntoBoundObject ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = expectedValue;

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, expectedValue));
    }

    [Test]
    public void ReadsBusinessObject_Clears_HasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void HasBusinessObjectChanged_False_DoesNotReadBusinessObject ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (
          stub => stub.SetProperty (Arg.Is (_referencePropertyStub), Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void ReferencedDataSource_Null_DoesNotReadBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (null, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (
          stub => stub.SetProperty (Arg.Is (_referencePropertyStub), Arg<IBusinessObject>.Is.Anything));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedDataSource_BusinessObject_Null_DoesNotClearHasBusinessObjectChanged ()
    {
      _referencedDataSourceStub.BusinessObject = null;
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.True);
    }

    [Test]
    public void ReferencedProperty_Null_DoesNotReadBusinessObject ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, null);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (
          stub => stub.SetProperty (Arg.Is (_referencePropertyStub), Arg<IBusinessObject>.Is.Anything));
    }

    [Test]
    public void ReferenceClassRequiresWriteBack_True_ReadsBusinessObject_SavesValueIntoBoundObject ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (expectedValue);
      _referencePropertyStub.ReferenceClass.Stub (stub => stub.RequiresWriteBack).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, expectedValue));
    }

    [Test]
    public void SavesValuesForBoundControls ()
    {
      var expectedValue = MockRepository.GenerateStub<IBusinessObject>();
      bool isControlSaved = false;

      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.SetProperty (_referencePropertyStub, expectedValue))
// ReSharper disable AccessToModifiedClosure
          .WhenCalled (mi => Assert.That (isControlSaved, Is.True));
// ReSharper restore AccessToModifiedClosure

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = expectedValue;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl>();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl>();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (secondControlMock);

      firstControlMock.Expect (mock => mock.SaveValue (false)).WhenCalled (mi => isControlSaved = true);
      secondControlMock.Expect (mock => mock.SaveValue (false));

      referenceDataSource.SaveValue (false);

      firstControlMock.VerifyAllExpectations();
      secondControlMock.VerifyAllExpectations();
    }

    [Test]
    public void IsDefaultValue_True_DeletesObject ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (stub => stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new IBusinessObjectProperty[0]))
          .Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      _referencePropertyStub.AssertWasCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, referencedObject));
    }

    [Test]
    public void IsDefaultValue_SavesNullIntoBoundObject ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (stub => stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new IBusinessObjectProperty[0]))
          .Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, null));
    }

    [Test]
    public void IsDefaultValue_Clears_BusinessObject ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (stub => stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new IBusinessObjectProperty[0]))
          .Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.BusinessObject, Is.Null);
    }

    [Test]
    public void IsDefaultValue_Clears_HasBusinessObjectChanged ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (stub => stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new IBusinessObjectProperty[0]))
          .Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void IsDefaultValue_DoesNotSaveValuesForBoundControls ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (stub => stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new IBusinessObjectProperty[0]))
          .Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (secondControlMock);

      referenceDataSource.SaveValue (false);

      firstControlMock.AssertWasNotCalled (mock => mock.SaveValue (Arg<bool>.Is.Anything));
      secondControlMock.AssertWasNotCalled (mock => mock.SaveValue (Arg<bool>.Is.Anything));
    }

    [Test]
    public void IsDefaultValue_RequiresAllBoundControlsEmpty_ContainsOnlyEmtptyControls ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (stub => stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new IBusinessObjectProperty[0]))
          .Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundControl> ();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      //firstControlMock.Stub (stub => stub.HasValue).Return (true);
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundControl> ();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      //secondControlMock.Stub (stub => stub.HasValue).Return (true);
      referenceDataSource.Register (secondControlMock);

      var thirdControlMock = MockRepository.GenerateMock<IBusinessObjectBoundControl> ();
      thirdControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      //thirdControlMock.Stub (stub => stub.HasValue).Return (true);
      referenceDataSource.Register (thirdControlMock);

      referenceDataSource.SaveValue (false);
    }

    [Test]
    public void IsDefaultValue_PassesAllEmptyProperties ()
    {
      Assert.Ignore ("impl");
    }

    [Test]
    public void BusinessObjectIsNull_IgnoresDefaultValueSemantics ()
    {
      Assert.Ignore ("impl");
    }
  }
}