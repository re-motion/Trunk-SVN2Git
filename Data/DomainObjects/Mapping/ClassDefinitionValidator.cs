/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
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

    public virtual void ValidateInheritanceHierarchy (Dictionary<string, List<PropertyDefinition>> persistentPropertyDefinitionsInInheritanceHierarchy)
    {
      ArgumentUtility.CheckNotNull ("persistentPropertyDefinitionsInInheritanceHierarchy", persistentPropertyDefinitionsInInheritanceHierarchy);

      ValidateEntityName ();
      ValidateUniquePropertyDefinitions ();
      ValidateStorageSpecificPropertyNames (persistentPropertyDefinitionsInInheritanceHierarchy);

      foreach (ClassDefinition derivedClassDefinition in _classDefinition.DerivedClasses)
        derivedClassDefinition.GetValidator().ValidateInheritanceHierarchy (persistentPropertyDefinitionsInInheritanceHierarchy);
    }

    private void ValidateEntityName ()
    {
      if (_classDefinition.IsClassTypeResolved)
      {
        if (_classDefinition.GetEntityName () == null && !_classDefinition.IsAbstract)
        {
          throw CreateMappingException (
              "Type '{0}' must be abstract, because neither class '{1}' nor its base classes specify an entity name.",
              _classDefinition.ClassType.AssemblyQualifiedName,
              _classDefinition.ID);
        }
      }

      if (_classDefinition.BaseClass != null && _classDefinition.MyEntityName != null && _classDefinition.BaseClass.GetEntityName () != null && _classDefinition.MyEntityName != _classDefinition.BaseClass.GetEntityName ())
      {
        throw CreateMappingException (
            "Class '{0}' must not specify an entity name '{1}' which is different from inherited entity name '{2}'.",
            _classDefinition.ID,
            _classDefinition.MyEntityName,
            _classDefinition.BaseClass.GetEntityName ());
      }
    }

    private void ValidateUniquePropertyDefinitions ()
    {
      if (_classDefinition.BaseClass != null)
      {
        PropertyDefinitionCollection basePropertyDefinitions = _classDefinition.BaseClass.GetPropertyDefinitions ();
        foreach (PropertyDefinition propertyDefinition in _classDefinition.MyPropertyDefinitions)
        {
          if (basePropertyDefinitions.Contains (propertyDefinition.PropertyName))
          {
            throw CreateMappingException (
                "Class '{0}' must not define property '{1}', because base class '{2}' already defines a property with the same name.",
                _classDefinition.ID,
                propertyDefinition.PropertyName,
                basePropertyDefinitions[propertyDefinition.PropertyName].ClassDefinition.ID);
          }
        }
      }
    }

    private void ValidateStorageSpecificPropertyNames (IDictionary<string, List<PropertyDefinition>> persistentPropertyDefinitionsInInheritanceHierarchy)
    {
      foreach (PropertyDefinition myPropertyDefinition in _classDefinition.MyPropertyDefinitions)
      {
        if (myPropertyDefinition.IsPersistent)
        {
          List<PropertyDefinition> basePropertyDefinitions;
          if (persistentPropertyDefinitionsInInheritanceHierarchy.TryGetValue (myPropertyDefinition.StorageSpecificName, out basePropertyDefinitions)
              && basePropertyDefinitions != null && basePropertyDefinitions.Count > 0)
          {
            PropertyDefinition basePropertyDefinition = basePropertyDefinitions[0];

            throw CreateMappingException (
                "Property '{0}' of class '{1}' must not define storage specific name '{2}',"
                    + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same storage specific name.",
                myPropertyDefinition.PropertyName,
                _classDefinition.ID,
                myPropertyDefinition.StorageSpecificName,
                basePropertyDefinition.ClassDefinition.ID,
                basePropertyDefinition.PropertyName);
          }

          persistentPropertyDefinitionsInInheritanceHierarchy[myPropertyDefinition.StorageSpecificName] =
              new List<PropertyDefinition> (new PropertyDefinition[] {myPropertyDefinition});
        }
      }
    }

    public virtual void ValidateCurrentMixinConfiguration ()
    {
      // do nothing by default, implementations supporting mixins can add validation code checking that the current mixin configuration is compatible
      // with the mixin information stored in the class definition

      // call parent validation, if parent class exists
      if (_classDefinition.BaseClass != null)
        _classDefinition.BaseClass.GetValidator ().ValidateCurrentMixinConfiguration ();
    }

    private static MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}