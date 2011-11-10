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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal class NextCallMethodGenerator
  {
    private readonly IMethodEmitter _methodEmitter;
    private readonly ConcreteMixinType[] _concreteMixinTypes;
    private readonly TargetClassDefinition _targetClassConfiguration;
    private readonly FieldReference _depthField;
    private readonly FieldReference _thisField;
    private readonly TypeGenerator _surroundingType;

    public NextCallMethodGenerator (IMethodEmitter methodEmitter, NextCallProxyGenerator NextCallProxyGenerator,
        ConcreteMixinType[] concreteMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("methodEmitter", methodEmitter);
      ArgumentUtility.CheckNotNull ("NextCallProxyGenerator", NextCallProxyGenerator);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);

      _methodEmitter = methodEmitter;
      _concreteMixinTypes = concreteMixinTypes;
      _thisField = NextCallProxyGenerator.ThisField;
      _depthField = NextCallProxyGenerator.DepthField;
      _surroundingType = NextCallProxyGenerator.SurroundingType;
      _targetClassConfiguration = _surroundingType.Configuration;
    }

    public void AddBaseCallToNextInChain (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue(methodDefinitionOnTarget.DeclaringClass == _targetClassConfiguration);

      for (int potentialDepth = 0; potentialDepth < _targetClassConfiguration.Mixins.Count; ++potentialDepth)
      {
        MethodDefinition nextInChain = GetNextInChain (methodDefinitionOnTarget, potentialDepth);
        AddBaseCallToTargetIfDepthMatches (nextInChain, potentialDepth);
      }
      AddBaseCallToTarget (methodDefinitionOnTarget);
    }

    private MethodDefinition GetNextInChain (MethodDefinition methodDefinitionOnTarget, int potentialDepth)
    {
      Assertion.IsTrue (methodDefinitionOnTarget.DeclaringClass == _targetClassConfiguration);

      for (int i = potentialDepth; i < _targetClassConfiguration.Mixins.Count; ++i)
        if (methodDefinitionOnTarget.Overrides.ContainsKey (_targetClassConfiguration.Mixins[i].Type))
          return methodDefinitionOnTarget.Overrides[_targetClassConfiguration.Mixins[i].Type];
      return methodDefinitionOnTarget;
    }

    private void AddBaseCallToTargetIfDepthMatches (MethodDefinition target, int requestedDepth)
    {
      _methodEmitter.AddStatement (
          new IfStatement (
              new SameConditionExpression (_depthField.ToExpression (), new ConstReference (requestedDepth).ToExpression ()),
          CreateBaseCallStatement (target, _methodEmitter.ArgumentReferences)));
    }

    public void AddBaseCallToTarget (MethodDefinition target)
    {
      Statement baseCallStatement = CreateBaseCallStatement (target, _methodEmitter.ArgumentReferences);
      _methodEmitter.AddStatement (baseCallStatement);
    }

    private Statement CreateBaseCallStatement (MethodDefinition target, ArgumentReference[] args)
    {
      Expression[] argExpressions = Array.ConvertAll (args, a => a.ToExpression());
      
      if (target.DeclaringClass == _targetClassConfiguration)
        return CreateBaseCallToTargetClassStatement(target, argExpressions);
      else
        return CreateBaseCallToMixinStatement(target, argExpressions);
    }

    private Statement CreateBaseCallToTargetClassStatement (MethodDefinition target, Expression[] argExpressions)
    {
      MethodInfo baseCallMethod = _surroundingType.GetBaseCallMethod (target.MethodInfo);
      return new ReturnStatement (new AutomaticMethodInvocationExpression (new TypeReferenceWrapper (_thisField, _surroundingType.Emitter.TypeBuilder),
          baseCallMethod, argExpressions));
    }

    private Statement CreateBaseCallToMixinStatement (MethodDefinition target, Expression[] argExpressions)
    {
      MixinDefinition mixin = (MixinDefinition) target.DeclaringClass;
      MethodInfo baseCallMethod = GetMixinMethodToCall (mixin.MixinIndex, target);
      TypeReference mixinReference = GetMixinReference (mixin, baseCallMethod.DeclaringType);

      return new ReturnStatement (new AutomaticMethodInvocationExpression (mixinReference, baseCallMethod, argExpressions));
    }

    private MethodInfo GetMixinMethodToCall (int mixinIndex, MethodDefinition mixinMethod)
    {
      if (mixinMethod.MethodInfo.IsPublic)
        return mixinMethod.MethodInfo;
      else
      {
        Assertion.IsNotNull (_concreteMixinTypes[mixinIndex]);
        return _concreteMixinTypes[mixinIndex].GetMethodWrapper (mixinMethod.MethodInfo);
      }
    }

    private TypeReference GetMixinReference (MixinDefinition mixin, Type concreteMixinType)
    {
      Reference extensionsReference = new FieldInfoReference (_thisField, _surroundingType.ExtensionsField);
      Expression mixinExpression = new ConvertExpression (concreteMixinType, 
        new LoadArrayElementExpression (mixin.MixinIndex, extensionsReference, typeof (object)));
      return new ExpressionReference (concreteMixinType, mixinExpression, _methodEmitter);
    }
  }
}
