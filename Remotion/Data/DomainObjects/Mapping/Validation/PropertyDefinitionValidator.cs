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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation
{
  /// <summary>
  /// Holds a read-only collection of class definition validation rules and exposes a Validate-method, which gets a list of 
  /// class definitions to validate. Each validation rule is applied to all property definition for each class definition and for every rule which 
  /// is invalid a mapping validation result with the respective error message is returned.
  /// </summary>
  public class PropertyDefinitionValidator : IPropertyDefinitionValidator
  {
    private readonly ReadOnlyCollection<IPropertyDefinitionValidationRule> _validationRules;

    public static PropertyDefinitionValidator Create ()
    {
      return new PropertyDefinitionValidator (
        new MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule(),
        new MappingAttributesAreSupportedForPropertyTypeValidationRule(),
        new StorageClassIsSupportedValidationRule(),
        new PropertyTypeIsSupportedValidationRule());
    }

    public PropertyDefinitionValidator (params IPropertyDefinitionValidationRule[] classDefinitionValidationRules)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classDefinitionValidationRules", classDefinitionValidationRules);

      _validationRules = Array.AsReadOnly (classDefinitionValidationRules);
    }

    public ReadOnlyCollection<IPropertyDefinitionValidationRule> ValidationRules
    {
      get { return _validationRules; }
    }

    public IEnumerable<MappingValidationResult> Validate (IEnumerable<ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classDefinitions", classDefinitions);

      return _validationRules.SelectMany (rule => classDefinitions, (rule, cd) => rule.Validate (cd)).Where (result => !result.IsValid);
    }
  }
}