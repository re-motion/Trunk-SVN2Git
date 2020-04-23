﻿using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocReferenceValueImplementation.Validation
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueValidatorFactoryTest
  {
    private IBocAutoCompleteReferenceValueValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocAutoCompleteReferenceValueValidatorFactory();
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) },
        Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabled (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled (isRequired);
      var validators = _validatorFactory.CreateValidators (control, isReadonly);

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, true, true, true, new[] { typeof (RequiredFieldValidator), typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, true, true, true, new[] { typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase (true, false, false, false, true, new[] { typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase (true, false, true, false, true, new[] { typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase (true, false, true, true, false, new[] { typeof (BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabled (bool isRequired, bool isReadonly, bool hasDataSource, bool hasBusinessObject, bool hasProperty, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled (isRequired, hasDataSource, hasBusinessObject, hasProperty);
      var validators = _validatorFactory.CreateValidators (control, isReadonly);

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    private IBocAutoCompleteReferenceValue GetControlWithOptionalValidatorsEnabled (bool isRequired)
    {
      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
      propertyStub.Stub (p => p.IsNullable).Throw (new InvalidOperationException ("Property.IsNullable is not relevant with optional validators enabled."));
      propertyStub.Stub (p => p.IsRequired).Throw (new InvalidOperationException ("Property.IsRequired is not relevant."));

      var controlMock = MockRepository.GenerateMock<IBocAutoCompleteReferenceValue>();
      controlMock.Expect (c => c.IsRequired).Return (isRequired);
      controlMock.Expect (c => c.TextBoxStyle).Return (new TextBoxStyle());
      controlMock.Expect (c => c.AreOptionalValidatorsEnabled).Return (true);
      controlMock.Expect (c => c.Property).Return (propertyStub);
      controlMock.Expect (c => c.DataSource).Return (dataSourceStub);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }

    private IBocAutoCompleteReferenceValue GetControlWithOptionalValidatorsDisabled (bool isRequired, bool hasDataSource = true, bool hasBusinessObject = true, bool hasProperty = true)
    {
      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      dataSourceStub.BusinessObject = hasBusinessObject ? MockRepository.GenerateStub<IBusinessObject>() : null;
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
      propertyStub.Stub (p => p.IsNullable).Return (!isRequired);
      propertyStub.Stub (p => p.IsRequired).Throw (new InvalidOperationException ("Property.IsRequired is not relevant."));

      var controlMock = MockRepository.GenerateMock<IBocAutoCompleteReferenceValue>();
      controlMock.Expect (c => c.IsRequired).Throw (new InvalidOperationException ("Control settings are not relevant with optional validators disabled."));
      controlMock.Expect (c => c.TextBoxStyle).Return (new TextBoxStyle());
      controlMock.Expect (c => c.AreOptionalValidatorsEnabled).Return (false);
      controlMock.Expect (c => c.Property).Return (hasProperty ? propertyStub : null);
      controlMock.Expect (c => c.DataSource).Return (hasDataSource ? dataSourceStub : null);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }
  }
}