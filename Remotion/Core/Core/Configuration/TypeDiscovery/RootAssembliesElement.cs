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
using System.Collections.Generic;
using System.Configuration;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures the root assemblies to be used to discover types.
  /// </summary>
  public sealed class RootAssembliesElement : ConfigurationElement
  {
    public RootAssembliesElement ()
    {
      var byNameProperty = new ConfigurationProperty (
          "byName", 
          typeof (ByNameRootAssemblyElementCollection), 
          new ByNameRootAssemblyElementCollection (), 
          ConfigurationPropertyOptions.None);
      Properties.Add (byNameProperty);

      var byFileProperty = new ConfigurationProperty (
          "byFile", 
          typeof (ByFileRootAssemblyElementCollection), 
          new ByFileRootAssemblyElementCollection (), 
          ConfigurationPropertyOptions.None);
      Properties.Add (byFileProperty);
    }

    public ByNameRootAssemblyElementCollection ByName
    {
      get { return (ByNameRootAssemblyElementCollection) this["byName"]; }
    }
    
    public ByFileRootAssemblyElementCollection ByFile
    {
      get { return (ByFileRootAssemblyElementCollection) this["byFile"]; }
    }

    public CompositeRootAssemblyFinder CreateRootAssemblyFinder ()
    {
      var namedFinder = ByName.CreateRootAssemblyFinder ();
      var filePatternFinder = ByFile.CreateRootAssemblyFinder ();

      return new CompositeRootAssemblyFinder (new IRootAssemblyFinder[] { namedFinder, filePatternFinder });
    }
  }
}