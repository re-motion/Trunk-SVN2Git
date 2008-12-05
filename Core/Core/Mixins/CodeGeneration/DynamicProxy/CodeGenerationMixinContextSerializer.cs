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
using System.Linq;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class CodeGenerationMixinContextSerializer : IMixinContextSerializer
  {
    private static readonly ConstructorInfo s_constructor = 
        typeof (MixinContext).GetConstructor (new[] {typeof (MixinKind), typeof (Type), typeof (MemberVisibility), typeof (IEnumerable<Type>)});
    
    private readonly Expression[] _constructorArguments = new Expression[4];
    private readonly AbstractCodeBuilder _codeBuilder;

    public CodeGenerationMixinContextSerializer (AbstractCodeBuilder codeBuilder)
    {
      ArgumentUtility.CheckNotNull ("codeBuilder", codeBuilder);

      _codeBuilder = codeBuilder;
    }

    public Expression GetConstructorInvocationExpression ()
    {
      Assertion.IsNotNull (s_constructor);
      return new NewInstanceExpression (s_constructor, _constructorArguments);
    }

    public void AddMixinType(Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _constructorArguments[1] = new TypeTokenExpression (mixinType);
    }

    public void AddMixinKind(MixinKind mixinKind)
    {
      _constructorArguments[0] = new ConstReference ((int) mixinKind).ToExpression();
    }

    public void AddIntroducedMemberVisibility(MemberVisibility introducedMemberVisibility)
    {
      _constructorArguments[2] = new ConstReference ((int) introducedMemberVisibility).ToExpression ();
    }

    public void AddExplicitDependencies(IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
      var explicitDependenciesArray = explicitDependencies.ToArray ();

      ArgumentUtility.CheckNotNull ("explicitDependenciesArray", explicitDependenciesArray);
      ArgumentUtility.CheckNotNull ("codeBuilder", _codeBuilder);
      LocalReference arrayLocal = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (explicitDependenciesArray, _codeBuilder, t => new TypeTokenExpression (t));
      _constructorArguments[3] = arrayLocal.ToExpression();
    }
  }
}