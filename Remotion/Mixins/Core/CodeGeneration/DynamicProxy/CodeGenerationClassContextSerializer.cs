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
  /// Serializes a <see cref="ClassContext"/> object into instructions that reinstantiate an equivalent object when executed.
  /// </summary>
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

    public void AddComposedInterfaces(IEnumerable<Type> composedInterfaces)
    {
      ArgumentUtility.CheckNotNull ("composedInterfaces", composedInterfaces);

      LocalReference arrayLocal = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (
          composedInterfaces.ToArray(), _codeBuilder, t => new TypeTokenExpression (t));
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
