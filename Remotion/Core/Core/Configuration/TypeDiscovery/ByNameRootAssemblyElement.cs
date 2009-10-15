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
using System.Configuration;
using System.Reflection;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures a root assembly by name.
  /// </summary>
  public class ByNameRootAssemblyElement : ConfigurationElement
  {
    [ConfigurationProperty ("name", IsRequired = true, IsKey = true)]
    public string Name
    {
      get { return (string) this["name"]; }
      set { this["name"] = value; }
    }

    [ConfigurationProperty ("includeReferencedAssemblies", DefaultValue = "false", IsRequired = false)]
    public bool IncludeReferencedAssemblies
    {
      get { return (bool) this["includeReferencedAssemblies"]; }
      set { this["includeReferencedAssemblies"] = value; }
    }

    public NamedRootAssemblyFinder.Specification CreateSpecification ()
    {
      var assemblyName = new AssemblyName (Name);
      return new NamedRootAssemblyFinder.Specification (assemblyName, IncludeReferencedAssemblies);
    }
  }
}