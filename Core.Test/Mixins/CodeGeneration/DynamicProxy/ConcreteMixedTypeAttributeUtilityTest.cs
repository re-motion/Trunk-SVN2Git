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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class ConcreteMixedTypeAttributeUtilityTest
  {
    [Test]
    public void CreateAttributeBuilder ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .AddMixin (typeof (string)).WithDependency (typeof (bool)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      CustomAttributeBuilder builder = ConcreteMixedTypeAttributeUtility.CreateAttributeBuilder (context);
      TypeBuilder typeBuilder = ((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope.ObtainDynamicModuleWithWeakName ().DefineType ("Test_ConcreteMixedTypeAttribute");
      typeBuilder.SetCustomAttribute (builder);
      Type type = typeBuilder.CreateType ();
      ConcreteMixedTypeAttribute attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (type, false);
      Assert.That (attribute.GetClassContext (), Is.EqualTo (context));
    }

    [Test]
    public void CreateNewAttributeExpression ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .AddMixin (typeof (string)).WithDependency (typeof (bool)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      DynamicMethod method =
          new DynamicMethod ("Test_NewAttributeExpressionFromClassContext", typeof (ConcreteMixedTypeAttribute), new Type[] { typeof (ClassContext) });
      ILGenerator ilGenerator = method.GetILGenerator ();
      DynamicMethodCodeBuilder codeBuilder = new DynamicMethodCodeBuilder (ilGenerator);
      DynamicMethodEmitter emitter = new DynamicMethodEmitter (method);

      Expression expression = ConcreteMixedTypeAttributeUtility.CreateNewAttributeExpression (context, codeBuilder);
      codeBuilder.AddStatement (new ReturnStatement (expression));

      PrivateInvoke.InvokeNonPublicMethod (codeBuilder, "Generate", emitter, ilGenerator);

      Func<ClassContext, ConcreteMixedTypeAttribute> compiledMethod =
          (Func<ClassContext, ConcreteMixedTypeAttribute>) method.CreateDelegate (typeof (Func<ClassContext, ConcreteMixedTypeAttribute>));
      ConcreteMixedTypeAttribute generatedAttribute = compiledMethod (context);
      ClassContext generatedContext = generatedAttribute.GetClassContext ();
      Assert.That (generatedContext, Is.EqualTo (context));
    }
  }
}
