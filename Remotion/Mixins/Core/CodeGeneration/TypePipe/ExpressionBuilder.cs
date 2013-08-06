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
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class ExpressionBuilder : IExpressionBuilder
  {
    private static readonly MethodInfo s_initializeMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IInitializableMixinTarget o) => o.Initialize());
    private static readonly MethodInfo s_initializeAfterDeserializationMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IInitializableMixinTarget o) => o.InitializeAfterDeserialization(null));

    public Expression CreateNewClassContext (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      var serializer = new ExpressionClassContextSerializer();
      classContext.Serialize (serializer);

      return serializer.CreateNewExpression();
    }

    public Expression CreateInitializationOnConstructionOrDeserialization (
        MutableType concreteTarget, Expression extensionsField, Expression extensionsInitializedField, Expression initializationSemantcis)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("extensionsInitializedField", extensionsInitializedField);
      ArgumentUtility.CheckNotNull ("initializationSemantcis", initializationSemantcis);

      // if (__extensions == null || !__extensionsInitialized) {
      //   if (initializationSemantics == InitializationSemantics.Construction)
      //     ((IInitializableMixinTarget) this).Initialize();
      //   else
      //     ((IInitializableMixinTarget) this).InitializeAfterDeserialization(mixinInstances: null);
      // }

      var @this = new ThisExpression (concreteTarget);
      var initializeOnConstructionOrDeserialization =
          Expression.IfThenElse (
              Expression.Equal (initializationSemantcis, Expression.Constant (InitializationSemantics.Construction)),
              Expression.Call (@this, s_initializeMethod),
              Expression.Call (@this, s_initializeAfterDeserializationMethod, extensionsField));
      return ExecuteIfInitializationIsNeeded (extensionsField, extensionsInitializedField, initializeOnConstructionOrDeserialization);
    }

    public Expression CreateInitialization (MutableType concreteTarget, Expression extensionsField, Expression extensionsInitializedField)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("extensionsInitializedField", extensionsInitializedField);

      // if (__extensions == null || !__extensionsInitialized)
      //   ((IInitializableMixinTarget) this).Initialize();

      var initializeAction = Expression.Call (new ThisExpression (concreteTarget), s_initializeMethod);
      return ExecuteIfInitializationIsNeeded (extensionsField, extensionsInitializedField, initializeAction);
    }

    public Expression CreateInitializingDelegation (
        MethodBodyContextBase bodyContext, Expression extensionsField, Expression extensionsInitializedField, Expression instance, MethodInfo methodToCall)
    {
      ArgumentUtility.CheckNotNull ("bodyContext", bodyContext);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("extensionsInitializedField", extensionsInitializedField);
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("methodToCall", methodToCall);

      // <CreateInitialization>
      // instance<GenericParameters>.MethodToCall(<parameters>);

      return Expression.Block (
          CreateInitialization (bodyContext.DeclaringType, extensionsField, extensionsInitializedField),
          bodyContext.DelegateTo (instance, methodToCall));
    }

    private Expression ExecuteIfInitializationIsNeeded (Expression extensionsField, Expression extensionsInitializedField, Expression action)
    {
      // if (__extensions == null || !__extensionsInitialized)
      //   <action>();

      return Expression.IfThen (
          Expression.OrElse (Expression.Equal (extensionsField, Expression.Constant (null)), Expression.Not (extensionsInitializedField)),
          action);
    }
  }
}