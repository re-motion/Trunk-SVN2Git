﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class AddingPropertyValidationRuleCollector1Test
  {
    private Expression<Func<Customer, string>> _userNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector;
    private IPropertyInformation _property;
    private IPropertyValidatorExtractor _propertyValidatorExtractorMock;
    private StubPropertyValidator _stubPropertyValidator1;
    private NotEmptyValidator _stubPropertyValidator2;
    private NotEqualValidator _stubPropertyValidator3;

    [SetUp]
    public void SetUp ()
    {
      _property = PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName"));

      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);

      _stubPropertyValidator1 = new StubPropertyValidator();
      _stubPropertyValidator2 = new NotEmptyValidator (new InvariantValidationMessage ("Fake Message"));
      _stubPropertyValidator3 = new NotEqualValidator ("gfsf", new InvariantValidationMessage ("Fake Message"));

      _propertyValidatorExtractorMock = MockRepository.GenerateStrictMock<IPropertyValidatorExtractor>();

      _addingPropertyValidationRuleCollector = AddingPropertyValidationRuleCollector.Create (_userNameExpression, typeof (CustomerValidationRuleCollector1));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      var propertyInfo = ((PropertyInfoAdapter) _addingPropertyValidationRuleCollector.Property).PropertyInfo;
      Assert.That (_addingPropertyValidationRuleCollector.Property.Equals(_property), Is.True);
      Assert.That (_addingPropertyValidationRuleCollector.Property, Is.EqualTo (_property));
      Assert.That (propertyInfo.DeclaringType, Is.EqualTo (typeof (Customer)));
      Assert.That (propertyInfo.ReflectedType, Is.EqualTo (typeof (Customer)));
      Assert.That (_addingPropertyValidationRuleCollector.CollectorType, Is.EqualTo (typeof (CustomerValidationRuleCollector1)));
      Assert.That (_addingPropertyValidationRuleCollector.Validators.Any(), Is.False);
      Assert.That (_addingPropertyValidationRuleCollector.IsHardConstraint, Is.False);
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void Initialization_CollectorTypeDoesNotImplementIComponentValidationCollector_ThrowsArgumentException ()
    {
    }

    [Test]
    public void Create_MemberInfoIsNoPropertyInfo_ExceptionIsThrown ()
    {
      var dummyExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.Dummy());

      Assert.That (
          () => AddingPropertyValidationRuleCollector.Create (dummyExpression, typeof (CustomerValidationRuleCollector1)),
          Throws.ArgumentException.With.Message.EqualTo ("Must be a MemberExpression.\r\nParameter name: expression"));
    }

    [Test]
    public void Create_PropertyDeclaredInBaseClass ()
    {
      var componentPropertyRule = AddingPropertyValidationRuleCollector.Create (_lastNameExpression, typeof (CustomerValidationRuleCollector1));
      var propertyInfo = ((PropertyInfoAdapter) componentPropertyRule.Property).PropertyInfo;

      //TODO-5906 simplify assertion with PropertyInfoAdapter compare
      Assert.That (
          MemberInfoEqualityComparer<MemberInfo>.Instance.Equals (propertyInfo, typeof (Customer).GetMember ("LastName")[0]),
          Is.True);
      Assert.That (propertyInfo.DeclaringType, Is.EqualTo (typeof (Person)));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void Create_BuildsFuncForPropertyAccess ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_InitializesDeferredInitializationValidationMessages ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_WhenCalledTwice_InitializesDeferredInitializationValidationMessagesOnlyForNewlyRegisteredValidators ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_IgnoresValidationMessagesForValidatorsWithoutADeferredInitializationValidationMessage ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_WithCondition_UsesConditionForCreatedPropertyValidationRule ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_WithoutCondition_UsesAlwaysTrueConditionForCreatedPropertyValidationRule ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void SetCondition ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void SetCondition_Twice_UsesNewCondition ()
    {
    }

    [Test]
    public void RegisterValidator ()
    {
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator1);
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator2);

      Assert.That (_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo (2));
      Assert.That (
          _addingPropertyValidationRuleCollector.Validators,
          Is.EquivalentTo (new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator2 }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_NoHardConstraint ()
    {
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator1);
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator2);
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator3);
      Assert.That (_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo (3));

      _propertyValidatorExtractorMock
          .Expect (
              mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector))
          .Return (new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator3 });

      _addingPropertyValidationRuleCollector.ApplyRemoveValidatorRegistrations (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (_addingPropertyValidationRuleCollector.Validators, Is.EqualTo (new[] { _stubPropertyValidator2 }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_HardConstraintAndNoValidatorsToRemove_NoExceptionIsThrown ()
    {
      _addingPropertyValidationRuleCollector.SetHardConstraint();
      Assert.That (_addingPropertyValidationRuleCollector.IsHardConstraint, Is.True);
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator1);
      Assert.That (_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo (1));

      _propertyValidatorExtractorMock
          .Stub (
              stub => stub.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector))
          .Return (new IPropertyValidator[0]);

      _addingPropertyValidationRuleCollector.ApplyRemoveValidatorRegistrations (_propertyValidatorExtractorMock);

      Assert.That (_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo (1));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_HardConstraintAndValidatorsToRemove ()
    {
      _addingPropertyValidationRuleCollector.SetHardConstraint();
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator1);
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator2);
      _addingPropertyValidationRuleCollector.RegisterValidator (_ => _stubPropertyValidator3);
      Assert.That (_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo (3));

      _propertyValidatorExtractorMock
          .Stub (
              stub => stub.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector))
          .Return (new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator3 });

      Assert.That (
          () => _addingPropertyValidationRuleCollector.ApplyRemoveValidatorRegistrations (_propertyValidatorExtractorMock),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo (
              "Hard constraint validator(s) 'StubPropertyValidator, NotEqualValidator' on property "
              + "'Remotion.Validation.UnitTests.TestDomain.Customer.UserName' cannot be removed."));
    }

    [Test]
    public void ToString_NoHardConstraint ()
    {
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That (
          result,
          Is.EqualTo ("AddingPropertyValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_HardConstraint ()
    {
      _addingPropertyValidationRuleCollector.SetHardConstraint();
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingPropertyValidationRuleCollector (HARD CONSTRAINT): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithoutCondition ()
    {
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingPropertyValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithCondition ()
    {
      _addingPropertyValidationRuleCollector.SetCondition ((Customer o) => true);
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingPropertyValidationRuleCollector (CONDITIONAL): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithConditionAndHardConstraint ()
    {
      _addingPropertyValidationRuleCollector.SetCondition ((Customer o) => true);
      _addingPropertyValidationRuleCollector.SetHardConstraint();
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingPropertyValidationRuleCollector (CONDITIONAL, HARD CONSTRAINT): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }
  }
}