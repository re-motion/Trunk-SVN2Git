/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal class BaseCallProxyGenerator
  {
    private readonly TypeGenerator _surroundingType;
    private readonly ConcreteMixinType[] _concreteMixinTypes;
    private readonly IClassEmitter _emitter;
    private readonly ConstructorEmitter _ctor;
    private readonly TargetClassDefinition _targetClassConfiguration;
    private readonly FieldReference _depthField;
    private readonly FieldReference _thisField;
    private readonly Dictionary<MethodDefinition, MethodInfo> _overriddenMethodToImplementationMap = new Dictionary<MethodDefinition, MethodInfo>();

    public BaseCallProxyGenerator (TypeGenerator surroundingType, IClassEmitter surroundingTypeEmitter, ConcreteMixinType[] concreteMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("surroundingType", surroundingType);
      ArgumentUtility.CheckNotNull ("surroundingTypeEmitter", surroundingTypeEmitter);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);

      _surroundingType = surroundingType;
      _concreteMixinTypes = concreteMixinTypes;
      _targetClassConfiguration = surroundingType.Configuration;

      List<Type> interfaces = new List<Type> ();
      interfaces.Add (typeof (IGeneratedBaseCallProxyType));
      foreach (RequiredBaseCallTypeDefinition requiredType in _targetClassConfiguration.RequiredBaseCallTypes)
        interfaces.Add (requiredType.Type);

      _emitter = surroundingTypeEmitter.CreateNestedClass("BaseCallProxy", typeof (object), interfaces.ToArray());
      _emitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SerializableAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      _thisField = _emitter.CreateField ("__this", _surroundingType.TypeBuilder);
      _depthField = _emitter.CreateField ("__depth", typeof (int));

      _ctor = ImplementConstructor();
      ImplementBaseCallsForOverriddenMethodsOnTarget();
      ImplementBaseCallsForRequirements();
    }

    public Type TypeBuilder
    {
      get { return _emitter.TypeBuilder; }
    }

    public FieldReference ThisField
    {
      get { return _thisField; }
    }

    public FieldReference DepthField
    {
      get { return _depthField; }
    }

    public TypeGenerator SurroundingType
    {
      get { return _surroundingType; }
    }

    public ConstructorInfo Ctor
    {
      get { return _ctor.ConstructorBuilder; }
    }

    public MethodInfo GetProxyMethodForOverriddenMethod (MethodDefinition method)
    {
      Assertion.IsTrue (_overriddenMethodToImplementationMap.ContainsKey (method), 
          "The method " + method.Name + " must be registered with the BaseCallProxyGenerator.");
      return _overriddenMethodToImplementationMap[method];
    }

    private ConstructorEmitter ImplementConstructor ()
    {
      ArgumentReference arg1 = new ArgumentReference (_surroundingType.TypeBuilder);
      ArgumentReference arg2 = new ArgumentReference (typeof (int));
      ConstructorEmitter ctor = _emitter.CreateConstructor (new ArgumentReference[] { arg1, arg2 });

      ctor.CodeBuilder.InvokeBaseConstructor();
      ctor.CodeBuilder.AddStatement (new AssignStatement (_thisField, arg1.ToExpression()));
      ctor.CodeBuilder.AddStatement (new AssignStatement (_depthField, arg2.ToExpression()));
      ctor.CodeBuilder.AddStatement (new ReturnStatement());
      return ctor;
    }

    private void ImplementBaseCallsForOverriddenMethodsOnTarget ()
    {
      foreach (MethodDefinition method in _targetClassConfiguration.GetAllMethods())
      {
        if (method.Overrides.Count > 0)
          ImplementBaseCallForOverridenMethodOnTarget (method);
      }
    }

    private void ImplementBaseCallForOverridenMethodOnTarget (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue (methodDefinitionOnTarget.DeclaringClass == _targetClassConfiguration);

      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
      CustomMethodEmitter methodOverride = _emitter.CreateMethod (methodDefinitionOnTarget.FullName, attributes);
      methodOverride.CopyParametersAndReturnType (methodDefinitionOnTarget.MethodInfo);
      
      BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodOverride, this, _concreteMixinTypes);
      methodGenerator.AddBaseCallToNextInChain (methodDefinitionOnTarget);

      _overriddenMethodToImplementationMap.Add (methodDefinitionOnTarget, methodOverride.MethodBuilder);

      MethodInfo sameMethodOnProxyBase = _emitter.BaseType.GetMethod(methodDefinitionOnTarget.Name,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, methodOverride.ParameterTypes, null);
      if (sameMethodOnProxyBase != null && sameMethodOnProxyBase.IsVirtual)
        _emitter.CreateMethodOverride (sameMethodOnProxyBase).ImplementByDelegating (
            new TypeReferenceWrapper (SelfReference.Self, _emitter.TypeBuilder),
            methodOverride.MethodBuilder);
    }

    private void ImplementBaseCallsForRequirements ()
    {
      foreach (RequiredBaseCallTypeDefinition requiredType in _targetClassConfiguration.RequiredBaseCallTypes)
        foreach (RequiredMethodDefinition requiredMethod in requiredType.Methods)
          ImplementBaseCallForRequirement (requiredMethod);
    }

    private void ImplementBaseCallForRequirement (RequiredMethodDefinition requiredMethod)
    {
      if (requiredMethod.ImplementingMethod.DeclaringClass == _targetClassConfiguration)
        ImplementBaseCallForRequirementOnTarget (requiredMethod);
      else
        ImplementBaseCallForRequirementOnMixin (requiredMethod);
    }

    // Required base call method implemented by "this" -> either overridden or not
    // If overridden, delegate to next in chain, else simply delegate to "this" field
    private void ImplementBaseCallForRequirementOnTarget (RequiredMethodDefinition requiredMethod)
    {
      CustomMethodEmitter methodImplementation = _emitter.CreateInterfaceMethodImplementation (requiredMethod.InterfaceMethod);
      BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodImplementation, this, _concreteMixinTypes);
      if (requiredMethod.ImplementingMethod.Overrides.Count == 0) // this is not an overridden method, call method directly on _this
        methodGenerator.AddBaseCallToTarget (requiredMethod.ImplementingMethod);
      else // this is an override, go to next in chain
      {
        // a base call for this might already have been implemented as an overriden method, but we explicitly implement the call chains anyway: it's
        // slightly easier and better for performance
        Assertion.IsFalse (_targetClassConfiguration.Methods.ContainsKey (requiredMethod.InterfaceMethod));
        methodGenerator.AddBaseCallToNextInChain (requiredMethod.ImplementingMethod);
      }
    }

    // Required base call method implemented by extension -> either as an overridde or not
    // If an overridde, delegate to next in chain, else simply delegate to the extension implementing it field
    private void ImplementBaseCallForRequirementOnMixin (RequiredMethodDefinition requiredMethod)
    {
      CustomMethodEmitter methodImplementation = _emitter.CreateInterfaceMethodImplementation (requiredMethod.InterfaceMethod);
      BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodImplementation, this, _concreteMixinTypes);
      if (requiredMethod.ImplementingMethod.Base == null) // this is not an override, call method directly on extension
        methodGenerator.AddBaseCallToTarget (requiredMethod.ImplementingMethod);
      else // this is an override, go to next in chain
      {
        // a base call for this has already been implemented as an overriden method, but we explicitly implement the call chains anyway: it's
        // slightly easier and better for performance
        Assertion.IsTrue (_overriddenMethodToImplementationMap.ContainsKey (requiredMethod.ImplementingMethod.Base));
        methodGenerator.AddBaseCallToNextInChain (requiredMethod.ImplementingMethod.Base);
      }
    }
  }
}
