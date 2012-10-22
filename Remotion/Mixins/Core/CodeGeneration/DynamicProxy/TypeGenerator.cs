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
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Text;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class TypeGenerator : ITypeGenerator
  {
    private readonly IConcreteMixinTypeProvider _concreteMixinTypeProvider;
    private readonly ICodeGenerationModule _module;
    private readonly TargetClassDefinition _configuration;
    private readonly IClassEmitter _emitter;
    private readonly NextCallProxyGenerator _nextCallGenerator;

    private readonly DebuggerDisplayAttributeGenerator _debuggerDisplayAttributeGenerator = new DebuggerDisplayAttributeGenerator();
    private readonly DebuggerBrowsableAttributeGenerator _debuggerBrowsableAttributeGenerator = new DebuggerBrowsableAttributeGenerator ();
    private readonly IntroducedMemberAttributeGenerator _introducedMemberAttributeGenerator = new IntroducedMemberAttributeGenerator ();
    private readonly AttributeGenerator _attributeGenerator = new AttributeGenerator ();
    private readonly AttributeReplicator _attributeReplicator = new AttributeReplicator ();

    private readonly InitializationCodeGenerator _initializationCodeGenerator;

    private readonly FieldReference _classContextField;
    private readonly FieldReference _extensionsField;
    private readonly FieldReference _firstField;
    private readonly FieldReference _mixinArrayInitializerField;
    private readonly Dictionary<MethodInfo, MethodInfo> _baseCallMethods = new Dictionary<MethodInfo, MethodInfo> ();

    public TypeGenerator (
        ICodeGenerationModule module, 
        TargetClassDefinition configuration, 
        IConcreteMixedTypeNameProvider nameProvider, 
        IConcreteMixinTypeProvider concreteMixinTypeProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinTypeProvider", concreteMixinTypeProvider);
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _concreteMixinTypeProvider = concreteMixinTypeProvider;
      _module = module;
      _configuration = configuration;

      string typeName = nameProvider.GetNameForConcreteMixedType (configuration);
      typeName = CustomClassEmitter.FlattenTypeName (typeName);

      var concreteMixinTypes = GetConcreteMixinTypes (); // elements may be null
      Assertion.IsTrue (concreteMixinTypes.Length == _configuration.Mixins.Count);

      var implementedInterfaceFinder = new ImplementedInterfaceFinder (
          _configuration.ImplementedInterfaces, 
          _configuration.ReceivedInterfaces, 
          _configuration.RequiredTargetCallTypes, 
          concreteMixinTypes.Where (t => t != null));
      
      var interfacesToImplement = implementedInterfaceFinder.GetInterfacesToImplement ();

      TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;
      if (_configuration.IsAbstract)
        flags |= TypeAttributes.Abstract;

      bool forceUnsigned = StrongNameUtil.IsAnyTypeFromUnsignedAssembly (GetMixinTypes());

      _emitter = _module.CreateClassEmitter (typeName, configuration.Type, interfacesToImplement, flags, forceUnsigned);

      _classContextField = _emitter.CreateStaticField ("__classContext", typeof (ClassContext), FieldAttributes.Private);
      _debuggerBrowsableAttributeGenerator.HideFieldFromDebugger (_classContextField);

      _mixinArrayInitializerField = _emitter.CreateStaticField ("__mixinArrayInitializer", typeof (MixinArrayInitializer), FieldAttributes.Private);
      _debuggerBrowsableAttributeGenerator.HideFieldFromDebugger (_mixinArrayInitializerField);

      _extensionsField = _emitter.CreateField ("__extensions", typeof (object[]), FieldAttributes.Private);
      _debuggerBrowsableAttributeGenerator.HideFieldFromDebugger (_extensionsField);

      _nextCallGenerator = new NextCallProxyGenerator (this, _emitter, concreteMixinTypes);

      _firstField = _emitter.CreateField ("__first", _nextCallGenerator.TypeBuilder, FieldAttributes.Private);
       _debuggerBrowsableAttributeGenerator.HideFieldFromDebugger (_firstField);

      var expectedMixinTypes = new Type[Configuration.Mixins.Count];
      for (int i = 0; i < expectedMixinTypes.Length; i++)
        expectedMixinTypes[i] = concreteMixinTypes[i] != null ? concreteMixinTypes[i].GeneratedType : Configuration.Mixins[i].Type;

      _initializationCodeGenerator = new InitializationCodeGenerator (
          expectedMixinTypes, 
          _extensionsField, 
          _firstField, 
          _classContextField, 
          _nextCallGenerator.Ctor);

      AddTypeInitializer (expectedMixinTypes);

      _emitter.ReplicateBaseTypeConstructors (
          delegate { }, 
          emitter => emitter.CodeBuilder.AddStatement (_initializationCodeGenerator.GetInitializationStatement ()));

      ImplementISerializable();

      ImplementIInitializableMixinTarget();
      ImplementIMixinTarget ();
      ImplementIntroducedInterfaces ();
      ImplementRequiredDuckMethods ();
      ImplementAttributes (configuration, _emitter);

      AddMixedTypeAttribute ();
      AddDebuggerAttributes();

      ImplementOverrides ();

      var overrideInterfaceImplementer = new OverrideInterfaceImplementer (Configuration, concreteMixinTypes);
      overrideInterfaceImplementer.ImplementOverridingMethods (Emitter);
    }

    private IEnumerable<Type> GetMixinTypes ()
    {
      return Configuration.Mixins.Select (mixin => mixin.Type);
    }

    private ConcreteMixinType[] GetConcreteMixinTypes ()
    {
      var concreteMixinTypes = new ConcreteMixinType[Configuration.Mixins.Count];
      for (int i = 0; i < concreteMixinTypes.Length; ++i)
      {
        MixinDefinition mixinDefinition = Configuration.Mixins[i];
        if (mixinDefinition.NeedsDerivedMixinType ())
        {
          concreteMixinTypes[i] = _concreteMixinTypeProvider.GetConcreteMixinType (mixinDefinition.GetConcreteMixinTypeIdentifier());
        }
      }
      return concreteMixinTypes;
    }

    public TypeBuilder TypeBuilder
    {
      get { return Emitter.TypeBuilder; }
    }

    public bool IsAssemblySigned
    {
      get { return ReflectionUtility.IsAssemblySigned (Emitter.TypeBuilder.Assembly); }
    }

    public IClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public TargetClassDefinition Configuration
    {
      get { return _configuration; }
    }

    public Type GetBuiltType ()
    {
      Type builtType = Emitter.BuildType();
      _module.OnTypeGenerated (builtType, TypeBuilder);
      return builtType;
    }

    internal FieldInfo ExtensionsField
    {
      get { return _extensionsField.Reference; }
    }

    private void AddTypeInitializer (Type[] expectedMixinTypes)
    {
      var codeGenerator = new TypeInitializerCodeGenerator (
          Configuration.ConfigurationContext, 
          expectedMixinTypes,
          _classContextField, 
          _mixinArrayInitializerField);
      codeGenerator.ImplementTypeInitializer (Emitter);
    }

    private void ImplementISerializable ()
    {
      var codeGenerator = new SerializationCodeGenerator (_classContextField, _extensionsField);
      codeGenerator.ImplementISerializable (Emitter);
    }

    private void ImplementIInitializableMixinTarget ()
    {
      _initializationCodeGenerator.ImplementIInitializableMixinTarget (Emitter, _mixinArrayInitializerField);
    }

    private void ImplementIMixinTarget ()
    {
      var codeGenerator = new MixinTargetCodeGenerator (
          Configuration.Type.Name, 
          _classContextField, 
          _extensionsField, 
          _firstField, 
          _debuggerDisplayAttributeGenerator,
          _initializationCodeGenerator);
      codeGenerator.ImplementIMixinTarget (Emitter);
    }

    private void ImplementIntroducedInterfaces ()
    {
      foreach (InterfaceIntroductionDefinition introduction in _configuration.ReceivedInterfaces)
      {
        Expression implementerExpression = new ConvertExpression (
            introduction.InterfaceType,
            new LoadArrayElementExpression (introduction.Implementer.MixinIndex, _extensionsField, typeof (object)));

        foreach (MethodIntroductionDefinition method in introduction.IntroducedMethods)
          ImplementIntroducedMethod (implementerExpression, method.ImplementingMember, method.InterfaceMember, method.Visibility);

        foreach (PropertyIntroductionDefinition property in introduction.IntroducedProperties)
          ImplementIntroducedProperty (implementerExpression, property.ImplementingMember, property.InterfaceMember, property.Visibility);

        foreach (EventIntroductionDefinition eventIntro in introduction.IntroducedEvents)
          ImplementIntroducedEvent (implementerExpression, eventIntro.ImplementingMember, eventIntro.InterfaceMember, eventIntro.Visibility);
      }
    }

    private IMethodEmitter ImplementIntroducedMethod (
        Expression implementerExpression,
        MethodDefinition implementingMember,
        MethodInfo interfaceMember,
        MemberVisibility visibility)
    {
      var methodEmitter = 
          visibility == MemberVisibility.Public 
          ? Emitter.CreatePublicInterfaceMethodImplementation (interfaceMember) 
          : Emitter.CreateInterfaceMethodImplementation (interfaceMember);

      // initialize this instance in case we're being called before the ctor has finished running
      var initializationStatement = _initializationCodeGenerator.GetInitializationStatement ();
      methodEmitter.AddStatement (initializationStatement);

      var implementer = new ExpressionReference (interfaceMember.DeclaringType, implementerExpression, methodEmitter);
      methodEmitter.ImplementByDelegating (implementer, interfaceMember);

      _attributeReplicator.ReplicateAttributes (implementingMember, methodEmitter);
      _introducedMemberAttributeGenerator.AddIntroducedMemberAttribute (methodEmitter, implementingMember, interfaceMember);
      return methodEmitter;
    }

    private void ImplementIntroducedProperty (
        Expression implementerExpression, 
        PropertyDefinition implementingMember, 
        PropertyInfo interfaceMember, 
        MemberVisibility visibility)
    {
      CustomPropertyEmitter propertyEmitter = 
          visibility == MemberVisibility.Public 
          ? Emitter.CreatePublicInterfacePropertyImplementation (interfaceMember) 
          : Emitter.CreateInterfacePropertyImplementation (interfaceMember);

      if (interfaceMember.GetGetMethod () != null)
      {
        propertyEmitter.GetMethod = ImplementIntroducedMethod (
            implementerExpression,
            implementingMember.GetMethod,
            interfaceMember.GetGetMethod(),
            visibility);
      }

      if (interfaceMember.GetSetMethod () != null)
      {
        propertyEmitter.SetMethod = ImplementIntroducedMethod (
            implementerExpression,
            implementingMember.SetMethod,
            interfaceMember.GetSetMethod (),
            visibility);
      }

      _attributeReplicator.ReplicateAttributes (implementingMember, propertyEmitter);
      _introducedMemberAttributeGenerator.AddIntroducedMemberAttribute (propertyEmitter, implementingMember, interfaceMember);
      return;
    }

    private void ImplementIntroducedEvent (
        Expression implementerExpression,
        EventDefinition implementingMember,
        EventInfo interfaceMember,
        MemberVisibility visibility)
    {
      Assertion.IsNotNull (implementingMember.AddMethod);
      Assertion.IsNotNull (implementingMember.RemoveMethod);

      CustomEventEmitter eventEmitter = 
          visibility == MemberVisibility.Public 
          ? Emitter.CreatePublicInterfaceEventImplementation (interfaceMember) 
          : Emitter.CreateInterfaceEventImplementation (interfaceMember);

      eventEmitter.AddMethod = ImplementIntroducedMethod (
          implementerExpression,
          implementingMember.AddMethod,
          interfaceMember.GetAddMethod(),
          visibility);
      eventEmitter.RemoveMethod = ImplementIntroducedMethod (
          implementerExpression,
          implementingMember.RemoveMethod,
          interfaceMember.GetRemoveMethod (),
          visibility);

      _attributeReplicator.ReplicateAttributes (implementingMember, eventEmitter);
      _introducedMemberAttributeGenerator.AddIntroducedMemberAttribute (eventEmitter, implementingMember, interfaceMember);
      return;
    }

    private void ImplementRequiredDuckMethods ()
    {
      foreach (RequiredTargetCallTypeDefinition faceRequirement in Configuration.RequiredTargetCallTypes)
      {
        if (faceRequirement.Type.IsInterface && !Configuration.ImplementedInterfaces.Contains (faceRequirement.Type)
            && !Configuration.ReceivedInterfaces.ContainsKey (faceRequirement.Type))
        {
          foreach (RequiredMethodDefinition requiredMethod in faceRequirement.Methods)
            ImplementRequiredDuckMethod (requiredMethod);
        }
      }
    }

    private void ImplementRequiredDuckMethod (RequiredMethodDefinition requiredMethod)
    {
      Assertion.IsTrue (requiredMethod.ImplementingMethod.DeclaringClass == Configuration,
        "Duck typing is only supported with members from the base type");

      IMethodEmitter methodImplementation = _emitter.CreateInterfaceMethodImplementation (requiredMethod.InterfaceMethod);
      methodImplementation.ImplementByDelegating (new TypeReferenceWrapper (SelfReference.Self, TypeBuilder),
          requiredMethod.ImplementingMethod.MethodInfo);
    }

    private void ImplementOverrides ()
    {
      foreach (MemberDefinitionBase member in _configuration.GetAllMembers())
      {
        if (member.Overrides.Count > 0)
        {
          IAttributableEmitter emitter = ImplementOverride (member);
          ImplementAttributes (member, emitter);
        }
      }
    }

    private IAttributableEmitter ImplementOverride (MemberDefinitionBase member)
    {
      var method = member as MethodDefinition;
      if (method != null)
        return ImplementMethodOverride (method);

      var property = member as PropertyDefinition;
      if (property != null)
        return ImplementPropertyOverride (property);

      var eventDefinition = member as EventDefinition;
      Assertion.IsNotNull (eventDefinition, "Only methods, properties, and events can be overridden.");
      return ImplementEventOverride (eventDefinition);
    }

    private IMethodEmitter ImplementMethodOverride (MethodDefinition method)
    {
      MethodInfo proxyMethod = _nextCallGenerator.GetProxyMethodForOverriddenMethod (method);
      IMethodEmitter methodOverride = Emitter.CreateMethodOverride (method.MethodInfo);

      // initialize this instance in case we're being called before the ctor has finished running
      var initializationStatement = _initializationCodeGenerator.GetInitializationStatement ();
      methodOverride.AddStatement (initializationStatement);
      
      methodOverride.ImplementByDelegating (new TypeReferenceWrapper (_firstField, _firstField.Reference.FieldType), proxyMethod);
      return methodOverride;
    }

    private CustomPropertyEmitter ImplementPropertyOverride (PropertyDefinition property)
    {
      CustomPropertyEmitter propertyOverride = Emitter.CreatePropertyOverride (property.PropertyInfo);
      if (property.GetMethod != null && property.GetMethod.Overrides.Count > 0)
        propertyOverride.GetMethod = ImplementMethodOverride (property.GetMethod);
      if (property.SetMethod != null && property.SetMethod.Overrides.Count > 0)
        propertyOverride.SetMethod = ImplementMethodOverride (property.SetMethod);
      return propertyOverride;
    }

    private CustomEventEmitter ImplementEventOverride (EventDefinition eventDefinition)
    {
      CustomEventEmitter eventOverride = Emitter.CreateEventOverride (eventDefinition.EventInfo);
      if (eventDefinition.AddMethod.Overrides.Count > 0)
        eventOverride.AddMethod = ImplementMethodOverride (eventDefinition.AddMethod);
      if (eventDefinition.RemoveMethod.Overrides.Count > 0)
        eventOverride.RemoveMethod = ImplementMethodOverride (eventDefinition.RemoveMethod);
      return eventOverride;
    }

    private void ImplementAttributes (IAttributeIntroductionTarget targetConfiguration, IAttributableEmitter targetEmitter)
    {
      foreach (AttributeDefinition attribute in targetConfiguration.CustomAttributes)
      {
        // only replicate those attributes from the base which are not inherited anyway
        if (!attribute.IsCopyTemplate
            && (!CanInheritAttributesFromBase (targetConfiguration)
            || (!AttributeUtility.IsAttributeInherited (attribute.AttributeType) && !IsSuppressedByMixin (attribute))))
        {
          _attributeGenerator.GenerateAttribute (targetEmitter, attribute.Data);
        }
      }

      // Replicate introduced attributes
      foreach (AttributeIntroductionDefinition attribute in targetConfiguration.ReceivedAttributes)
        _attributeGenerator.GenerateAttribute (targetEmitter, attribute.Attribute.Data);
    }

    private bool IsSuppressedByMixin (AttributeDefinition attribute)
    {
      var declaringEntity = attribute.DeclaringDefinition.CustomAttributeProvider;
      var suppressAttributesAttributes =
          from suppressAttribute in Configuration.ReceivedAttributes[typeof (SuppressAttributesAttribute)]
          let suppressAttributeInstance = (SuppressAttributesAttribute) suppressAttribute.Attribute.Instance
          let suppressingEntity = suppressAttribute.Attribute.DeclaringDefinition.CustomAttributeProvider
          where suppressAttributeInstance.IsSuppressed (attribute.AttributeType, declaringEntity, suppressingEntity)
          select suppressAttributeInstance;
      return suppressAttributesAttributes.Any();
    }

    private bool CanInheritAttributesFromBase (IAttributeIntroductionTarget configuration)
    {
      // only methods and base classes can supply attributes for inheritance
      return configuration is TargetClassDefinition || configuration is MethodDefinition;
    }

    private void AddDebuggerAttributes ()
    {
      if (!Configuration.ReceivedAttributes.ContainsKey (typeof (DebuggerDisplayAttribute)) 
          && !Configuration.CustomAttributes.ContainsKey (typeof (DebuggerDisplayAttribute)))
      {
        string debuggerString = "{ToString(),nq} (mixed)";
        _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (Emitter, debuggerString);
      }
    }

    private void AddMixedTypeAttribute ()
    {
      var orderedMixinTypes = Configuration.Mixins.Select (m => m.Type);
      CustomAttributeBuilder attributeBuilder = ConcreteMixedTypeAttributeUtility.CreateAttributeBuilder (
          Configuration.ConfigurationContext, 
          orderedMixinTypes);

      Emitter.AddCustomAttribute (attributeBuilder);
    }

    public MethodInfo GetBaseCallMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethod", overriddenMethod);
      if (!overriddenMethod.DeclaringType.IsAssignableFrom (TypeBuilder.BaseType))
      {
        string message = String.Format (
            "Cannot create base call method for a method defined on a different type than the base type: {0}.{1}.",
            overriddenMethod.DeclaringType.FullName,
            overriddenMethod.Name);
        throw new ArgumentException (message, "overriddenMethod");
      }
      if (!_baseCallMethods.ContainsKey (overriddenMethod))
        _baseCallMethods.Add (overriddenMethod, ImplementBaseCallMethod (overriddenMethod));
      return _baseCallMethods[overriddenMethod];
    }

    private MethodInfo ImplementBaseCallMethod (MethodInfo method)
    {
      Assertion.IsTrue (ReflectionUtility.IsPublicOrProtected (method));

      const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      IMethodEmitter baseCallMethod = Emitter.CreateMethod ("__base__" + method.Name, attributes, method);
      baseCallMethod.ImplementByBaseCall (method);
      return baseCallMethod.MethodBuilder;
    }

    public MethodInfo GetPublicMethodWrapper (MethodDefinition methodToBeWrapped)
    {
      ArgumentUtility.CheckNotNull ("methodToBeWrapped", methodToBeWrapped);
      if (methodToBeWrapped.DeclaringClass != Configuration)
        throw new ArgumentException ("Only methods from class " + Configuration.FullName + " can be wrapped.");

      return Emitter.GetPublicMethodWrapper (methodToBeWrapped.MethodInfo);
    }
  }
}
