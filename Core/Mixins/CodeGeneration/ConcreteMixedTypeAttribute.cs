using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class ConcreteMixedTypeAttribute : Attribute
  {
    internal class GenericAssignArrayStatement : Statement
    {
      private readonly Type _elementType;
      private readonly Reference _arrayReference;
      private readonly int _elementIndex;
      private readonly Expression _elementValue;

      public GenericAssignArrayStatement (Type elementType, Reference arrayReference, int elementIndex, Expression elementValue)
      {
        _elementType = elementType;
        _arrayReference = arrayReference;
        _elementIndex = elementIndex;
        _elementValue = elementValue;
      }

      public override void Emit (Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, ILGenerator il)
      {
        ArgumentsUtil.EmitLoadOwnerAndReference (_arrayReference, il);
        il.Emit (OpCodes.Ldc_I4, _elementIndex);
        _elementValue.Emit (member, il);
        il.Emit (OpCodes.Stelem, _elementType);
      }
    }

    private static readonly ConstructorInfo s_attributeCtor = typeof (ConcreteMixedTypeAttribute).GetConstructor (
        new Type[] {typeof (Type), typeof (MixinKind[]), typeof (Type[]), typeof (Type[]), typeof (Type[])});

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

    public static CustomAttributeBuilder BuilderFromClassContext (ClassContext context)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixedTypeAttribute attribute = FromClassContext (context);
      CustomAttributeBuilder builder = new CustomAttributeBuilder (s_attributeCtor,
          new object[] { attribute.TargetType, attribute.MixinKinds, attribute.MixinTypes, attribute.CompleteInterfaces, attribute.ExplicitDependenciesPerMixin });
      return builder;
    }

    public static Expression NewAttributeExpressionFromClassContext (ClassContext context, AbstractCodeBuilder codeBuilder)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixedTypeAttribute attribute = FromClassContext (context);

      Dictionary<MixinKind, LocalReference> mixinKinds = new Dictionary<MixinKind, LocalReference> ();
      foreach (MixinKind value in Enum.GetValues (typeof (MixinKind)))
      {
        LocalReference valueReference = codeBuilder.DeclareLocal (typeof (MixinKind));
        codeBuilder.AddStatement (new AssignStatement (valueReference, new ConstReference ((int) value).ToExpression()));
        mixinKinds.Add (value, valueReference);
      }

      LocalReference mixinKindsArray = CreateArrayLocal (codeBuilder, attribute.MixinKinds, delegate (MixinKind k) { return mixinKinds[k].ToExpression(); });
      LocalReference mixinTypesArray = CreateArrayLocal (codeBuilder, attribute.MixinTypes, delegate (Type t) { return new TypeTokenExpression (t); });
      LocalReference completeInterfacesArray = CreateArrayLocal (codeBuilder, attribute.CompleteInterfaces, delegate (Type t) { return new TypeTokenExpression (t); });
      LocalReference explicitDependenciesPerMixinArray = CreateArrayLocal (codeBuilder, attribute.ExplicitDependenciesPerMixin, delegate (Type t) { return new TypeTokenExpression (t); });

      return new NewInstanceExpression (s_attributeCtor,
          new TypeTokenExpression (attribute.TargetType),
          mixinKindsArray.ToExpression(),
          mixinTypesArray.ToExpression (),
          completeInterfacesArray.ToExpression (),
          explicitDependenciesPerMixinArray.ToExpression ());
    }

    private static LocalReference CreateArrayLocal<T> (AbstractCodeBuilder codeBuilder, T[] types, Func<T, Expression> elementExpressor)
    {
      LocalReference arrayLocal = codeBuilder.DeclareLocal (typeof (T[]));
      codeBuilder.AddStatement (new AssignStatement (arrayLocal, new NewArrayExpression (types.Length, typeof (T))));
      for (int i = 0; i < types.Length; ++i)
        codeBuilder.AddStatement (new GenericAssignArrayStatement (typeof (T), arrayLocal, i, elementExpressor(types[i])));
      return arrayLocal;
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