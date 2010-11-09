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
using System.Linq;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that each defined property definition in a class is not already defined in a base class.
  /// </summary>
  public class PropertyNamesAreUniqueWithinInheritanceTreeValidationRule : IClassDefinitionValidatorRule
  {
    public PropertyNamesAreUniqueWithinInheritanceTreeValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var errorMessages = new StringBuilder ();
      if (classDefinition.BaseClass != null)
      {
        var basePropertyDefinitions = PropertyDefinitionCollection.CreateForAllProperties (classDefinition.BaseClass);
        foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        {
          var propertyName = propertyDefinition.PropertyName;
          var basePropertyDefinition = basePropertyDefinitions.SingleOrDefault(pd=>pd.PropertyName==propertyName);
          if (basePropertyDefinition!=null)
          {
            string message = string.Format("Class '{0}' must not define property '{1}', because base class '{2}' already defines a property with the "
              +"same name.\r\n\r\n"
              +"Declaring type: {3}\r\nProperty: {4}",
                classDefinition.ClassType.Name,
                propertyDefinition.PropertyInfo.Name,
                basePropertyDefinition.ClassDefinition.ClassType.Name,
                classDefinition.ClassType.FullName,
                propertyDefinition.PropertyInfo.Name);
            if(errorMessages.Length>0)
              errorMessages.AppendLine (new string ('-', 10));
            errorMessages.AppendLine (message);
          }
        }
      }
      
      var messages = errorMessages.ToString ().Trim ();
      return string.IsNullOrEmpty (messages) ? MappingValidationResult.CreateValidResult() : MappingValidationResult.CreateInvalidResult(messages);
    }
  }
}