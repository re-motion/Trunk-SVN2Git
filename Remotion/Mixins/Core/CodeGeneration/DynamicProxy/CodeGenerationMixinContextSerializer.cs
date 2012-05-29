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
using System.Linq;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  /// <summary>
  /// Serializes a <see cref="MixinContext"/> object into instructions that reinstantiate an equivalent object when executed.
  /// </summary>
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
