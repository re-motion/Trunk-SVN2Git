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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class ConcreteMixedTypeAttributeUtility
  {
    private static readonly ConstructorInfo s_attributeCtor = typeof (ConcreteMixedTypeAttribute).GetConstructor (
        new Type[] {typeof (Type), typeof (MixinKind[]), typeof (Type[]), typeof (Type[]), typeof (Type[])});

    public static CustomAttributeBuilder CreateAttributeBuilder (ClassContext context)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);
      CustomAttributeBuilder builder = new CustomAttributeBuilder (s_attributeCtor,
          new object[] { attribute.TargetType, attribute.MixinKinds, attribute.MixinTypes, attribute.CompleteInterfaces, attribute.ExplicitDependenciesPerMixin });
      return builder;
    }

    public static Expression CreateNewAttributeExpression (ClassContext context, AbstractCodeBuilder codeBuilder)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context);

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
        codeBuilder.AddStatement (new GeneralTypeAssignArrayStatement (typeof (T), arrayLocal, i, elementExpressor(types[i])));
      return arrayLocal;
    }
  }
}
