using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal class BaseCallMethodGenerator
  {
    private readonly CustomMethodEmitter _methodEmitter;
    private readonly MixinTypeGenerator[] _mixinTypeGenerators;
    private readonly TargetClassDefinition _targetClassConfiguration;
    private readonly FieldReference _depthField;
    private readonly FieldReference _thisField;
    private readonly TypeGenerator _surroundingType;

    public BaseCallMethodGenerator (CustomMethodEmitter methodEmitter, BaseCallProxyGenerator baseCallProxyGenerator,
        MixinTypeGenerator[] mixinTypeGenerators)
    {
      ArgumentUtility.CheckNotNull ("methodEmitter", methodEmitter);
      ArgumentUtility.CheckNotNull ("baseCallProxyGenerator", baseCallProxyGenerator);
      ArgumentUtility.CheckNotNull ("mixinTypeGenerators", mixinTypeGenerators);

      _methodEmitter = methodEmitter;
      _mixinTypeGenerators = mixinTypeGenerators;
      _thisField = baseCallProxyGenerator.ThisField;
      _depthField = baseCallProxyGenerator.DepthField;
      _surroundingType = baseCallProxyGenerator.SurroundingType;
      _targetClassConfiguration = _surroundingType.Configuration;
    }

    public void AddBaseCallToNextInChain (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue(methodDefinitionOnTarget.DeclaringClass == _targetClassConfiguration);

      for (int potentialDepth = 0; potentialDepth < _targetClassConfiguration.Mixins.Count; ++potentialDepth)
      {
        MethodDefinition nextInChain = GetNextInBaseChain (methodDefinitionOnTarget, potentialDepth);
        AddBaseCallToTargetIfDepthMatches (nextInChain, potentialDepth);
      }
      AddBaseCallToTarget (methodDefinitionOnTarget);
    }

    private MethodDefinition GetNextInBaseChain (MethodDefinition methodDefinitionOnTarget, int potentialDepth)
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
      Expression[] argExpressions = Array.ConvertAll<ArgumentReference, Expression> (args, delegate (ArgumentReference a) { return a.ToExpression (); });
      
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
        Assertion.IsNotNull (_mixinTypeGenerators[mixinIndex]);
        return _mixinTypeGenerators[mixinIndex].GetPublicMethodWrapper (mixinMethod);
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
