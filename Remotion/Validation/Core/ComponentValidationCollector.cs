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
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Utilities;
using Remotion.Validation.RuleBuilders;

namespace Remotion.Validation
{
  /// <summary>
  /// Provides a base class for declaring the validation rules within a component.
  /// </summary>
  /// <remarks>TODO RM-5906: sample</remarks>
  /// <threadsafety static="true" instance="false" />
  public abstract class ComponentValidationCollector<TValidatedType> : IComponentValidationCollector<TValidatedType>
  {
    private readonly List<IAddingComponentPropertyRule> _addedPropertyRules;
    private readonly List<IAddingComponentPropertyMetaValidationRule> _addedPropertyMetaValidationRules;
    private readonly List<IRemovingComponentPropertyRule> _removedPropertyRules;

    protected ComponentValidationCollector ()
    {
      _addedPropertyRules = new List<IAddingComponentPropertyRule>();
      _addedPropertyMetaValidationRules = new List<IAddingComponentPropertyMetaValidationRule>();
      _removedPropertyRules = new List<IRemovingComponentPropertyRule>();
    }

    public Type ValidatedType
    {
      get { return typeof (TValidatedType); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IAddingComponentPropertyRule> AddedPropertyRules
    {
      get { return _addedPropertyRules.AsReadOnly(); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IAddingComponentPropertyMetaValidationRule> AddedPropertyMetaValidationRules
    {
      get { return _addedPropertyMetaValidationRules.AsReadOnly(); }
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IRemovingComponentPropertyRule> RemovedPropertyRules
    {
      get { return _removedPropertyRules.AsReadOnly(); }
    }

    /// <inheritdoc />
    public IAddingComponentRuleBuilder<TValidatedType, TProperty> AddRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull ("propertySelector", propertySelector);
      
      var componentPropertyRule = AddingComponentPropertyRule.Create (propertySelector, GetType());
      _addedPropertyRules.Add (componentPropertyRule);

      var metaValidationPropertyRule = AddingComponentPropertyMetaValidationRule.Create (propertySelector, GetType());
      _addedPropertyMetaValidationRules.Add (metaValidationPropertyRule);

      return new AddingComponentRuleBuilder<TValidatedType, TProperty> (componentPropertyRule, metaValidationPropertyRule);
    }

    /// <inheritdoc />
    public IRemovingComponentRuleBuilder<TValidatedType, TProperty> RemoveRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector)
    {
      ArgumentUtility.CheckNotNull ("propertySelector", propertySelector);
      
      var componentPropertyRule = RemovingComponentPropertyRule.Create (propertySelector, GetType());
      _removedPropertyRules.Add (componentPropertyRule);

      return new RemovingComponentRuleBuilder<TValidatedType, TProperty> (componentPropertyRule);
    }
  }
}