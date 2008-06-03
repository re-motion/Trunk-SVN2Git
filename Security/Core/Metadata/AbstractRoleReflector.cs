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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class AbstractRoleReflector : Remotion.Security.Metadata.IAbstractRoleReflector
  {
    // types

    // static members

    // member fields
    private IEnumerationReflector _enumerationReflector;

    // construction and disposing

    public AbstractRoleReflector ()
      : this (new EnumerationReflector ())
    {
    }

    public AbstractRoleReflector (IEnumerationReflector enumerationReflector)
    {
      ArgumentUtility.CheckNotNull ("enumerationReflector", enumerationReflector);
      _enumerationReflector = enumerationReflector;
    }

    // methods and properties

    public IEnumerationReflector EnumerationTypeReflector
    {
      get { return _enumerationReflector; }
    }

    public List<EnumValueInfo> GetAbstractRoles (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);

      List<EnumValueInfo> abstractRoles = new List<EnumValueInfo> ();
      foreach (Type type in assembly.GetTypes ())
      {
        if (type.IsEnum && Attribute.IsDefined (type, typeof (AbstractRoleAttribute), false))
        {
          Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (type, cache);
          foreach (KeyValuePair<Enum, EnumValueInfo> entry in values)
          {
            if (!cache.ContainsAbstractRole (entry.Key))
              cache.AddAbstractRole (entry.Key, entry.Value);
            abstractRoles.Add (entry.Value);
          }
        }
      }

      return abstractRoles;
    }
  }
}
