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

namespace Remotion.Data.DomainObjects.Mapping
{
  public class ClassDefinitionValidator
  {
    private readonly ClassDefinition _classDefinition;

    public ClassDefinitionValidator (ClassDefinition classDefinition) 
    {
      _classDefinition = classDefinition;
    }

    public void ValidateCurrentMixinConfiguration ()
    {
      var currentMixinConfiguration = PersistentMixinFinder.GetMixinConfigurationForDomainObjectType (_classDefinition.ClassType);
      if (!Equals (currentMixinConfiguration, _classDefinition.PersistentMixinFinder.MixinConfiguration))
      {
          string message = string.Format (
              "The mixin configuration for domain object type '{0}' was changed after the mapping information was built." + Environment.NewLine
              + "Original configuration: {1}." + Environment.NewLine
              + "Active configuration: {2}", 
              _classDefinition.ClassType, 
              _classDefinition.PersistentMixinFinder.MixinConfiguration,
              currentMixinConfiguration);
          throw new MappingException (message);
        }
    }
  }
}
