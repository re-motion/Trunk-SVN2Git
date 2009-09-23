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
using Remotion.Collections;
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
  ///   <item>Then, the corresponding <see cref="ClassContext"/> from the parent configuration is retrieved.</item>
  ///   <item>Then, a new <see cref="ClassContext"/> is created from the <see cref="ClassContextBuilder"/>; inheriting everything from the base
  ///   and parent contexts.</item>
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

      var parentContexts = parentConfiguration != null ? parentConfiguration.ClassContexts : new ClassContextCollection();
      var buildersWithParentContexts = classContextBuilders.ToDictionary (
          classContextBuilder => classContextBuilder.TargetType, 
          classContextBuilder => Tuple.NewTuple (classContextBuilder, parentContexts.GetExact (classContextBuilder.TargetType)));

      var unchangedContextsFromParent = parentContexts.Where (parentContext => !buildersWithParentContexts.ContainsKey (parentContext.Type));

      var mixinConfigurationBuilder = new InheritanceAwareMixinConfigurationBuilder (buildersWithParentContexts, unchangedContextsFromParent);

      var result = new MixinConfiguration (parentConfiguration);
      
      var finishedContextsOfBuilders = buildersWithParentContexts.Keys.Select (type => mixinConfigurationBuilder.GetFinishedContext (type));
      foreach (var context in finishedContextsOfBuilders)
        result.ClassContexts.AddOrReplace (context);

      return result;
    }

    private readonly Dictionary<Type, Tuple<ClassContextBuilder, ClassContext>> _buildersWithParentContexts;
    private readonly Dictionary<Type, ClassContext> _finishedContextCache;
    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    public InheritanceAwareMixinConfigurationBuilder (
        Dictionary<Type, Tuple<ClassContextBuilder, ClassContext>> buildersWithParentContexts, 
        IEnumerable<ClassContext> initialContexts)
    {
      ArgumentUtility.CheckNotNull ("buildersWithParentContexts", buildersWithParentContexts);
      ArgumentUtility.CheckNotNull ("initialContexts", initialContexts);

      _buildersWithParentContexts = buildersWithParentContexts;
      _finishedContextCache = initialContexts.ToDictionary (c => c.Type);
      _inheritanceAlgorithm = new InheritedClassContextRetrievalAlgorithm (GetFinishedContextFromCache, GetFinishedContext);
    }

    public ClassContext GetFinishedContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      // First probe the store...
      var cachedContext = GetFinishedContextFromCache (type);
      if (cachedContext != null)
        return cachedContext;

      // If we have nothing in the store, get the combined context from the base classes, get the context from the parent configuration, then 
      // derive the new context from those two.
      // This is recursive, the _inheritanceAlgorithm will call this method for the different base classes (base, generic type definition, interfaces)
      // it combines.
      // For cases where we have no builder, return the context from the base class, or an empty one if none exists.
      
      ClassContext contextInheritedFromBaseClasses = _inheritanceAlgorithm.GetWithInheritance (type);

      Tuple<ClassContextBuilder, ClassContext> builderWithParentContext;
      ClassContext builtContext;
      if (_buildersWithParentContexts.TryGetValue (type, out builderWithParentContext))
        builtContext = CreateContextWithBuilder (builderWithParentContext.A, builderWithParentContext.B, contextInheritedFromBaseClasses);
      else
        builtContext = CreateContextWithoutBuilder (type, contextInheritedFromBaseClasses);

      _finishedContextCache.Add (type, builtContext);
      return builtContext;
    }

    private ClassContext CreateContextWithBuilder (
        ClassContextBuilder builder, 
        ClassContext contextFromParentConfiguration, 
        ClassContext contextFromBaseClasses)
    {
      var inheritedContexts = new[] { contextFromParentConfiguration, contextFromBaseClasses }.Where (c => c != null);
      var builtContext = builder.BuildClassContext (inheritedContexts);
      return builtContext;
    }

    private ClassContext CreateContextWithoutBuilder (Type type, ClassContext contextInheritedFromBaseClasses)
    {
      var builtContext = contextInheritedFromBaseClasses ?? new ClassContext (type);
      Assertion.IsTrue (builtContext.Type == type, "Guaranteed by ClassContextCombiner");
      return builtContext;
    }

    private ClassContext GetFinishedContextFromCache (Type type)
    {
      ClassContext finishedContext;
      _finishedContextCache.TryGetValue (type, out finishedContext);
      return finishedContext;
    }
  }
}