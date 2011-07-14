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
      _referencedDataSourceStub.Mode = DataSourceMode.Edit;
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
    public void HasBusinessObjectChanged_False_DoesNotSaveValueIntoBoundObject ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub))
          .Return (MockRepository.GenerateStub<IBusinessObject>());
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments());
    }

    [Test]
    public void HasBusinessObjectChanged_False_BusinessObject_Null_DoesNotSaveValueIntoBoundObject ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      _referencedDataSourceStub.BusinessObject.Stub (stub => stub.GetProperty (_referencePropertyStub)).Return (null);
      referenceDataSource.LoadValue (false);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments ());
    }

    [Test]
    public void ReferencedDataSource_Null_DoesNotReadBusinessObject_DoesNotClearHasBusinessObjectChanged ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (null, _referencePropertyStub);
      referenceDataSource.BusinessObject = MockRepository.GenerateStub<IBusinessObject>();

      referenceDataSource.SaveValue (false);

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments ());
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

      _referencedDataSourceStub.BusinessObject.AssertWasNotCalled (stub => stub.SetProperty (null, null), options => options.IgnoreArguments ());
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
    public void SavesValuesForBoundControls_BusinessObject_NotNull ()
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
    public void SavesValuesForBoundControls_BusinessObject_Null ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      var controlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      controlMock.Stub (stub => stub.HasValidBinding).Return (true);
      referenceDataSource.Register (controlMock);

      controlMock.Expect (mock => mock.SaveValue (false));
      
      referenceDataSource.SaveValue (false);

      controlMock.VerifyAllExpectations ();
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
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);

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
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);

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
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments ().Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void IsDefaultValue_DoesNotSaveValuesForBoundControls ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      var firstPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty> ();
      var secondPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub.Stub (stub => stub.IsDefaultValue (null, null, null)).IgnoreArguments().Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      firstControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlMock.Stub (stub => stub.HasValue).Return (false);
      firstControlMock.Property = firstPropertyStub;
      referenceDataSource.Register (firstControlMock);

      var secondControlMock = MockRepository.GenerateMock<IBusinessObjectBoundEditableControl> ();
      secondControlMock.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlMock.Stub (stub => stub.HasValue).Return (false);
      secondControlMock.Property = secondPropertyStub;
      referenceDataSource.Register (secondControlMock);

      referenceDataSource.SaveValue (false);

      firstControlMock.AssertWasNotCalled (mock => mock.SaveValue (Arg<bool>.Is.Anything));
      secondControlMock.AssertWasNotCalled (mock => mock.SaveValue (Arg<bool>.Is.Anything));
    }

    [Test]
    public void IsDefaultValue_RequiresAllBoundControlsEmpty_ContainsOnlyEmtpyControls ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      var firstPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      var secondPropertyStub = MockRepository.GenerateStub<IBusinessObjectProperty> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);
      _referencePropertyStub
          .Stub (
              stub =>
              stub.IsDefaultValue (_referencedDataSourceStub.BusinessObject, referencedObject, new[] { firstPropertyStub, secondPropertyStub }))
          .Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (false);
      firstControlStub.Property = firstPropertyStub;
      referenceDataSource.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (false);
      secondControlStub.Property = secondPropertyStub;
      referenceDataSource.Register (secondControlStub);

      var thirdControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      thirdControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      thirdControlStub.Stub (stub => stub.HasValue).Return (false);
      thirdControlStub.Property = firstPropertyStub;
      referenceDataSource.Register (thirdControlStub);

      referenceDataSource.SaveValue (false);

      _referencePropertyStub.AssertWasCalled (stub => stub.Delete (_referencedDataSourceStub.BusinessObject, referencedObject));
      Assert.That (referenceDataSource.BusinessObject, Is.Null);
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void IsDefaultValue_RequiresAllBoundControlsEmpty_ContainsNonEmtpyControls ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (true);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      var firstControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      firstControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      firstControlStub.Stub (stub => stub.HasValue).Return (false);
      referenceDataSource.Register (firstControlStub);

      var secondControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      secondControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      secondControlStub.Stub (stub => stub.HasValue).Return (true);
      referenceDataSource.Register (secondControlStub);

      var thirdControlStub = MockRepository.GenerateStub<IBusinessObjectBoundControl> ();
      thirdControlStub.Stub (stub => stub.HasValidBinding).Return (true);
      thirdControlStub.Stub (stub => stub.HasValue).Return (false);
      referenceDataSource.Register (thirdControlStub);

      referenceDataSource.SaveValue (false);

      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments());
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (null, null), options => options.IgnoreArguments ());
      Assert.That (referenceDataSource.BusinessObject, Is.SameAs (referencedObject));
      Assert.That (referenceDataSource.HasBusinessObjectChanged, Is.False);
    }

    [Test]
    public void BusinessObject_Null_IgnoresDefaultValueSemantics ()
    {
      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = null;

      referenceDataSource.SaveValue (false);

      _referencePropertyStub.AssertWasNotCalled (stub => stub.SupportsDefaultValue);
      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments ());
      _referencePropertyStub.AssertWasNotCalled (stub => stub.Delete (null, null), options => options.IgnoreArguments ());
      _referencedDataSourceStub.BusinessObject.AssertWasCalled (stub => stub.SetProperty (_referencePropertyStub, null));
    }

    [Test]
    public void SupportsDefaultValue_False_IsDefaultValue_NotCalled ()
    {
      var referencedObject = MockRepository.GenerateStub<IBusinessObject> ();

      _referencePropertyStub.Stub (stub => stub.SupportsDefaultValue).Return (false);

      var referenceDataSource = new TestableBusinessObjectReferenceDataSource (_referencedDataSourceStub, _referencePropertyStub);
      referenceDataSource.BusinessObject = referencedObject;

      referenceDataSource.SaveValue (false);

      _referencePropertyStub.AssertWasNotCalled (stub => stub.IsDefaultValue (null, null, null), options => options.IgnoreArguments ());
    }
  }
}