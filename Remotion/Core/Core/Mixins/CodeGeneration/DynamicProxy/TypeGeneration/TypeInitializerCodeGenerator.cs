// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Generates code required to implement a type initializer on a concrete mixed type.
  /// </summary>
  public class TypeInitializerCodeGenerator
  {
    private static readonly ConstructorInfo s_mixinArrayInitializerCtor = 
        typeof (MixinArrayInitializer).GetConstructor (new[] { typeof (Type), typeof (Type[]) });

    private readonly ClassContext _classContext;
    private readonly Type[] _expectedMixinTypes;
    private readonly FieldReference _classContextField;
    private readonly FieldReference _mixinArrayInitializerField;

    public TypeInitializerCodeGenerator (
        ClassContext classContext, 
        Type[] expectedMixinTypes,
        FieldReference classContextField, 
        FieldReference mixinArrayInitializerField)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("expectedMixinTypes", expectedMixinTypes);
      ArgumentUtility.CheckNotNull ("classContextField", classContextField);
      ArgumentUtility.CheckNotNull ("mixinArrayInitializerField", mixinArrayInitializerField);

      _classContext = classContext;
      _expectedMixinTypes = expectedMixinTypes;
      _classContextField = classContextField;
      _mixinArrayInitializerField = mixinArrayInitializerField;
    }

    public void ImplementTypeInitializer (IClassEmitter classEmitter)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);

      ConstructorEmitter constructorEmitter = classEmitter.CreateTypeConstructor ();

      var classContextSerializer = new CodeGenerationClassContextSerializer (constructorEmitter.CodeBuilder);
      _classContext.Serialize (classContextSerializer);
      var classContextExpression = classContextSerializer.GetConstructorInvocationExpression ();

      constructorEmitter.CodeBuilder.AddStatement (new AssignStatement (_classContextField, classContextExpression));

      var assignMixinArrayInitializerStatement = GetAssignMixinArrayInitializerStatement (constructorEmitter.CodeBuilder.DeclareLocal (typeof (Type[])));
      constructorEmitter.CodeBuilder.AddStatement (assignMixinArrayInitializerStatement);

      constructorEmitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }


    private Statement GetAssignMixinArrayInitializerStatement (LocalReference expectedMixinTypesLocal)
    {
      var statements = new List<Statement> ();

      // var expectedMixinTypes = new Type[<expectedMixinTypes.Length>];
      statements.Add (new AssignStatement (expectedMixinTypesLocal, new NewArrayExpression (_expectedMixinTypes.Length, typeof (Type))));

      for (int i = 0; i < _expectedMixinTypes.Length; ++i)
      {
        // expectedMixinTypes[i] = <expectedMixinTypes[i]>
        statements.Add (new AssignArrayStatement (expectedMixinTypesLocal, i, new TypeTokenExpression (_expectedMixinTypes[i])));
      }

      // <targetField> = MixinArrayInitializer (<targetType>, expectedMixinTypes

      var newInitializerExpression = new NewInstanceExpression (
          s_mixinArrayInitializerCtor,
          new TypeTokenExpression (_classContext.Type),
          expectedMixinTypesLocal.ToExpression ());

      statements.Add (new AssignStatement (_mixinArrayInitializerField, newInitializerExpression));
      return new BlockStatement (statements.ToArray ());
    }
  }
}
