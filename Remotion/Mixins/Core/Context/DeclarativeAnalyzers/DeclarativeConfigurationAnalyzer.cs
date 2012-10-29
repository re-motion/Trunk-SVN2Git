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
using Remotion.Collections;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class DeclarativeConfigurationAnalyzer
  {
    private readonly ExtendsAnalyzer _extendsAnalyzer;
    private readonly UsesAnalyzer _usesAnalyzer;
    private readonly HasCompleteInterfaceMarkerAnalyzer _hasCompleteInterfaceMarkerAnalyzer;
    private readonly MixAnalyzer _mixAnalyzer;
    private readonly IgnoresAnalyzer _ignoresAnalyzer;

    public DeclarativeConfigurationAnalyzer (ExtendsAnalyzer extendsAnalyzer, UsesAnalyzer usesAnalyzer, HasCompleteInterfaceMarkerAnalyzer interfaceMarkerAnalyzer, 
        MixAnalyzer mixAnalyzer, IgnoresAnalyzer ignoresAnalyzer)
    {
      ArgumentUtility.CheckNotNull ("extendsAnalyzer", extendsAnalyzer);
      ArgumentUtility.CheckNotNull ("usesAnalyzer", usesAnalyzer);
      ArgumentUtility.CheckNotNull ("interfaceMarkerAnalyzer", interfaceMarkerAnalyzer);
      ArgumentUtility.CheckNotNull ("mixAnalyzer", mixAnalyzer);
      ArgumentUtility.CheckNotNull ("ignoresAnalyzer", ignoresAnalyzer);

      _extendsAnalyzer = extendsAnalyzer;
      _usesAnalyzer = usesAnalyzer;
      _hasCompleteInterfaceMarkerAnalyzer = interfaceMarkerAnalyzer;
      _mixAnalyzer = mixAnalyzer;
      _ignoresAnalyzer = ignoresAnalyzer;
    }
    
    public void Analyze (IEnumerable<Type> types, MixinConfigurationBuilder configurationBuilder)
    {
      Set<Assembly> assemblies = new Set<Assembly>();

      foreach (Type type in types)
      {
        _extendsAnalyzer.Analyze (type, configurationBuilder);
        _usesAnalyzer.Analyze (type, configurationBuilder);
        _hasCompleteInterfaceMarkerAnalyzer.Analyze (type, configurationBuilder);
        _ignoresAnalyzer.Analyze (type, configurationBuilder);

        if (!assemblies.Contains (type.Assembly))
        {
          assemblies.Add (type.Assembly);
          _mixAnalyzer.Analyze (type.Assembly, configurationBuilder);
        }
      }
    }
  }
}
