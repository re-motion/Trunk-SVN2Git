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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that the StorageGroupAttribute is not defined twice in the class hierarchy.
  /// </summary>
  public class StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule : IClassDefinitionValidatorRule
  {
    public StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return Validate (classDefinition.ClassType);
    }

    public MappingValidationResult Validate (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (ReflectionUtility.IsInheritanceRoot(type) && Attribute.IsDefined (type.BaseType, typeof (StorageGroupAttribute), true)) 
      {
        Type baseType = type.BaseType;
        while (!AttributeUtility.IsDefined<StorageGroupAttribute> (baseType, false)) //get base type which has the attribute applied
          baseType = baseType.BaseType;

        var message = string.Format (
            "The domain object type cannot redefine the '{0}' already defined on base type '{1}'.",
            typeof (StorageGroupAttribute),
            baseType);
        return new MappingValidationResult (false, message);
      }
      return new MappingValidationResult (true);
    }

    
  }
}