// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
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

    public void AddExternalOverriders (HashSet<MethodInfo> externalOverriders)
    {
      var externalOverridersLocal = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (
          externalOverriders, 
          _codeBuilder, 
          GetResolveMethodExpression);

      _constructorArguments[1] = new NewInstanceExpression (s_hashSetConstructor, externalOverridersLocal.ToExpression());
    }

    public void AddWrappedProtectedMembers (HashSet<MethodInfo> wrappedProtectedMembers)
    {
      var wrappedProtectedMembersLocal = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (
          wrappedProtectedMembers, 
          _codeBuilder,
          GetResolveMethodExpression);
      
      _constructorArguments[2] = new NewInstanceExpression (s_hashSetConstructor, wrappedProtectedMembersLocal.ToExpression());
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