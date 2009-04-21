// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Collections;
using Remotion.Text;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class ReflectionBasedClassDefinitionValidator : ClassDefinitionValidator
  {
    private readonly ReflectionBasedClassDefinition _classDefinition;

    public ReflectionBasedClassDefinitionValidator (ReflectionBasedClassDefinition classDefinition)
        : base(classDefinition)
    {
      _classDefinition = classDefinition;
    }

    public override void ValidateCurrentMixinConfiguration ()
    {
      base.ValidateCurrentMixinConfiguration ();
      Set<Type> currentMixins = new Set<Type> (CreateNewPersistentMixinFinder().GetPersistentMixins ());
      foreach (Type t in _classDefinition.PersistentMixins)
      {
        if (!currentMixins.Contains (t))
        {
          string message = string.Format ("A persistence-related mixin was removed from the domain object type {0} after the mapping "
            + "information was built: {1}.", _classDefinition.ClassType.FullName, t.FullName);
          throw new MappingException (message);
        }
        currentMixins.Remove (t);
      }
      if (currentMixins.Count > 0)
      {
        string message = string.Format ("One or more persistence-related mixins were added to the domain object type {0} after the mapping "
            + "information was built: {1}.", _classDefinition.ClassType.FullName, SeparatedStringBuilder.Build (", ", currentMixins));
        throw new MappingException (message);
      }
    }

    public PersistentMixinFinder CreateNewPersistentMixinFinder ()
    {
      return new PersistentMixinFinder (_classDefinition.ClassType, _classDefinition.GetInheritanceRootClass() == _classDefinition);
    }
  }
}
