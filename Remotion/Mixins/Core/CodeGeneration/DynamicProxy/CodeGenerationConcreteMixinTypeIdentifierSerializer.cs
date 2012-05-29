// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  /// <summary>
  /// Serializes a <see cref="ConcreteMixinTypeIdentifier"/> object into instructions that reinstantiate an equivalent object when executed.
  /// </summary>
  public class CodeGenerationConcreteMixinTypeIdentifierSerializer : IConcreteMixinTypeIdentifierSerializer
  {
    private static readonly ConstructorInfo s_constructor = typeof (ConcreteMixinTypeIdentifier).GetConstructor (
        new[] { typeof (Type), typeof (HashSet<MethodInfo>), typeof (HashSet<MethodInfo>) });
    private static readonly ConstructorInfo s_hashSetConstructor = typeof (HashSet<MethodInfo>).GetConstructor (
        new[] { typeof (IEnumerable<MethodInfo>) });
    private static readonly MethodInfo s_resolveMethodMethod = typeof (MethodResolver).GetMethod (
        "ResolveMethod", new[] { typeof (Type), typeof (string), typeof (string) });

    private readonly Expression[] _constructorArguments = new Expression[3];
    private readonly AbstractCodeBuilder _codeBuilder;

    public CodeGenerationConcreteMixinTypeIdentifierSerializer (AbstractCodeBuilder codeBuilder)
    {
      ArgumentUtility.CheckNotNull ("codeBuilder", codeBuilder);
      _codeBuilder = codeBuilder;
    }

    public Expression GetConstructorInvocationExpression ()
    {
      Assertion.IsNotNull (s_constructor);
      return new NewInstanceExpression (s_constructor, _constructorArguments);
    }

    public void AddMixinType (Type mixinType)
    {
      _constructorArguments[0] = new TypeTokenExpression (mixinType);
    }

    public void AddOverriders (HashSet<MethodInfo> overriders)
    {
      var overridersLocal = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (
          overriders, 
          _codeBuilder, 
          GetResolveMethodExpression);

      _constructorArguments[1] = new NewInstanceExpression (s_hashSetConstructor, overridersLocal.ToExpression ());
    }

    public void AddOverridden (HashSet<MethodInfo> overridden)
    {
      var overriddenLocal = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (
          overridden, 
          _codeBuilder,
          GetResolveMethodExpression);

      _constructorArguments[2] = new NewInstanceExpression (s_hashSetConstructor, overriddenLocal.ToExpression ());
    }

    private Expression GetResolveMethodExpression (MethodInfo methodInfo)
    {
      return new MethodInvocationExpression (
          null,
          s_resolveMethodMethod,
          new TypeTokenExpression (methodInfo.DeclaringType),
          new ConstReference (methodInfo.Name).ToExpression (),
          new ConstReference (methodInfo.ToString ()).ToExpression ());
    }
  }
}
