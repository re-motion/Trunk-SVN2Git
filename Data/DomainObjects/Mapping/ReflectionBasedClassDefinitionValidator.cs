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
      Set<Type> currentMixins = new Set<Type> (new PersistentMixinFinder (_classDefinition.ClassType).GetPersistentMixins ());
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
  }
}