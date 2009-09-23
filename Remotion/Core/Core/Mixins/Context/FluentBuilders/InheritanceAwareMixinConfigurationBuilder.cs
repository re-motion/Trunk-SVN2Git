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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  internal class InheritanceAwareMixinConfigurationBuilder
  {
    public static MixinConfiguration BuildMixinConfiguration (
        MixinConfiguration parentConfiguration, 
        IEnumerable<ClassContextBuilder> classContextBuilders)
    {
      ArgumentUtility.CheckNotNull ("classContextBuilders", classContextBuilders);

      var builders = classContextBuilders.ToDictionary (builder => builder.TargetType);

      var parentContexts = parentConfiguration != null ? (IEnumerable<ClassContext>) parentConfiguration.ClassContexts : new ClassContext[0];
      var initialContexts = parentContexts.Where (parentContext => !builders.ContainsKey (parentContext.Type));

      var result = new InheritanceAwareMixinConfigurationBuilder (builders, initialContexts);

      var builtConfiguration = new MixinConfiguration (parentConfiguration);
      
      var contextsOfBuilders = builders.Keys.Select (type => result.GetFinishedContext (type));
      foreach (var context in contextsOfBuilders)
        builtConfiguration.ClassContexts.AddOrReplace (context);

      return builtConfiguration;
    }

    private readonly Dictionary<Type, ClassContextBuilder> _builders;
    private readonly Dictionary<Type, ClassContext> _finishedContextStore;
    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    public InheritanceAwareMixinConfigurationBuilder (Dictionary<Type, ClassContextBuilder> builders, IEnumerable<ClassContext> initialContexts)
    {
      ArgumentUtility.CheckNotNull ("builders", builders);
      ArgumentUtility.CheckNotNull ("initialContexts", initialContexts);

      _builders = builders;
      _finishedContextStore = initialContexts.ToDictionary (c => c.Type);
      _inheritanceAlgorithm = new InheritedClassContextRetrievalAlgorithm (GetFinishedContextFromStore, GetFinishedContext);
    }

    public ClassContext GetFinishedContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext finishedContext = GetFinishedContextFromStore (type);
      if (finishedContext != null)
        return finishedContext;

      ClassContext inheritedContext = _inheritanceAlgorithm.GetWithInheritance (type);
      if (_builders.ContainsKey (type))
      {
        var inheritedContexts = new List<ClassContext> (1);
        if (inheritedContext != null)
          inheritedContexts.Add (inheritedContext);
        finishedContext = _builders[type].BuildClassContext (inheritedContexts);
      }
      else
        finishedContext = inheritedContext ?? new ClassContext (type);

      _finishedContextStore.Add (type, finishedContext);
      return finishedContext;
    }

    private ClassContext GetFinishedContextFromStore (Type type)
    {
      ClassContext finishedContext;
      _finishedContextStore.TryGetValue (type, out finishedContext);
      return finishedContext;
    }
  }
}