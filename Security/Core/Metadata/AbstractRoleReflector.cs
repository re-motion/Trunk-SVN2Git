// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class AbstractRoleReflector : IAbstractRoleReflector
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
