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

    public InheritanceAwareMixinConfigurationBuilder (
        Dictionary<Type, Tuple<ClassContextBuilder, ClassContext>> buildersWithParentContexts, 
        IEnumerable<ClassContext> initialContexts)
    {
      ArgumentUtility.CheckNotNull ("buildersWithParentContexts", buildersWithParentContexts);
      ArgumentUtility.CheckNotNull ("initialContexts", initialContexts);

      _buildersWithParentContexts = buildersWithParentContexts;
      _finishedContextCache = initialContexts.ToDictionary (c => c.Type);
    }

    public ClassContext GetFinishedContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      // First probe the store...
      var cachedContext = GetFinishedContextFromCache (type);
      if (cachedContext != null)
        return cachedContext;

      // If we have nothing in the caching, get the contexts of the base classes we need to derive our mixins from, then create a new context.
      ClassContext builtContext = CreateContext (type, InheritedClassContextRetrievalAlgorithm.GetTypesToInheritFrom (type));
      _finishedContextCache.Add (type, builtContext);
      return builtContext;
    }

    private ClassContext GetFinishedContextFromCache (Type type)
    {
      ClassContext finishedContext;
      _finishedContextCache.TryGetValue (type, out finishedContext);
      return finishedContext;
    }

    private ClassContext CreateContext (Type type, IEnumerable<Type> baseTypesToInheritFrom)
    {
      var inheritedContextCombiner = new ClassContextCombiner ();
      foreach (var baseTypes in baseTypesToInheritFrom)
      {
        var inheritedContext = GetFinishedContext (baseTypes); // recursion!
        inheritedContextCombiner.AddIfNotNull (inheritedContext);
      }

      Tuple<ClassContextBuilder, ClassContext> builderWithParentContext;
      if (_buildersWithParentContexts.TryGetValue (type, out builderWithParentContext))
      {
        inheritedContextCombiner.AddIfNotNull (builderWithParentContext.B);
        return CreateContextWithBuilder (builderWithParentContext.A, inheritedContextCombiner.GetCombinedContexts (type));
      }
      else
      {
        return CreateContextWithoutBuilder (type, inheritedContextCombiner.GetCombinedContexts (type));
      }
    }

    private ClassContext CreateContextWithBuilder (ClassContextBuilder builder, ClassContext inheritedContext)
    {
      var inheritedContexts = inheritedContext != null ? new[] { inheritedContext } : new ClassContext[0];
      var builtContext = builder.BuildClassContext (inheritedContexts);
      return builtContext;
    }

    private ClassContext CreateContextWithoutBuilder (Type type, ClassContext inheritedContext)
    {
      var builtContext = inheritedContext ?? new ClassContext (type);
      Assertion.IsTrue (builtContext.Type == type, "Guaranteed by ClassContextCombiner");
      return builtContext;
    }
  }
}