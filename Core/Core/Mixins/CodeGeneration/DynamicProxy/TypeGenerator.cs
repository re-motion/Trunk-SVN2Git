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
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Text;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal class TypeGenerator : ITypeGenerator
  {
    private static readonly MethodInfo s_concreteTypeInitializationMethod =
        typeof (GeneratedClassInstanceInitializer).GetMethod ("InitializeMixinTarget", new Type[] { typeof (IInitializableMixinTarget), typeof (bool) });
    private static readonly ConstructorInfo s_debuggerBrowsableAttributeConstructor =
        typeof (DebuggerBrowsableAttribute).GetConstructor (new Type[] { typeof (DebuggerBrowsableState) });
    private static readonly ConstructorInfo s_debuggerDisplayAttributeConstructor =
        typeof (DebuggerDisplayAttribute).GetConstructor (new Type[] { typeof (string) });
    private readonly PropertyInfo[] s_debuggerDisplayNameProperty = new PropertyInfo[] { typeof (DebuggerDisplayAttribute).GetProperty ("Name") };

    private readonly ModuleManager _module;
    private readonly TargetClassDefinition _configuration;
    private readonly CustomClassEmitter _emitter;
    private readonly BaseCallProxyGenerator _baseCallGenerator;

    private readonly FieldReference _configurationField;
    private readonly FieldReference _extensionsField;
    private readonly FieldReference _firstField;
    private readonly Dictionary<MethodInfo, MethodInfo> _baseCallMethods = new Dictionary<MethodInfo, MethodInfo>();
    private readonly MixinTypeGenerator[] _mixinTypeGenerators;

    public TypeGenerator (ModuleManager module, TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _module = module;
      _configuration = configuration;

      string typeName = nameProvider.GetNewTypeName (configuration);
      typeName = CustomClassEmitter.FlattenTypeName (typeName);

      List<Type> interfaces = GetInterfacesToImplement (true);

      TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;
      if (_configuration.IsAbstract)
        flags |= TypeAttributes.Abstract;

      bool forceUnsigned = StrongNameUtil.IsAnyTypeFromUnsignedAssembly (GetMixinTypes());
      ClassEmitter classEmitter = new ClassEmitter (_module.Scope, typeName, configuration.Type, interfaces.ToArray(), flags, forceUnsigned);
      _emitter = new CustomClassEmitter (classEmitter);

      _configurationField = _emitter.CreateStaticField ("__configuration", typeof (TargetClassDefinition), FieldAttributes.Private);
      HideFieldFromDebugger (_configurationField);
      _extensionsField = _emitter.CreateField ("__extensions", typeof (object[]), FieldAttributes.Private);
      HideFieldFromDebugger (_extensionsField);

      _mixinTypeGenerators = CreateMixinTypeGenerators (mixinNameProvider);
      _baseCallGenerator = new BaseCallProxyGenerator (this, classEmitter, _mixinTypeGenerators);

      _firstField = _emitter.CreateField ("__first", _baseCallGenerator.TypeBuilder, FieldAttributes.Private);
      HideFieldFromDebugger (_firstField);

      Statement initializationStatement = GetInitializationStatement();
      _emitter.ReplicateBaseTypeConstructors (initializationStatement);

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

    private Statement GetInitializationStatement ()
    {
      ConditionExpression condition = new SameConditionExpression (_extensionsField.ToExpression (), NullExpression.Instance);
      ExpressionStatement initializationMethodCall = new ExpressionStatement (new MethodInvocationExpression (null, s_concreteTypeInitializationMethod,
          new ConvertExpression (typeof (IInitializableMixinTarget), SelfReference.Self.ToExpression ()),
          new ConstReference (false).ToExpression ()));
      
      IfStatement ifStatement = new IfStatement (condition, initializationMethodCall);
      return ifStatement;
    }

    private void HideFieldFromDebugger (FieldReference field)
    {
      CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder (s_debuggerBrowsableAttributeConstructor,
          new object[] {DebuggerBrowsableState.Never});
      field.Reference.SetCustomAttribute (attributeBuilder);
    }

    private IEnumerable<Type> GetMixinTypes ()
    {
      foreach (MixinDefinition mixin in Configuration.Mixins)
        yield return mixin.Type;
    }

    private MixinTypeGenerator[] CreateMixinTypeGenerators (INameProvider mixinNameProvider)
    {
      MixinTypeGenerator[] mixinTypeGenerators = new MixinTypeGenerator[Configuration.Mixins.Count];
      for (int i = 0; i < mixinTypeGenerators.Length; ++i)
      {
        MixinDefinition mixinConfiguration = Configuration.Mixins[i];
        if (NeedsDerivedMixinType (mixinConfiguration))
          mixinTypeGenerators[i] = new MixinTypeGenerator (_module, this, mixinConfiguration, mixinNameProvider);
      }
      return mixinTypeGenerators;
    }

    public static bool NeedsDerivedMixinType (MixinDefinition configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      return configuration.HasOverriddenMembers () || configuration.HasProtectedOverriders ();
    }

    private List<Type> GetInterfacesToImplement (bool isSerializable)
    {
      List<Type> interfaces = new List<Type>();
      interfaces.Add (typeof (IMixinTarget));
      interfaces.Add (typeof (IInitializableMixinTarget));

      foreach (InterfaceIntroductionDefinition introduction in _configuration.IntroducedInterfaces)
        interfaces.Add (introduction.Type);

      Set<Type> alreadyImplementedInterfaces = new Set<Type> (interfaces);
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

    public CustomClassEmitter Emitter
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

    public IEnumerable<Tuple<MixinDefinition, Type>> GetBuiltMixinTypes ()
    {
      foreach (MixinTypeGenerator mixinTypeGenerator in _mixinTypeGenerators)
      {
        if (mixinTypeGenerator != null)
          yield return new Tuple<MixinDefinition, Type> (mixinTypeGenerator.Configuration, mixinTypeGenerator.GetBuiltType());
      }
    }

    internal FieldInfo ExtensionsField
    {
      get { return _extensionsField.Reference; }
    }

    private void AddTypeInitializer ()
    {
      ConstructorEmitter emitter = _emitter.CreateTypeConstructor ();

      Expression attributeExpression =
          ConcreteMixedTypeAttributeUtility.CreateNewAttributeExpression (Configuration.ConfigurationContext, emitter.CodeBuilder);
      LocalReference attributeLocal = emitter.CodeBuilder.DeclareLocal (typeof (ConcreteMixedTypeAttribute));
      emitter.CodeBuilder.AddStatement (new AssignStatement (attributeLocal, attributeExpression));
      
      MethodInfo getTargetClassDefinitionMethod = typeof (ConcreteMixedTypeAttribute).GetMethod ("GetTargetClassDefinition");
      Assertion.IsNotNull (getTargetClassDefinitionMethod);
      emitter.CodeBuilder.AddStatement (new AssignStatement (_configurationField,
          new VirtualMethodInvocationExpression (attributeLocal, getTargetClassDefinitionMethod)));

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
      SerializationImplementer.ImplementGetObjectDataByDelegation (Emitter, delegate (CustomMethodEmitter newMethod, bool baseIsISerializable)
          {
            return new MethodInvocationExpression (
                null,
                typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
                new ReferenceExpression (newMethod.ArgumentReferences[0]),
                new ReferenceExpression (newMethod.ArgumentReferences[1]),
                new ReferenceExpression (SelfReference.Self),
                new ReferenceExpression (_configurationField),
                new ReferenceExpression (_extensionsField),
                new ReferenceExpression (new ConstReference (!baseIsISerializable)));
          });

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
      AddDebuggerDisplayAttribute (configurationProperty, "Target class configuration for " + _configuration.Type.Name, "Configuration");

      CustomPropertyEmitter mixinsProperty =
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("Mixins"));
      mixinsProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_Mixins"));
      mixinsProperty.ImplementWithBackingField (_extensionsField);
      AddDebuggerDisplayAttribute (mixinsProperty, "Count = {__extensions.Length}", "Mixins");

      CustomPropertyEmitter firstProperty =
          Emitter.CreateInterfacePropertyImplementation (typeof (IMixinTarget).GetProperty ("FirstBaseCallProxy"));
      firstProperty.GetMethod =
          Emitter.CreateInterfaceMethodImplementation (typeof (IMixinTarget).GetMethod ("get_FirstBaseCallProxy"));
      firstProperty.ImplementWithBackingField (_firstField);
      AddDebuggerDisplayAttribute (firstProperty, "Generated proxy", "FirstBaseCallProxy");
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

    private void AddDebuggerDisplayAttribute (IAttributableEmitter property, string displayString, string nameString)
    {
      CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder (s_debuggerDisplayAttributeConstructor,
          new object[] { displayString }, s_debuggerDisplayNameProperty, new object[] { nameString });
      property.AddCustomAttribute (attributeBuilder);
    }

    private void ImplementIntroducedInterfaces ()
    {
      foreach (InterfaceIntroductionDefinition introduction in _configuration.IntroducedInterfaces)
      {
        Expression implementerExpression = new ConvertExpression (
            introduction.Type,
            new LoadArrayElementExpression (introduction.Implementer.MixinIndex, _extensionsField, typeof (object)));

        foreach (MethodIntroductionDefinition method in introduction.IntroducedMethods)
          ImplementIntroducedMethod (implementerExpression, method.ImplementingMember, method.InterfaceMember, method.Visibility);

        foreach (PropertyIntroductionDefinition property in introduction.IntroducedProperties)
          ImplementIntroducedProperty (implementerExpression, property);

        foreach (EventIntroductionDefinition eventIntro in introduction.IntroducedEvents)
          ImplementIntroducedEvent (eventIntro, implementerExpression);
      }
    }

    private CustomMethodEmitter ImplementIntroducedMethod (
        Expression implementerExpression,
        MethodDefinition implementingMember,
        MethodInfo interfaceMember,
        MemberVisibility visibility)
    {
      CustomMethodEmitter introducedMethod;
      if (visibility == MemberVisibility.Public)
        introducedMethod = Emitter.CreatePublicInterfaceMethodImplementation (interfaceMember);
      else
        introducedMethod = Emitter.CreateInterfaceMethodImplementation (interfaceMember);

      Statement initializationStatement = GetInitializationStatement ();
      introducedMethod.AddStatement (initializationStatement);

      ExpressionReference implementer =
          new ExpressionReference (interfaceMember.DeclaringType, implementerExpression, introducedMethod);
      introducedMethod.ImplementByDelegating (implementer, interfaceMember);

      ReplicateAttributes (implementingMember, introducedMethod);
      return introducedMethod;
    }

    private CustomPropertyEmitter ImplementIntroducedProperty (Expression implementerExpression, PropertyIntroductionDefinition property)
    {
      CustomPropertyEmitter propertyEmitter;
      if (property.Visibility == MemberVisibility.Public)
        propertyEmitter = Emitter.CreatePublicInterfacePropertyImplementation (property.InterfaceMember);
      else
        propertyEmitter = Emitter.CreateInterfacePropertyImplementation (property.InterfaceMember);

      if (property.IntroducesGetMethod)
        propertyEmitter.GetMethod = ImplementIntroducedMethod (
            implementerExpression,
            property.ImplementingMember.GetMethod,
            property.InterfaceMember.GetGetMethod(),
            property.Visibility);

      if (property.IntroducesSetMethod)
        propertyEmitter.SetMethod = ImplementIntroducedMethod (
            implementerExpression,
            property.ImplementingMember.SetMethod,
            property.InterfaceMember.GetSetMethod (),
            property.Visibility);

      ReplicateAttributes (property.ImplementingMember, propertyEmitter);
      return propertyEmitter;
    }

    private CustomEventEmitter ImplementIntroducedEvent (EventIntroductionDefinition eventIntro, Expression implementerExpression)
    {
      Assertion.IsNotNull (eventIntro.ImplementingMember.AddMethod);
      Assertion.IsNotNull (eventIntro.ImplementingMember.RemoveMethod);

      CustomEventEmitter eventEmitter;
      if (eventIntro.Visibility == MemberVisibility.Public)
        eventEmitter = Emitter.CreatePublicInterfaceEventImplementation (eventIntro.InterfaceMember);
      else
        eventEmitter = Emitter.CreateInterfaceEventImplementation (eventIntro.InterfaceMember);

      eventEmitter.AddMethod = ImplementIntroducedMethod (
          implementerExpression,
          eventIntro.ImplementingMember.AddMethod,
          eventIntro.InterfaceMember.GetAddMethod(),
          eventIntro.Visibility);
      eventEmitter.RemoveMethod = ImplementIntroducedMethod (
          implementerExpression,
          eventIntro.ImplementingMember.RemoveMethod,
          eventIntro.InterfaceMember.GetRemoveMethod (),
          eventIntro.Visibility);

      ReplicateAttributes (eventIntro.ImplementingMember, eventEmitter);
      return eventEmitter;
    }

    private void ImplementRequiredDuckMethods ()
    {
      foreach (RequiredFaceTypeDefinition faceRequirement in Configuration.RequiredFaceTypes)
      {
        if (faceRequirement.Type.IsInterface && !Configuration.ImplementedInterfaces.Contains (faceRequirement.Type)
            && !Configuration.IntroducedInterfaces.ContainsKey (faceRequirement.Type))
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
      foreach (MemberDefinition member in _configuration.GetAllMembers())
      {
        if (member.Overrides.Count > 0)
        {
          IAttributableEmitter emitter = ImplementOverride (member);
          ImplementAttributes (member, emitter);
        }
      }
    }

    private IAttributableEmitter ImplementOverride (MemberDefinition member)
    {
      MethodDefinition method = member as MethodDefinition;
      if (method != null)
        return ImplementMethodOverride (method);

      PropertyDefinition property = member as PropertyDefinition;
      if (property != null)
        return ImplementPropertyOverride (property);

      EventDefinition eventDefinition = member as EventDefinition;
      Assertion.IsNotNull (eventDefinition, "Only methods, properties, and events can be overridden.");
      return ImplementEventOverride (eventDefinition);
    }

    private CustomMethodEmitter ImplementMethodOverride (MethodDefinition method)
    {
      MethodInfo proxyMethod = _baseCallGenerator.GetProxyMethodForOverriddenMethod (method);
      CustomMethodEmitter methodOverride = Emitter.CreateMethodOverride (method.MethodInfo);
      Statement initializationStatement = GetInitializationStatement ();
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

    private void ImplementAttributes (IAttributeIntroductionTargetDefinition targetConfiguration, IAttributableEmitter targetEmitter)
    {
      foreach (AttributeDefinition attribute in targetConfiguration.CustomAttributes)
      {
        // only replicate those attributes from the base which are not inherited anyway
        if (!attribute.IsCopyTemplate
            && (!CanInheritAttributesFromBase (targetConfiguration) 
            || (!AttributeUtility.IsAttributeInherited (attribute.AttributeType) && !IsSuppressed (attribute))))
          AttributeReplicator.ReplicateAttribute (targetEmitter, attribute.Data);
      }

      // Replicate introduced attributes
      foreach (AttributeIntroductionDefinition attribute in targetConfiguration.IntroducedAttributes)
      {
        if (!IsSuppressed (attribute.Attribute))
          AttributeReplicator.ReplicateAttribute (targetEmitter, attribute.Attribute.Data);
      }
    }

    private bool IsSuppressed (AttributeDefinition attribute)
    {
      foreach (AttributeIntroductionDefinition suppressAttribute in Configuration.IntroducedAttributes[typeof (SuppressAttributesAttribute)])
      {
        Type suppressedType = (Type) suppressAttribute.Attribute.Data.ConstructorArguments[0].Value;
        if (suppressedType.IsAssignableFrom (attribute.AttributeType) 
            && suppressAttribute.Attribute.DeclaringDefinition.DeclaringEntity != attribute.DeclaringDefinition.DeclaringEntity)
          return true;
      }
      return false;
    }

    private bool CanInheritAttributesFromBase (IAttributeIntroductionTargetDefinition configuration)
    {
      // only methods and base classes can supply attributes for inheritance
      return configuration is TargetClassDefinition || configuration is MethodDefinition;
    }

    private void ReplicateAttributes (IAttributableDefinition source, IAttributableEmitter targetEmitter)
    {
      foreach (AttributeDefinition attribute in source.CustomAttributes)
        AttributeReplicator.ReplicateAttribute (targetEmitter, attribute.Data);
    }

    private void AddDebuggerAttributes ()
    {
      if (!Configuration.IntroducedAttributes.ContainsKey (typeof (DebuggerDisplayAttribute)))
      {
        string debuggerString = "Mix of " + _configuration.Type.FullName + " + "
            + SeparatedStringBuilder.Build (" + ", _configuration.Mixins, delegate (MixinDefinition m) { return m.FullName; });
        CustomAttributeBuilder debuggerAttribute = new CustomAttributeBuilder (s_debuggerDisplayAttributeConstructor, new object[] {debuggerString});
        Emitter.AddCustomAttribute (debuggerAttribute);
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
        string message = string.Format (
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

      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      CustomMethodEmitter baseCallMethod = new CustomMethodEmitter (Emitter, "__base__" + method.Name, attributes);
      baseCallMethod.CopyParametersAndReturnType (method);
      baseCallMethod.ImplementByBaseCall (method);
      return baseCallMethod.MethodBuilder;
    }

    public MethodInfo GetPublicMethodWrapper (MethodDefinition methodToBeWrapped)
    {
      ArgumentUtility.CheckNotNull ("methodToBeWrapped", methodToBeWrapped);
      if (methodToBeWrapped.DeclaringClass != Configuration)
        throw new ArgumentException ("Only methods from class " + Configuration.FullName + " can be wrapped.");

      return Emitter.GetPublicMethodWrapper (methodToBeWrapped.MethodInfo).MethodBuilder;
    }
  }
}
