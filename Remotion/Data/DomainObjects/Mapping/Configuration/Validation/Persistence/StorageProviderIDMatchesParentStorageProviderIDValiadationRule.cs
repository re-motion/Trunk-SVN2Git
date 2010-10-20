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

namespace Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Persistence
{
  /// <summary>
  /// Validates that the StorageProviderID of a class defintion is equal to the StorageProviderID of the base class definition.
  /// </summary>
  public class StorageProviderIDMatchesParentStorageProviderIDValiadationRule : IClassDefinitionValidatorRule
  {
    public StorageProviderIDMatchesParentStorageProviderIDValiadationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      if (classDefinition.BaseClass.StorageProviderID != classDefinition.StorageProviderID)
      {
        var message = string.Format(
            "Cannot derive class '{0}' from base class '{1}' handled by different StorageProviders.",
            classDefinition.ID,
            classDefinition.BaseClass.ID);
        return new MappingValidationResult (false, message);
      }
      return new MappingValidationResult (true);
    }
  }
}