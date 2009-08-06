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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Generates the initialization statements to initialize a concrete mixed type and its mixins.
  /// </summary>
  public class InitializationStatementGenerator
  {
    private static readonly MethodInfo s_concreteTypeInitializationMethod =
        typeof (GeneratedClassInstanceInitializer).GetMethod ("InitializeMixinTarget", new[] { typeof (IInitializableMixinTarget), typeof (bool) });

    private readonly FieldReference _extensionsField;

    public InitializationStatementGenerator (FieldReference extensionsField)
    {
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      _extensionsField = extensionsField;
    }

    public Statement GetInitializationStatement ()
    {
      var initializationMethodCall = new ExpressionStatement (
          new MethodInvocationExpression (
              null,
              s_concreteTypeInitializationMethod,
              new ConvertExpression (typeof (IInitializableMixinTarget), SelfReference.Self.ToExpression()),
              new ConstReference (false).ToExpression()));

      var condition = new SameConditionExpression (_extensionsField.ToExpression(), NullExpression.Instance);
      return new IfStatement (condition, initializationMethodCall);
    }
  }
}