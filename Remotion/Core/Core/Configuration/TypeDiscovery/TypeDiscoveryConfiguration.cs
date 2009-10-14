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
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures the type discovery performed by <see cref="ContextAwareTypeDiscoveryUtility"/>.
  /// </summary>
  public sealed class TypeDiscoveryConfiguration : ConfigurationSection
  {
    public TypeDiscoveryConfiguration ()
    {
      var xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      Properties.Add (xmlnsProperty);
    }

    [ConfigurationProperty ("customRootAssemblyFinder")]
    public TypeElement<IRootAssemblyFinder> CustomRootAssemblyFinder
    {
      get { return (TypeElement<IRootAssemblyFinder>) this["customRootAssemblyFinder"]; }
    }

    [ConfigurationProperty ("specificRootAssemblies")]
    public RootAssembliesElement SpecificRootAssemblies
    {
      get { return (RootAssembliesElement) this["specificRootAssemblies"]; }
    }

    protected override void PostDeserialize ()
    {
      base.PostDeserialize ();
      if (CustomRootAssemblyFinder.Type != null && (SpecificRootAssemblies.ByFile.Count != 0 || SpecificRootAssemblies.ByName.Count != 0))
      {
        var message = "Custom root assembly finder and specific root assemblies cannot both be specified.";
        throw new ConfigurationErrorsException (message);
      }
    }
  }
}