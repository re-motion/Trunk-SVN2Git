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
  /// <summary>
  /// Builds a <see cref="MixinConfiguration"/> from a parent configuration and a list of <see cref="ClassContextBuilder"/> objects.
  /// This is done as follows:
  /// <list type="bullet">
  /// <item>Each <see cref="ClassContext"/> that exists in the parent configuration is kept as is, unless there is a 
  /// <see cref="ClassContextBuilder"/> for the same type.</item>
  /// <item>Each <see cref="ClassContextBuilder"/> is transformed into a new <see cref="ClassContext"/>:</item>
  ///   <list type="bullet">
  ///   <item>First, the <see cref="ClassContext"/> objects for its base classes and interfaces are retrieved or created.</item> 
  ///   <item>Then, a new <see cref="ClassContext"/> is created from the <see cref="ClassContextBuilder"/>; inheriting everything from the base
  ///   contexts.</item>
  ///   <item>The <see cref="ClassContext"/> for the class from the parent configuration is ignored by this class. However, when a new 
  ///   <see cref="ClassContextBuilder"/> is created, that parent configuration is copied; so effectively, the <see cref="ClassContext"/> does inherit
  ///   from its parent context.</item>
  ///   </list>
  /// </list>
  /// </summary>
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
    private readonly Dictionary<Type, ClassContext> _finishedContextCache;
    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    public InheritanceAwareMixinConfigurationBuilder (Dictionary<Type, ClassContextBuilder> builders, IEnumerable<ClassContext> initialContexts)
    {
      ArgumentUtility.CheckNotNull ("builders", builders);
      ArgumentUtility.CheckNotNull ("initialContexts", initialContexts);

      _builders = builders;
      _finishedContextCache = initialContexts.ToDictionary (c => c.Type);
      _inheritanceAlgorithm = new InheritedClassContextRetrievalAlgorithm (GetFinishedContextFromCache, GetFinishedContext);
    }

    public ClassContext GetFinishedContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      // First probe the store...
      ClassContext finishedContext = GetFinishedContextFromCache (type);
      if (finishedContext != null)
        return finishedContext;

      // If we have nothing in the store, get the combined context from the base classes, then derive the new context from it.
      // This is recursive, the _inheritanceAlgorithm will call this method for the different base classes (base, generic type definition, interfaces)
      // it combines.
      ClassContext contextInheritedFromBaseClasses = _inheritanceAlgorithm.GetWithInheritance (type);
      if (_builders.ContainsKey (type))
      {
        IEnumerable<ClassContext> inheritedContexts =
            contextInheritedFromBaseClasses != null ? new[] { contextInheritedFromBaseClasses } : new ClassContext[0];
        finishedContext = _builders[type].BuildClassContext (inheritedContexts);
      }
      else
      {
        finishedContext = contextInheritedFromBaseClasses ?? new ClassContext (type);
      }

      _finishedContextCache.Add (type, finishedContext);
      return finishedContext;
    }

    private ClassContext GetFinishedContextFromCache (Type type)
    {
      ClassContext finishedContext;
      _finishedContextCache.TryGetValue (type, out finishedContext);
      return finishedContext;
    }
  }
}