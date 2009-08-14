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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  public class TypeInitializerCodeGenerator
  {
    private readonly ClassContext _classContext;
    private readonly FieldReference _classContextField;
    private readonly FieldReference _mixinArrayInitializerField;
    private readonly InitializationCodeGenerator _initializationCodeGenerator;

    public TypeInitializerCodeGenerator (ClassContext classContext, FieldReference classContextField, FieldReference mixinArrayInitializerField, InitializationCodeGenerator initializationCodeGenerator)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("classContextField", classContextField);
      ArgumentUtility.CheckNotNull ("mixinArrayInitializerField", mixinArrayInitializerField);
      ArgumentUtility.CheckNotNull ("initializationCodeGenerator", initializationCodeGenerator);

      _classContext = classContext;
      _classContextField = classContextField;
      _mixinArrayInitializerField = mixinArrayInitializerField;
      _initializationCodeGenerator = initializationCodeGenerator;
    }

    public void ImplementTypeInitializer (IClassEmitter classEmitter)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);

      ConstructorEmitter constructorEmitter = classEmitter.CreateTypeConstructor ();

      var classContextSerializer = new CodeGenerationClassContextSerializer (constructorEmitter.CodeBuilder);
      _classContext.Serialize (classContextSerializer);
      var classContextExpression = classContextSerializer.GetConstructorInvocationExpression ();

      constructorEmitter.CodeBuilder.AddStatement (new AssignStatement (_classContextField, classContextExpression));

      var assignMixinArrayInitializerStatement = _initializationCodeGenerator.GetAssignMixinArrayInitializerStatement (
          constructorEmitter.CodeBuilder.DeclareLocal (typeof (MixinArrayInitializer.ExpectedMixinInfo[])),
          _mixinArrayInitializerField);
      constructorEmitter.CodeBuilder.AddStatement (assignMixinArrayInitializerStatement);

      constructorEmitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }
  }
}