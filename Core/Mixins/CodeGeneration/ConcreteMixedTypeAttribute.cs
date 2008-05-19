using System;
using System.Collections.Generic;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class ConcreteMixedTypeAttribute : Attribute
  {
    public static ConcreteMixedTypeAttribute FromClassContext (ClassContext context)
    {
      Type baseType = context.Type;
      List<MixinKind> mixinKinds = new List<MixinKind> (context.Mixins.Count);
      List<Type> mixinTypes = new List<Type> (context.Mixins.Count);
      List<Type> completeInterfaces = new List<Type> (context.CompleteInterfaces.Count);
      List<Type> explicitDependencyList = new List<Type> ();

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
      ClassContextBuilder contextBuilder = new ClassContextBuilder (new MixinConfigurationBuilder (null), TargetType, null);
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

    public TargetClassDefinition GetTargetClassDefinition ()
    {
      return TargetClassDefinitionCache.Current.GetTargetClassDefinition (GetClassContext ());
    }
  }

  // marker type used in ConcreteMixedTypeAttribute
  public class NextMixinDependency
  {
  }
}