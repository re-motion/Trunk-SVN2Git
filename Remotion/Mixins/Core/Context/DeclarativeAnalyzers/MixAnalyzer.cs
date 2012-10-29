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
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  // TODO 5141: Test ignoring of duplicates (unit test and integration test)
  // TODO 5141: How can we generalize this, when we change to finding attributes based on the interface type?
  // Ideas: Remove this feature (don't ignore duplicates), generalize this feature (ignore duplicates for all), have the attribute define whether duplicates can be ignored.
  public class MixAnalyzer : IMixinDeclarationAnalyzer<Assembly>
  {
    private readonly MixinConfigurationAttributeAnalyzer<Assembly> _innerAnalyzer;
    private readonly HashSet<MixAttribute> _analyzedAttributes = new HashSet<MixAttribute> ();

    public MixAnalyzer ()
    {
      _innerAnalyzer = new MixinConfigurationAttributeAnalyzer<Assembly> (GetUniqueAttributes);
    }

    public void Analyze (Assembly entity, MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("entity", entity);
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);

      _innerAnalyzer.Analyze (entity, configurationBuilder);
    }

    private IEnumerable<IMixinConfigurationAttribute<Assembly>> GetUniqueAttributes (Assembly assembly)
    {
      return assembly
          .GetCustomAttributes (typeof (MixAttribute), false)
          .Cast<MixAttribute>()
          .Where (a => !_analyzedAttributes.Contains (a))
          .ApplySideEffect (a => _analyzedAttributes.Add (a))
          .Cast<IMixinConfigurationAttribute<Assembly>>();
    }
  }
}
