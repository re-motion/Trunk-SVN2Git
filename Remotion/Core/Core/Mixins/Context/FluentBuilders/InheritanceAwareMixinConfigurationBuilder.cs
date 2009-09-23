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
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context.FluentBuilders
{
  internal class InheritanceAwareMixinConfigurationBuilder
  {
    private readonly MixinConfiguration _parentConfiguration;
    private readonly Dictionary<Type, ClassContext> _finishedContexts;
    private readonly Dictionary<Type, ClassContextBuilder> _builders;

    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    public InheritanceAwareMixinConfigurationBuilder (MixinConfiguration parentConfiguration, IEnumerable<ClassContextBuilder> classContextBuilders)
    {
      ArgumentUtility.CheckNotNull ("classContextBuilders", classContextBuilders);

      _parentConfiguration = parentConfiguration;
      _builders = classContextBuilders.ToDictionary (builder => builder.TargetType);

      var parentContexts = parentConfiguration != null ? (IEnumerable<ClassContext>) _parentConfiguration.ClassContexts : new ClassContext[0];
      _finishedContexts = parentContexts.Where (parentContext => !_builders.ContainsKey (parentContext.Type)).ToDictionary (c => c.Type);

      _inheritanceAlgorithm = new InheritedClassContextRetrievalAlgorithm (GetFinishedContextNonRecursive, GetFinishedContext);
      ProcessBuilders();
    }

    private void ProcessBuilders ()
    {
      foreach (ClassContextBuilder builder in _builders.Values)
        GetFinishedContext (builder.TargetType);
    }

    private ClassContext GetFinishedContextNonRecursive (Type type)
    {
      ClassContext finishedContext;
      _finishedContexts.TryGetValue (type, out finishedContext);
      return finishedContext;
    }

    private ClassContext GetFinishedContext (Type type)
    {
      ClassContext finishedContext = GetFinishedContextNonRecursive (type);
      if (finishedContext != null)
        return finishedContext;

      ClassContext inheritedContext = _inheritanceAlgorithm.GetWithInheritance (type);
      if (_builders.ContainsKey (type))
      {
        var inheritedContexts = new List<ClassContext>(1);
        if (inheritedContext != null)
          inheritedContexts.Add (inheritedContext);
        finishedContext = _builders[type].BuildClassContext (inheritedContexts);
      }
      else
        finishedContext = inheritedContext ?? new ClassContext (type);

      _finishedContexts.Add (type, finishedContext);
      return finishedContext;
    }

    public MixinConfiguration BuildMixinConfiguration ()
    {
      var builtConfiguration = new MixinConfiguration (_parentConfiguration);
      var contextsOfBuilders = _builders.Keys.Select (type => _finishedContexts[type]);
      foreach (var context in contextsOfBuilders)
      {
        builtConfiguration.ClassContexts.AddOrReplace (context);
      }

      return builtConfiguration;
    }
  }
}
