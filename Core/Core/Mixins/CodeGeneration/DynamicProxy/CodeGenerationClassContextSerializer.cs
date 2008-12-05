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
  public class CodeGenerationClassContextSerializer : IClassContextSerializer
  {
    private static readonly ConstructorInfo s_constructor = typeof (ClassContext).GetConstructor (new[] {typeof (Type), typeof (IEnumerable<MixinContext>), typeof (IEnumerable<Type>)});
    
    private readonly Expression[] _constructorArguments = new Expression[3];
    private readonly AbstractCodeBuilder _codeBuilder;

    public CodeGenerationClassContextSerializer (AbstractCodeBuilder codeBuilder)
    {
      ArgumentUtility.CheckNotNull ("codeBuilder", codeBuilder);
      _codeBuilder = codeBuilder;
    }

    public Expression GetConstructorInvocationExpression ()
    {
      Assertion.IsNotNull (s_constructor);
      return new NewInstanceExpression (s_constructor, _constructorArguments);
    }

    public void AddClassType(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _constructorArguments[0] = new TypeTokenExpression (type);
    }

    public void AddMixins(IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);

      var mixinContextArray = mixinContexts.ToArray();
      var local = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (mixinContextArray, _codeBuilder, GetMixinContextConstructorInvocationExpression);
      _constructorArguments[1] = local.ToExpression ();
    }

    public void AddCompleteInterfaces(IEnumerable<Type> completeInterfaces)
    {
      ArgumentUtility.CheckNotNull ("completeInterfaces", completeInterfaces);

      var completeInterfacesArray = completeInterfaces.ToArray ();
      LocalReference arrayLocal = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (completeInterfacesArray, _codeBuilder, t => new TypeTokenExpression (t));
      _constructorArguments[2] = arrayLocal.ToExpression ();
    }

    private Expression GetMixinContextConstructorInvocationExpression (MixinContext mc)
    {
      var serializer = new CodeGenerationMixinContextSerializer (_codeBuilder);
      mc.Serialize (serializer);
      return serializer.GetConstructorInvocationExpression ();
    }
  }
}