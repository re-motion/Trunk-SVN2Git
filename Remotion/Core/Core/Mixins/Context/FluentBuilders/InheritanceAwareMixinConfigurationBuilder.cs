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
    private readonly MixinConfiguration _builtConfiguration;
    private readonly Dictionary<Type, ClassContext> _finishedContexts = new Dictionary<Type, ClassContext>();
    private readonly Dictionary<Type, ClassContextBuilder> _builders = new Dictionary<Type, ClassContextBuilder> ();

    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    // TODO: Change to not take a MixinConfiguration object
    public InheritanceAwareMixinConfigurationBuilder (MixinConfiguration builtConfiguration, IEnumerable<ClassContext> parentContexts, IEnumerable<ClassContextBuilder> classContextBuilders)
    {
      ArgumentUtility.CheckNotNull ("builtConfiguration", builtConfiguration);
      ArgumentUtility.CheckNotNull ("parentContexts", parentContexts);
      ArgumentUtility.CheckNotNull ("classContextBuilders", classContextBuilders);

      _builtConfiguration = builtConfiguration;

      foreach (ClassContextBuilder builder in classContextBuilders)
      {
        _builders.Add (builder.TargetType, builder);
      }

      foreach (ClassContext parentContext in parentContexts)
      {
        if (!_builders.ContainsKey (parentContext.Type))
          _finishedContexts.Add (parentContext.Type, parentContext);
      }

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
        List<ClassContext> inheritedContexts = new List<ClassContext>(1);
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
      foreach (var contextWithType in _finishedContexts)
      {
        if (_builders.ContainsKey (contextWithType.Key))
          _builtConfiguration.ClassContexts.AddOrReplace (contextWithType.Value);
      }
      return _builtConfiguration;
    }
  }
}
