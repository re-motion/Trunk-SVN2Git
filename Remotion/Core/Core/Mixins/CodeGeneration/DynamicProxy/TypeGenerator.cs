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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Collections;
using Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Text;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class TypeGenerator : ITypeGenerator
  {
    private readonly CodeGenerationCache _codeGenerationCache;
    private readonly ICodeGenerationModule _module;
    private readonly TargetClassDefinition _configuration;
    private readonly IClassEmitter _emitter;
    private readonly BaseCallProxyGenerator _baseCallGenerator;

    private readonly DebuggerDisplayAttributeGenerator _debuggerDisplayAttributeGenerator = new DebuggerDisplayAttributeGenerator();
    private readonly DebuggerBrowsableAttributeGenerator _debuggerBrowsableAttributeGenerator = new DebuggerBrowsableAttributeGenerator ();
    private readonly IntroducedMemberAttributeGenerator _introducedMemberAttributeGenerator = new IntroducedMemberAttributeGenerator ();
    private readonly AttributeGenerator _attributeGenerator = new AttributeGenerator ();
    private readonly AttributeReplicator _attributeReplicator = new AttributeReplicator ();

    private readonly InitializationStatementGenerator _initializationStatementGenerator;

    private readonly FieldReference _configurationField;
    private readonly FieldReference _extensionsField;
    private readonly FieldReference _firstField;
    private readonly Dictionary<MethodInfo, MethodInfo> _baseCallMethods = new Dictionary<MethodInfo, MethodInfo>();
    private readonly ConcreteMixinType[] _concreteMixinTypes;

    public TypeGenerator (CodeGenerationCache codeGenerationCache, ICodeGenerationModule module, TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("codeGenerationCache", codeGenerationCache);
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _codeGenerationCache = codeGenerationCache;
      _module = module;
      _configuration = configuration;

      string typeName = nameProvider.GetNewTypeName (configuration);
      typeName = CustomClassEmitter.FlattenTypeName (typeName);

      List<Type> interfaces = GetInterfacesToImplement (true);

      TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;
      if (_configuration.IsAbstract)
        flags |= TypeAttributes.Abstract;

      bool forceUnsigned = StrongNameUtil.IsAnyTypeFromUnsignedAssembly (GetMixinTypes());

      _emitter = _module.CreateClassEmitter (typeName, configuration.Type, interfaces.ToArray (), flags, forceUnsigned);

      _configurationField = _emitter.CreateStaticField ("__configuration", typeof (TargetClassDefinition), FieldAttributes.Private);
      _debuggerBrowsableAttributeGenerator.HideFieldFromDebugger (_configurationField);
      
      _extensionsField = _emitter.CreateField ("__extensions", typeof (object[]), FieldAttributes.Private);
      _debuggerBrowsableAttributeGenerator.HideFieldFromDebugger (_extensionsField);

      _concreteMixinTypes = GetConcreteMixinTypes (mixinNameProvider);
      _baseCallGenerator = new BaseCallProxyGenerator (this, _emitter, _concreteMixinTypes);

      _initializationStatementGenerator = new InitializationStatementGenerator (_extensionsField);
      
      _firstField = _emitter.CreateField ("__first", _baseCallGenerator.TypeBuilder, FieldAttributes.Private);
       _debuggerBrowsableAttributeGenerator.HideFieldFromDebugger (_firstField);

      _emitter.ReplicateBaseTypeConstructors (
          delegate { }, 
          emitter => emitter.CodeBuilder.AddStatement (_initializationStatementGenerator.GetInitializationStatement ()));

      AddTypeInitializer ();

      ImplementISerializable();

      ImplementIMixinTarget ();
      ImplementIInitializableMixinTarget ();
      ImplementIntroducedInterfaces ();
      ImplementRequiredDuckMethods ();
      ImplementAttributes (configuration, _emitter);

      AddMixedTypeAttribute ();
      AddDebuggerAttributes();

      ImplementOverrides ();
    }

    private IEnumerable<Type> GetMixinTypes ()
    {
      foreach (MixinDefinition mixin in Configuration.Mixins)
        yield return mixin.Type;
    }

    private ConcreteMixinType[] GetConcreteMixinTypes (INameProvider mixinNameProvider)
    {
      var concreteMixinTypes = new ConcreteMixinType[Configuration.Mixins.Count];
      for (int i = 0; i < concreteMixinTypes.Length; ++i)
      {
        MixinDefinition mixinConfiguration = Configuration.Mixins[i];
        if (mixinConfiguration.NeedsDerivedMixinType ())
          concreteMixinTypes[i] = _codeGenerationCache.GetOrCreateConcreteMixinType (this, mixinConfiguration, mixinNameProvider);
      }
      return concreteMixinTypes;
    }

    private List<Type> GetInterfacesToImplement (bool isSerializable)
    {
      var interfaces = new List<Type> {typeof (IMixinTarget), typeof (IInitializableMixinTarget)};

      foreach (InterfaceIntroductionDefinition introduction in _configuration.ReceivedInterfaces)
        interfaces.Add (introduction.InterfaceType);

      var alreadyImplementedInterfaces = new Set<Type> (interfaces);
      alreadyImplementedInterfaces.AddRange (_configuration.ImplementedInterfaces);

      foreach (RequiredFaceTypeDefinition requiredFaceType in _configuration.RequiredFaceTypes)
      {
        if (requiredFaceType.Type.IsInterface && !alreadyImplementedInterfaces.Contains (requiredFaceType.Type))
          interfaces.Add (requiredFaceType.Type);
      }

      if (isSerializable)
        interfaces.Add (typeof (ISerializable));

      return interfaces;
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

    private void AddTypeInitializer ()
    {
      ConstructorEmitter emitter = _emitter.CreateTypeConstructor ();

      var classContextSerializer = new CodeGenerationClassContextSerializer (emitter.CodeBuilder);
      Configuration.ConfigurationContext.Serialize (classContextSerializer);
      var classContextExpression = classContextSerializer.GetConstructorInvocationExpression ();

      var currentCacheProperty = typeof (TargetClassDefinitionCache).BaseType.GetProperty ("Current");
      Assertion.IsNotNull (currentCacheProperty);
      var currentCachePropertyReference = new PropertyReference (null, currentCacheProperty);

      MethodInfo getTargetClassDefinitionMethod = typeof (TargetClassDefinitionCache).GetMethod ("GetTargetClassDefinition");

      emitter.CodeBuilder.AddStatement (new AssignStatement (_configurationField,
          new VirtualMethodInvocationExpression (currentCachePropertyReference, getTargetClassDefinitionMethod, classContextExpression)));

      emitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    internal static LocalReference GetFirstAttributeLocal (ConstructorEmitter emitter, Type attributeType)
    {
      LocalReference thisTypeLocal = emitter.CodeBuilder.DeclareLocal (typeof (Type));
      emitter.CodeBuilder.AddStatement (new AssignStatement (thisTypeLocal, new TypeTokenExpression (emitter.ConstructorBuilder.DeclaringType)));

      Expression firstAttributeExpression = new CustomAttributeExpression (thisTypeLocal, attributeType, 0, false);
      LocalReference firstAttributeLocal = emitter.CodeBuilder.DeclareLocal (attributeType);
      emitter.CodeBuilder.AddStatement (new AssignStatement (firstAttributeLocal, firstAttributeExpression));
      return firstAttributeLocal;
    }

    private void ImplementISerializable ()
    {
      SerializationImplementer.ImplementGetObjectDataByDelegation (
          Emitter,
          (newMethod, baseIsISerializable) => new MethodInvocationExpression (
                                                  null,
                                                  typeof (SerializationHelper).GetMethod (
                                                      "GetObjectDataForGeneratedTypes"),
                                                  new ReferenceExpression (newMethod.ArgumentReferences[0]),
                                                  new ReferenceExpression (newMethod.ArgumentReferences[1]),
                                                  new ReferenceExpression (SelfReference.Self),
                                                  new ReferenceExpression (_configurationField),
                                                  new ReferenceExpression (_extensionsField),
                                                  new ReferenceExpression (new ConstReference (!baseIsISerializable))));

      // Implement dummy ISerializable constructor if we haven't already replicated it
      SerializationImplementer.ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (Emitter);
    }

    private void ImplementIMixinTarget ()
    {
      CustomPropertyEmitter configurationProperty =
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("Configuration"));
      configurationProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_Configuration"));
      configurationProperty.ImplementWithBackingField (_configurationField);
      _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (
          configurationProperty, 
          "Target class configuration for " + _configuration.Type.Name, 
          "Configuration");

      CustomPropertyEmitter mixinsProperty =
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("Mixins"));
      mixinsProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_Mixins"));
      mixinsProperty.ImplementWithBackingField (_extensionsField);
      _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (
          mixinsProperty,
          "Count = {__extensions.Length}", 
          "Mixins");

      CustomPropertyEmitter firstProperty =
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("FirstBaseCallProxy"));
      firstProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_FirstBaseCallProxy"));
      firstProperty.ImplementWithBackingField (_firstField);
      _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (
          firstProperty, 
          "Generated proxy", 
          "FirstBaseCallProxy");
    }

    private void ImplementIInitializableMixinTarget ()
    {
      CustomMethodEmitter createProxyMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IInitializableMixinTarget).GetMethod ("CreateBaseCallProxy"));
      createProxyMethod.ImplementByReturning (new NewInstanceExpression(_baseCallGenerator.Ctor,
          SelfReference.Self.ToExpression(), createProxyMethod.ArgumentReferences[0].ToExpression()));

      CustomMethodEmitter setProxyMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IInitializableMixinTarget).GetMethod ("SetFirstBaseCallProxy"));
      setProxyMethod.AddStatement (new AssignStatement (_firstField, 
          new ConvertExpression(_baseCallGenerator.TypeBuilder, setProxyMethod.ArgumentReferences[0].ToExpression ())));
      setProxyMethod.ImplementByReturningVoid ();

      CustomMethodEmitter setExtensionsMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IInitializableMixinTarget).GetMethod ("SetExtensions"));
      setExtensionsMethod.AddStatement (new AssignStatement (_extensionsField, setExtensionsMethod.ArgumentReferences[0].ToExpression ()));
      setExtensionsMethod.ImplementByReturningVoid ();
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

    private CustomMethodEmitter ImplementIntroducedMethod (
        Expression implementerExpression,
        MethodDefinition implementingMember,
        MethodInfo interfaceMember,
        MemberVisibility visibility)
    {
      CustomMethodEmitter methodEmitter = 
          visibility == MemberVisibility.Public 
          ? Emitter.CreatePublicInterfaceMethodImplementation (interfaceMember) 
          : Emitter.CreateInterfaceMethodImplementation (interfaceMember);

      var initializationStatement = _initializationStatementGenerator.GetInitializationStatement ();
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
      foreach (RequiredFaceTypeDefinition faceRequirement in Configuration.RequiredFaceTypes)
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

      CustomMethodEmitter methodImplementation = _emitter.CreateInterfaceMethodImplementation (requiredMethod.InterfaceMethod);
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

    private CustomMethodEmitter ImplementMethodOverride (MethodDefinition method)
    {
      MethodInfo proxyMethod = _baseCallGenerator.GetProxyMethodForOverriddenMethod (method);
      CustomMethodEmitter methodOverride = Emitter.CreateMethodOverride (method.MethodInfo);
      var initializationStatement = _initializationStatementGenerator.GetInitializationStatement ();
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
      ICustomAttributeProvider declaringEntity = attribute.DeclaringDefinition.CustomAttributeProvider;
      foreach (AttributeIntroductionDefinition suppressAttribute in Configuration.ReceivedAttributes[typeof (SuppressAttributesAttribute)])
      {
        var suppressAttributeInstance = (SuppressAttributesAttribute)suppressAttribute.Attribute.Instance;
        ICustomAttributeProvider suppressingEntity = suppressAttribute.Attribute.DeclaringDefinition.CustomAttributeProvider;
        if (suppressAttributeInstance.IsSuppressed (attribute.AttributeType, declaringEntity, suppressingEntity))
          return true;
      }
      return false;
    }

    private bool CanInheritAttributesFromBase (IAttributeIntroductionTarget configuration)
    {
      // only methods and base classes can supply attributes for inheritance
      return configuration is TargetClassDefinition || configuration is MethodDefinition;
    }

    private void AddDebuggerAttributes ()
    {
      if (!Configuration.ReceivedAttributes.ContainsKey (typeof (DebuggerDisplayAttribute)))
      {
        string debuggerString = "Mix of " + _configuration.Type.FullName + " + " 
            + SeparatedStringBuilder.Build (" + ", _configuration.Mixins, m => m.FullName);
        _debuggerDisplayAttributeGenerator.AddDebuggerDisplayAttribute (
            Emitter,
            debuggerString);
      }
    }

    private void AddMixedTypeAttribute ()
    {
      CustomAttributeBuilder attributeBuilder = ConcreteMixedTypeAttributeUtility.CreateAttributeBuilder (Configuration.ConfigurationContext);
      Emitter.AddCustomAttribute (attributeBuilder);
    }

    public MethodInfo GetBaseCallMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("method", overriddenMethod);
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
      IMethodEmitter baseCallMethod = Emitter.CreateMethod ("__base__" + method.Name, attributes);
      baseCallMethod.CopyParametersAndReturnType (method);
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
