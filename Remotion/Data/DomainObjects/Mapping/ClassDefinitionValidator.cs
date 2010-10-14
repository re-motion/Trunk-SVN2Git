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
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class ClassDefinitionValidator
  {
    private readonly ClassDefinition _classDefinition;

    public ClassDefinitionValidator (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      _classDefinition = classDefinition;
    }

    public virtual void ValidateInheritanceHierarchy ()
    {
      ValidateEntityName ();
      ValidateUniquePropertyDefinitions ();
      ValidateStorageSpecificPropertyNames ();

      foreach (ClassDefinition derivedClassDefinition in _classDefinition.DerivedClasses)
        derivedClassDefinition.GetValidator ().ValidateInheritanceHierarchy ();
    }

    private void ValidateEntityName ()
    {
      var entityNameValidationRule = new NonAbstractClassHasEntityNameValidationRule();
      var entityNameValidationResult = entityNameValidationRule.Validate (_classDefinition);
      if (!entityNameValidationResult.IsValid)
        throw CreateMappingException (entityNameValidationResult.Message);

      var parentEntityNameValidationRule = new EntityNameMatchesParentEntityNameValidationRule();
      var parentEntityNameValidationResult = parentEntityNameValidationRule.Validate (_classDefinition);
      if (!parentEntityNameValidationResult.IsValid)
        throw CreateMappingException (parentEntityNameValidationResult.Message);
    }

    private void ValidateUniquePropertyDefinitions ()
    {
      var validationRule = new PropertyNamesAreUniqueWithinInheritanceTreeValidationRule();
      var validationResult = validationRule.Validate (_classDefinition);
      if (!validationResult.IsValid)
        throw CreateMappingException (validationResult.Message);
    }

    private void ValidateStorageSpecificPropertyNames ()
    {
      var validationRule = new StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule();
      var validationResult = validationRule.Validate (_classDefinition);
      if(!validationResult.IsValid)
        throw CreateMappingException (validationResult.Message);
    }

    public virtual void ValidateCurrentMixinConfiguration ()
    {
      // do nothing by default, implementations supporting mixins can add validation code checking that the current mixin configuration is compatible
      // with the mixin information stored in the class definition
    }

    private static MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}
