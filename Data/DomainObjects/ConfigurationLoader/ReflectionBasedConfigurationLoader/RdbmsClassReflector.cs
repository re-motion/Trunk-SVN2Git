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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="RdbmsClassReflector"/> is used introduce <b>RDBMS</b> specific information into the building the 
  /// <see cref="ReflectionBasedClassDefinition"/> and the <see cref="RelationDefinition"/> objects.
  /// </summary>
  public class RdbmsClassReflector : ClassReflector
  {
    public RdbmsClassReflector (Type type)
        : base (type)
    {
    }

    protected override string GetStorageSpecificIdentifier()
    {
      if (IsTable())
        return base.GetStorageSpecificIdentifier();
      return null;
    }

    private bool IsTable()
    {
      return Attribute.IsDefined (Type, typeof (DBTableAttribute), false);
    }
  }
}
