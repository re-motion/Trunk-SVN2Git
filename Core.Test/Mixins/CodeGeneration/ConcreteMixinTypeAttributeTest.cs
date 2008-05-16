using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeAttributeTest
  {
    [ConcreteMixinType (3, typeof (ConcreteMixinTypeAttributeTest),
        new MixinKind[] {MixinKind.Extending, MixinKind.Used, MixinKind.Extending}, 
        new Type[] {typeof (string), typeof (object), typeof (int)},
        new Type[] {typeof (int)},
        new Type[] {typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool)})]
    private class TestType
    {
    }

    [Test]
    public void FromAttributeApplication ()
    {
      ConcreteMixinTypeAttribute attribute = ((ConcreteMixinTypeAttribute[]) typeof (TestType).GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))[0];

      Assert.That (attribute.MixinIndex, Is.EqualTo (3));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (ConcreteMixinTypeAttributeTest)));
      Assert.That (attribute.MixinKinds, Is.EqualTo (new object[] {MixinKind.Extending, MixinKind.Used, MixinKind.Extending}));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (object), typeof (int) }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (int) }));
      Assert.That (attribute.ExplicitDependenciesPerMixin,
          Is.EqualTo (new object[] { typeof (object), typeof (double), typeof (bool), typeof (NextMixinDependency), typeof (string), typeof (bool), }));
    }

    [Test]
    public void FromClassContextSimple ()
    {
      ClassContext simpleContext = new ClassContext (typeof (object), typeof (string));
      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (7, simpleContext);

      Assert.That (attribute.MixinIndex, Is.EqualTo (7));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (object)));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), }));
      Assert.That (attribute.CompleteInterfaces, Is.Empty);
      Assert.That (attribute.ExplicitDependenciesPerMixin, Is.Empty);
    }

    [Test]
    public void FromClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .AddMixin (typeof (double)).WithDependency (typeof (int))
          .BuildClassContext();

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.That (attribute.MixinIndex, Is.EqualTo (5));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (int)));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (double), }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (uint), }));
      Assert.That (attribute.ExplicitDependenciesPerMixin, 
          Is.EqualTo (new object[] { typeof (string), typeof (bool), typeof (NextMixinDependency), typeof (double), typeof (int), }));
    }

    [Test]
    public void FromClassContext_MixinKinds ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending)
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .BuildClassContext ();

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (5, context);

      Assert.That (attribute.MixinIndex, Is.EqualTo (5));
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (int)));
      Assert.That (attribute.MixinKinds, Is.EqualTo (new object[] { MixinKind.Extending, MixinKind.Used }));
      Assert.That (attribute.MixinTypes, Is.EqualTo (new object[] { typeof (string), typeof (double), }));
      Assert.That (attribute.CompleteInterfaces, Is.EqualTo (new object[] { typeof (uint), }));
      Assert.That (attribute.ExplicitDependenciesPerMixin, Is.Empty);
    }


    [Test]
    public void GetMixinDefinition ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType3));
      MixinDefinition referenceDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (context).Mixins[0];

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (0, context);
      MixinDefinition definition = attribute.GetMixinDefinition ();
      Assert.That (definition, Is.SameAs (referenceDefinition));
    }

    [Test]
    public void BuilderFromClassContext ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .AddMixin (typeof (string)).WithDependency (typeof (bool)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      CustomAttributeBuilder builder = ConcreteMixinTypeAttribute.BuilderFromClassContext (12, context);
      TypeBuilder typeBuilder = ((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope.ObtainDynamicModuleWithWeakName ().DefineType ("Test_ConcreteMixinTypeAttribute");
      typeBuilder.SetCustomAttribute (builder);
      Type type = typeBuilder.CreateType ();
      ConcreteMixinTypeAttribute attribute = AttributeUtility.GetCustomAttribute<ConcreteMixinTypeAttribute> (type, false);
      Assert.That (attribute.GetClassContext (), Is.EqualTo (context));
      Assert.That (attribute.MixinIndex, Is.EqualTo (12));
    }

    [Test]
    public void NewAttributeExpressionFromClassContext ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .AddMixin (typeof (string)).WithDependency (typeof (bool)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      DynamicMethod method =
          new DynamicMethod ("Test_NewAttributeExpressionFromClassContext", typeof (ConcreteMixinTypeAttribute), new Type[] { typeof (ClassContext) });
      ILGenerator ilGenerator = method.GetILGenerator ();
      DynamicMethodCodeBuilder codeBuilder = new DynamicMethodCodeBuilder (ilGenerator);
      DynamicMethodEmitter emitter = new DynamicMethodEmitter (method);

      Expression expression = ConcreteMixinTypeAttribute.NewAttributeExpressionFromClassContext (12, context, codeBuilder);
      codeBuilder.AddStatement (new ReturnStatement (expression));

      PrivateInvoke.InvokeNonPublicMethod (codeBuilder, "Generate", emitter, ilGenerator);

      Func<ClassContext, ConcreteMixinTypeAttribute> compiledMethod =
          (Func<ClassContext, ConcreteMixinTypeAttribute>) method.CreateDelegate (typeof (Func<ClassContext, ConcreteMixinTypeAttribute>));
      ConcreteMixinTypeAttribute generatedAttribute = compiledMethod (context);
      ClassContext generatedContext = generatedAttribute.GetClassContext ();
      Assert.That (generatedContext, Is.EqualTo (context));
      Assert.That (generatedAttribute.MixinIndex, Is.EqualTo (12));
    }
  }
}