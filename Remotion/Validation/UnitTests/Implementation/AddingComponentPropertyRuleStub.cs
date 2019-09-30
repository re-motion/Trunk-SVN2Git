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
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  public class AddingComponentPropertyRuleStub : IAddingComponentPropertyRule
  {
    private class Collector : IComponentValidationCollector<DomainType>
    {
      public Type ValidatedType => throw new NotImplementedException();

      public IReadOnlyCollection<IAddingComponentPropertyRule> AddedPropertyRules => throw new NotImplementedException();

      public IReadOnlyCollection<IAddingComponentPropertyMetaValidationRule> AddedPropertyMetaValidationRules => throw new NotImplementedException();

      public IReadOnlyCollection<IRemovingComponentPropertyRule> RemovedPropertyRules => throw new NotImplementedException();

      public IAddingComponentRuleBuilder<DomainType, TProperty> AddRule<TProperty> (Expression<Func<DomainType, TProperty>> propertySelector) => throw new NotImplementedException();

      public IRemovingComponentRuleBuilder<DomainType, TProperty> RemoveRule<TProperty> (Expression<Func<DomainType, TProperty>> propertySelector) => throw new NotImplementedException();

      public void When (Func<DomainType, bool> predicate, Action action) => throw new NotImplementedException();

      public void Unless (Func<DomainType, bool> predicate, Action action) => throw new NotImplementedException();
    }

    public class DomainType
    {
      public string DomainProperty { get; set; }
    }

    private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();

    public AddingComponentPropertyRuleStub ()
    {
    }

    public IEnumerable<IPropertyValidator> Validators => _validators;
    public void RegisterValidator (IPropertyValidator propertyValidator) => _validators.Add (propertyValidator);

    public string RuleSet => throw new NotImplementedException();

    public Type CollectorType => typeof (Collector);

    public IPropertyInformation Property
    {
      get { return PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty ((DomainType _) => _.DomainProperty)); }
    }

    public bool IsHardConstraint => throw new NotImplementedException();


    public void SetHardConstraint ()
    {
      throw new NotImplementedException();
    }

    public void ApplyRemoveValidatorRegistrations (IPropertyValidatorExtractor propertyValidatorExtractor)
    {
      throw new NotImplementedException();
    }

    public void ApplyCondition (Func<object, bool> predicate)
    {
      throw new NotImplementedException();
    }

    public IValidationRule CreateValidationRule ()
    {
      throw new NotImplementedException();
    }
  }
}