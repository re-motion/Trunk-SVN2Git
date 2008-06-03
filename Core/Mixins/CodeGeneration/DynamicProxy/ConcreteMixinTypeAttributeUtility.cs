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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class ConcreteMixinTypeAttributeUtility
  {
    private static readonly ConstructorInfo s_attributeCtor =
        typeof (ConcreteMixinTypeAttribute).GetConstructor (
            new Type[] {typeof (int), typeof (Type), typeof (MixinKind[]), typeof (Type[]), typeof (Type[]), typeof (Type[])});

    public static CustomAttributeBuilder CreateAttributeBuilder (int mixinIndex, ClassContext context)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.FromClassContext (mixinIndex, context);
      CustomAttributeBuilder builder = new CustomAttributeBuilder (s_attributeCtor, new object[] { attribute.MixinIndex, attribute.TargetType,
          attribute.MixinKinds, attribute.MixinTypes, attribute.CompleteInterfaces, attribute.ExplicitDependenciesPerMixin });
      return builder;
    }

    public static Expression CreateNewAttributeExpression (int mixinIndex, ClassContext context, AbstractCodeBuilder codeBuilder)
    {
      Assertion.IsNotNull (s_attributeCtor);

      Expression mixedTypeAttributeExpression = ConcreteMixedTypeAttributeUtility.CreateNewAttributeExpression (context, codeBuilder);
      Reference mixedTypeAttributeReference = codeBuilder.DeclareLocal(typeof (ConcreteMixedTypeAttribute));
      codeBuilder.AddStatement (new AssignStatement (mixedTypeAttributeReference, mixedTypeAttributeExpression));

      return new NewInstanceExpression (s_attributeCtor,
          new ConstReference (mixinIndex).ToExpression (),
          new PropertyReference (mixedTypeAttributeReference, typeof (ConcreteMixedTypeAttribute).GetProperty ("TargetType")).ToExpression (),
          new PropertyReference (mixedTypeAttributeReference, typeof (ConcreteMixedTypeAttribute).GetProperty ("MixinKinds")).ToExpression (),
          new PropertyReference (mixedTypeAttributeReference, typeof (ConcreteMixedTypeAttribute).GetProperty ("MixinTypes")).ToExpression (),
          new PropertyReference (mixedTypeAttributeReference, typeof (ConcreteMixedTypeAttribute).GetProperty ("CompleteInterfaces")).ToExpression (),
          new PropertyReference (mixedTypeAttributeReference, typeof (ConcreteMixedTypeAttribute).GetProperty ("ExplicitDependenciesPerMixin")).ToExpression ());
    }
  }
}
