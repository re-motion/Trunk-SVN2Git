/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  internal class InheritanceAwareMixinConfigurationBuilder
  {
    private readonly MixinConfiguration _builtConfiguration;
    private readonly Dictionary<Type, ClassContext> _finishedContexts = new Dictionary<Type, ClassContext>();
    private readonly List<ClassContext> _finishedContextsForBuilders = new List<ClassContext> ();
    private readonly Dictionary<Type, ClassContextBuilder> _builders = new Dictionary<Type, ClassContextBuilder> ();

    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    // TODO: Change to not take a MixinConfiguration object
    public InheritanceAwareMixinConfigurationBuilder (MixinConfiguration builtConfiguration, IEnumerable<ClassContext> parentContexts, IEnumerable<ClassContextBuilder> classContextBuilders)
    {
      ArgumentUtility.CheckNotNull ("builtConfiguration", builtConfiguration);
      ArgumentUtility.CheckNotNull ("parentContexts", parentContexts);
      ArgumentUtility.CheckNotNull ("classContextBuilders", classContextBuilders);

      _builtConfiguration = builtConfiguration;

      foreach (ClassContext parentContext in parentContexts)
        _finishedContexts.Add (parentContext.Type, parentContext);

      foreach (ClassContextBuilder builder in classContextBuilders)
      {
        _finishedContexts.Remove (builder.TargetType);
        _builders.Add (builder.TargetType, builder);
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
        _finishedContextsForBuilders.Add (finishedContext);
      }
      else
        finishedContext = inheritedContext ?? new ClassContext (type);

      _finishedContexts.Add (type, finishedContext);
      return finishedContext;
    }

    public MixinConfiguration BuildMixinConfiguration ()
    {
      foreach (ClassContext context in _finishedContextsForBuilders)
        _builtConfiguration.ClassContexts.AddOrReplace (context);
      return _builtConfiguration;
    }
  }
}
