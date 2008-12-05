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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class AssemblyReflector
  {
    // types

    // static members

    // member fields

    private IAccessTypeReflector _accessTypeReflector;
    private IClassReflector _classReflector;
    private IAbstractRoleReflector _abstractRoleReflector;
    
    // construction and disposing

    public AssemblyReflector () : this (new AccessTypeReflector(), new ClassReflector (), new AbstractRoleReflector ())
    {
    }

    public AssemblyReflector (IAccessTypeReflector accessTypeReflector, IClassReflector classReflector, IAbstractRoleReflector abstractRoleReflector)
    {
      ArgumentUtility.CheckNotNull ("accessTypeReflector", accessTypeReflector);
      ArgumentUtility.CheckNotNull ("classReflector", classReflector);
      ArgumentUtility.CheckNotNull ("abstractRoleReflector", abstractRoleReflector);

      _accessTypeReflector = accessTypeReflector;
      _classReflector = classReflector;
      _abstractRoleReflector = abstractRoleReflector;
    }

    // methods and properties

    public IAccessTypeReflector AccessTypeReflector
    {
      get { return _accessTypeReflector; }
    }

    public IClassReflector ClassReflector
    {
      get { return _classReflector; }
    }

    public IAbstractRoleReflector AbstractRoleReflector
    {
      get { return _abstractRoleReflector; }
    }

    public void GetMetadata (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);
      
      Assembly securityAssembly = GetType().Assembly;
      _accessTypeReflector.GetAccessTypesFromAssembly (securityAssembly, cache);
      _accessTypeReflector.GetAccessTypesFromAssembly (assembly, cache);

      _abstractRoleReflector.GetAbstractRoles (securityAssembly, cache);
      _abstractRoleReflector.GetAbstractRoles (assembly, cache);

      foreach (Type type in assembly.GetTypes ())
      {
        if (typeof (ISecurableObject).IsAssignableFrom (type))
          _classReflector.GetMetadata (type, cache);
      }

    }
  }
}
