// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class DeclarativeConfigurationAnalyzer
  {
    private readonly ICollection<IMixinDeclarationAnalyzer<Type>> _typeAnalyzers;
    private readonly ICollection<IMixinDeclarationAnalyzer<Assembly>> _assemblyAnalyzers;

    public DeclarativeConfigurationAnalyzer (
        IEnumerable<IMixinDeclarationAnalyzer<Type>> typeAnalyzers, 
        IEnumerable<IMixinDeclarationAnalyzer<Assembly>> assemblyAnalyzers)
    {
      ArgumentUtility.CheckNotNull ("typeAnalyzers", typeAnalyzers);
      ArgumentUtility.CheckNotNull ("assemblyAnalyzers", assemblyAnalyzers);

      _typeAnalyzers = typeAnalyzers.ConvertToCollection ();
      _assemblyAnalyzers = assemblyAnalyzers.ConvertToCollection ();
    }
    
    public void Analyze (IEnumerable<Type> types, MixinConfigurationBuilder configurationBuilder)
    {
      var assemblies = new HashSet<Assembly>();

      foreach (var type in types)
      {
        foreach (var typeAnalyzer in _typeAnalyzers)
          typeAnalyzer.Analyze (type, configurationBuilder);

        if (!assemblies.Contains (type.Assembly))
        {
          assemblies.Add (type.Assembly);

          foreach (var assemblyAnalyzer in _assemblyAnalyzers)
            assemblyAnalyzer.Analyze (type.Assembly, configurationBuilder);
        }
      }
    }
  }
}
