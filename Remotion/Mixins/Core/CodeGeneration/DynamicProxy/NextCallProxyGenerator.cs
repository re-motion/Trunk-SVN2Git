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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.FunctionalProgramming;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class NextCallProxyGenerator
  {
    private readonly TypeGenerator _surroundingType;
    private readonly ConcreteMixinType[] _concreteMixinTypes;
    private readonly IClassEmitter _emitter;
    private readonly ConstructorEmitter _ctor;
    private readonly TargetClassDefinition _targetClassConfiguration;
    private readonly FieldReference _depthField;
    private readonly FieldReference _thisField;
    private readonly Dictionary<MethodDefinition, MethodInfo> _overriddenMethodToImplementationMap = new Dictionary<MethodDefinition, MethodInfo>();

    public NextCallProxyGenerator (TypeGenerator surroundingType, IClassEmitter surroundingTypeEmitter, ConcreteMixinType[] concreteMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("surroundingType", surroundingType);
      ArgumentUtility.CheckNotNull ("surroundingTypeEmitter", surroundingTypeEmitter);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);

      _surroundingType = surroundingType;
      _concreteMixinTypes = concreteMixinTypes;
      _targetClassConfiguration = surroundingType.Configuration;

      var interfaces = EnumerableUtility
          .Singleton (typeof (IGeneratedNextCallProxyType))
          .Concat (_targetClassConfiguration.RequiredNextCallTypes.Select (requiredType => requiredType.Type))
          .ToArray();

      _emitter = surroundingTypeEmitter.CreateNestedClass("NextCallProxy", typeof (object), interfaces);
      var serializableAttributeCtor = typeof (SerializableAttribute).GetConstructor (Type.EmptyTypes);
      Assertion.IsNotNull (serializableAttributeCtor);
      _emitter.AddCustomAttribute (new CustomAttributeBuilder (serializableAttributeCtor, new object[0]));

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
          "The method " + method.Name + " must be registered with the NextCallProxyGenerator.");
      return _overriddenMethodToImplementationMap[method];
    }

    private ConstructorEmitter ImplementConstructor ()
    {
      ArgumentReference arg1 = new ArgumentReference (_surroundingType.TypeBuilder);
      ArgumentReference arg2 = new ArgumentReference (typeof (int));
      ConstructorEmitter ctor = _emitter.CreateConstructor (new[] { arg1, arg2 });

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
      var methodOverride = _emitter.CreateMethod (methodDefinitionOnTarget.FullName, attributes, methodDefinitionOnTarget.MethodInfo);

      NextCallMethodGenerator methodGenerator = new NextCallMethodGenerator (methodOverride, this, _concreteMixinTypes);
      methodGenerator.AddBaseCallToNextInChain (methodDefinitionOnTarget);

      _overriddenMethodToImplementationMap.Add (methodDefinitionOnTarget, methodOverride.MethodBuilder);

      // If the base type of the emitter (object) already has the method being overridden (ToString, Equals, etc.), mixins could use the base 
      // implementation of the method rather than coming via the next call interface. Therefore, we need to override that base method and point it
      // towards our next call above.
      Assertion.IsTrue (
          _emitter.BaseType == typeof (object), 
          "This code assumes that only non-generic methods could match on the base type, which holds for object.");
      // Since object has no generic methods, we can use the exact parameter types to find the equivalent method.
      var equivalentMethodOnProxyBase = _emitter.BaseType.GetMethod (
          methodDefinitionOnTarget.Name,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          methodOverride.ParameterTypes,
          null);
      if (equivalentMethodOnProxyBase != null && equivalentMethodOnProxyBase.IsVirtual)
      {
        _emitter.CreateMethodOverride (equivalentMethodOnProxyBase).ImplementByDelegating (
            new TypeReferenceWrapper (SelfReference.Self, _emitter.TypeBuilder),
            methodOverride.MethodBuilder);
      }
    }

    private void ImplementBaseCallsForRequirements ()
    {
      foreach (RequiredNextCallTypeDefinition requiredType in _targetClassConfiguration.RequiredNextCallTypes)
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
      IMethodEmitter methodImplementation = _emitter.CreateInterfaceMethodImplementation (requiredMethod.InterfaceMethod);
      NextCallMethodGenerator methodGenerator = new NextCallMethodGenerator (methodImplementation, this, _concreteMixinTypes);
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
      IMethodEmitter methodImplementation = _emitter.CreateInterfaceMethodImplementation (requiredMethod.InterfaceMethod);
      NextCallMethodGenerator methodGenerator = new NextCallMethodGenerator (methodImplementation, this, _concreteMixinTypes);
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
