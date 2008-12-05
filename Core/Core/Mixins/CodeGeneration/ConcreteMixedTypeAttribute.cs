// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class ConcreteMixedTypeAttribute : Attribute
  {
    public static ConcreteMixedTypeAttribute FromClassContext (ClassContext context)
    {
      Type baseType = context.Type;
      var mixinKinds = new List<MixinKind> (context.Mixins.Count);
      var mixinTypes = new List<Type> (context.Mixins.Count);
      var completeInterfaces = new List<Type> (context.CompleteInterfaces.Count);
      var explicitDependencyList = new List<Type> ();

      completeInterfaces.AddRange (context.CompleteInterfaces);

      foreach (MixinContext mixin in context.Mixins)
      {
        mixinTypes.Add (mixin.MixinType);
        if (mixin.ExplicitDependencies.Count > 0)
        {
          if (explicitDependencyList.Count != 0)
            explicitDependencyList.Add (typeof (NextMixinDependency));

          explicitDependencyList.Add (mixin.MixinType);
          explicitDependencyList.AddRange (mixin.ExplicitDependencies);
        }
        mixinKinds.Add (mixin.MixinKind);
      }

      return new ConcreteMixedTypeAttribute (baseType, mixinKinds.ToArray(), mixinTypes.ToArray(), completeInterfaces.ToArray(), explicitDependencyList.ToArray ());
    }

    private readonly Type _targetType;
    private readonly MixinKind[] _mixinKinds;
    private readonly Type[] _mixinTypes;
    private readonly Type[] _completeInterfaces;
    private readonly Type[] _explicitDependenciesPerMixin;

    public ConcreteMixedTypeAttribute (Type baseType, MixinKind[] mixinKindsPerMixin, Type[] mixinTypes, Type[] completeInterfaces, Type[] explicitDependenciesPerMixin)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("mixinKindsPerMixin", mixinKindsPerMixin);
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      ArgumentUtility.CheckNotNull ("completeInterfaces", completeInterfaces);
      ArgumentUtility.CheckNotNull ("explicitDependenciesPerMixin", explicitDependenciesPerMixin);

      _targetType = baseType;
      _mixinKinds = mixinKindsPerMixin;
      _mixinTypes = mixinTypes;
      _completeInterfaces = completeInterfaces;
      _explicitDependenciesPerMixin = explicitDependenciesPerMixin;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type[] MixinTypes
    {
      get { return _mixinTypes; }
    }

    public Type[] CompleteInterfaces
    {
      get { return _completeInterfaces; }
    }

    public Type[] ExplicitDependenciesPerMixin
    {
      get { return _explicitDependenciesPerMixin; }
    }

    public MixinKind[] MixinKinds
    {
      get { return _mixinKinds; }
    }

    public ClassContext GetClassContext ()
    {
      var contextBuilder = new ClassContextBuilder (new MixinConfigurationBuilder (null), TargetType, null);
      for (int i = 0; i < MixinTypes.Length; ++i)
        contextBuilder.AddMixin (MixinTypes[i]).OfKind (MixinKinds[i]);
      
      foreach (Type completeInterface in CompleteInterfaces)
        contextBuilder.AddCompleteInterface (completeInterface);

      Type currentMixinType = null;
      foreach (Type type in ExplicitDependenciesPerMixin)
      {
        if (type == typeof (NextMixinDependency))
          currentMixinType = null;
        else if (currentMixinType == null)
          currentMixinType = type;
        else
          contextBuilder.EnsureMixin (currentMixinType).WithDependency (type);
      }

      ClassContext context = contextBuilder.BuildClassContext (new ClassContext[0]);
      Assertion.IsTrue (context.Type == TargetType);
      return context;
    }

    public virtual TargetClassDefinition GetTargetClassDefinition (ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      return targetClassDefinitionCache.GetTargetClassDefinition (GetClassContext ());
    }
  }

  // marker type used in ConcreteMixedTypeAttribute
  public class NextMixinDependency
  {
  }
}
