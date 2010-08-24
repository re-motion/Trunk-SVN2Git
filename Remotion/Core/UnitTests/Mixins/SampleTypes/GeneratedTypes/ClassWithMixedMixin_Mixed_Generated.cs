using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;

namespace Remotion.UnitTests.Mixins.SampleTypes.GeneratedTypes
{
  [Serializable, ConcreteMixedType(new object[] { typeof(ClassWithMixedMixin), new object[] { new object[] { typeof(MixinMixingClass), 0, 0, new Type[] {  } } }, new Type[] {  } }, new Type[] { typeof(MixinMixingClass) }), DebuggerDisplay("Mix of Remotion.UnitTests.Mixins.SampleTypes.ClassWithMixedMixin + Remotion.UnitTests.Mixins.SampleTypes.MixinMixingClass")]
  [IgnoreForMixinConfiguration]
  public class ClassWithMixedMixin_Mixed_Generated_WithEfficientMixinCreation : ClassWithMixedMixin, IMixinTarget, IInitializableMixinTarget, ISerializable
  {
    // Fields
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static ClassContext __classContext;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object[] __extensions;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private BaseCallProxy __first;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static MixinArrayInitializer __mixinArrayInitializer;

    // Methods
    static ClassWithMixedMixin_Mixed_Generated_WithEfficientMixinCreation()
    {
      MixinContext[] mixins = new MixinContext[1];
      Type[] explicitDependencies = new Type[0];
      mixins[0] = new MixinContext(MixinKind.Extending, typeof(MixinMixingClass), MemberVisibility.Private, explicitDependencies);
      Type[] completeInterfaces = new Type[0];
      __classContext = new ClassContext(typeof(ClassWithMixedMixin), mixins, completeInterfaces);
      Type[] expectedMixinTypes = new Type[] { typeof(MixinMixingClass) };
      __mixinArrayInitializer = new MixinArrayInitializer(typeof(ClassWithMixedMixin), expectedMixinTypes);
    }

    public ClassWithMixedMixin_Mixed_Generated_WithEfficientMixinCreation()
    {
      if (this.__extensions == null)
      {
        ((IInitializableMixinTarget) this).Initialize();
      }
    }

    public ClassWithMixedMixin_Mixed_Generated_WithEfficientMixinCreation (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException("The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers.");
    }

    public string __base__StringMethod(int i)
    {
      return base.StringMethod(i);
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      SerializationHelper.GetObjectDataForGeneratedTypes(info, context, this, __classContext, this.__extensions, true);
    }

    void IInitializableMixinTarget.Initialize()
    {
      this.__first = new BaseCallProxy(this, 0);
      this.__extensions = new[] { new MixinMixingClass_Mixed_Generated_WithEfficientMixinCreation() };
      ((IInitializableMixin) this.__extensions[0]).Initialize(this, new BaseCallProxy(this, 1), false);
    }

    void IInitializableMixinTarget.InitializeAfterDeserialization(object[] mixinInstances)
    {
      this.__first = new BaseCallProxy(this, 0);
      __mixinArrayInitializer.CheckMixinArray(mixinInstances);
      this.__extensions = mixinInstances;
      ((IInitializableMixin) this.__extensions[0]).Initialize(this, new BaseCallProxy(this, 1), true);
    }

    public override string StringMethod(int i)
    {
      if (this.__extensions == null)
      {
        ((IInitializableMixinTarget) this).Initialize();
      }
      return this.__first.Remotion_UnitTests_Mixins_SampleTypes_ClassWithMixedMixin_StringMethod(i);
    }

    // Properties
    [DebuggerDisplay("Class context for ClassWithMixedMixin", Name="ClassContext")]
    ClassContext IMixinTarget.ClassContext
    {
      get
      {
        return __classContext;
      }
    }

    [DebuggerDisplay("Generated proxy", Name="FirstBaseCallProxy")]
    object IMixinTarget.FirstBaseCallProxy
    {
      get
      {
        if (this.__extensions == null)
        {
          ((IInitializableMixinTarget) this).Initialize();
        }
        return this.__first;
      }
    }

    [DebuggerDisplay("Count = {__extensions.Length}", Name="Mixins")]
    object[] IMixinTarget.Mixins
    {
      get
      {
        if (this.__extensions == null)
        {
          ((IInitializableMixinTarget) this).Initialize();
        }
        return this.__extensions;
      }
    }

    // Nested Types
    [Serializable]
    public sealed class BaseCallProxy : IGeneratedBaseCallProxyType, MixinMixingClass.IRequirements
    {
      // Fields
      public int __depth;
      public ClassWithMixedMixin_Mixed_Generated_WithEfficientMixinCreation __this;

      // Methods
      public BaseCallProxy(ClassWithMixedMixin_Mixed_Generated_WithEfficientMixinCreation _debefeafe1, int num1)
      {
        this.__this = _debefeafe1;
        this.__depth = num1;
      }

      public string Remotion_UnitTests_Mixins_SampleTypes_ClassWithMixedMixin_StringMethod(int i)
      {
        if (this.__depth == 0)
        {
          MixinMixingClass class2 = (MixinMixingClass) this.__this.__extensions[0];
          return class2.StringMethod(i);
        }
        return this.__this.__base__StringMethod(i);
      }

      string MixinMixingClass.IRequirements.StringMethod(int i)
      {
        if (this.__depth == 0)
        {
          MixinMixingClass class2 = (MixinMixingClass) this.__this.__extensions[0];
          return class2.StringMethod(i);
        }
        return this.__this.__base__StringMethod(i);
      }
    }
  }
}