// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Collections;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public class CustomClassEmitter : IClassEmitter
  {
    private static readonly ConstructorInfo s_generatedMethodWrapperAttributeCtor = 
        typeof (GeneratedMethodWrapperAttribute).GetConstructor (new[] { typeof (int), typeof (Type[]) });

    public static string FlattenTypeName (string fullName)
    {
      ArgumentUtility.CheckNotNull ("fullName", fullName);
      return fullName.Replace ("+", "/");
    }

    private readonly AbstractTypeEmitter _innerEmitter;
    private readonly Cache<MethodInfo, CustomMethodEmitter> _publicMethodWrappers = new Cache<MethodInfo, CustomMethodEmitter>();
    private bool _hasBeenBuilt = false;
    private readonly List<CustomEventEmitter> _eventEmitters = new List<CustomEventEmitter>();

    public CustomClassEmitter (AbstractTypeEmitter innerEmitter)
    {
      ArgumentUtility.CheckNotNull ("innerEmitter", innerEmitter);
      _innerEmitter = innerEmitter;
    }

    public CustomClassEmitter (ModuleScope scope, string name, Type baseType)
        : this (scope, name, baseType, Type.EmptyTypes, TypeAttributes.Class | TypeAttributes.Public, false)
    {
    }

    public CustomClassEmitter (ModuleScope scope, string name, Type baseType, Type[] interfaces, TypeAttributes flags, bool forceUnsigned)
        : this (
            new ClassEmitterSupportingOpenGenericBaseType (
                ArgumentUtility.CheckNotNull ("scope", scope),
                ArgumentUtility.CheckNotNullOrEmpty ("name", name),
                CheckBaseType (baseType),
                CheckInterfaces (interfaces),
                flags,
                forceUnsigned))
    {
    }

    private static Type CheckBaseType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      if (baseType.IsInterface)
        throw new ArgumentException ("Base type must not be an interface (" + baseType.FullName + ").", "baseType");
      if (baseType.IsSealed)
        throw new ArgumentException ("Base type must not be sealed (" + baseType.FullName + ").", "baseType");
      return baseType;
    }

    private static Type[] CheckInterfaces (Type[] interfaces)
    {
      ArgumentUtility.CheckNotNull ("interfaces", interfaces);
      foreach (Type interfaceType in interfaces)
      {
        if (!interfaceType.IsInterface)
          throw new ArgumentException ("Interface type must not be a class or value type (" + interfaceType.FullName + ").", "interfaces");
      }
      return interfaces;
    }

    internal AbstractTypeEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public Type BaseType
    {
      get { return TypeBuilder.BaseType; }
    }

    public TypeBuilder TypeBuilder
    {
      get { return InnerEmitter.TypeBuilder; }
    }

    public bool HasBeenBuilt
    {
      get { return _hasBeenBuilt; }
    }

    public ConstructorEmitter CreateConstructor (ArgumentReference[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      return InnerEmitter.CreateConstructor (arguments);
    }

    public ConstructorEmitter CreateConstructor (Type[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      ArgumentReference[] argumentReferences = ArgumentsUtil.ConvertToArgumentReference (arguments);
      return CreateConstructor (argumentReferences);
    }

    public void CreateDefaultConstructor ()
    {
      InnerEmitter.CreateDefaultConstructor();
    }

    public ConstructorEmitter CreateTypeConstructor ()
    {
      return InnerEmitter.CreateTypeConstructor();
    }

    public FieldReference CreateField (string name, Type fieldType)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return InnerEmitter.CreateField (name, fieldType);
    }

    public FieldReference CreateField (string name, Type fieldType, FieldAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return InnerEmitter.CreateField (name, fieldType, attributes);
    }

    public FieldReference CreateStaticField (string name, Type fieldType)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return InnerEmitter.CreateStaticField (name, fieldType);
    }

    public FieldReference CreateStaticField (string name, Type fieldType, FieldAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return InnerEmitter.CreateStaticField (name, fieldType, attributes);
    }

    public CustomMethodEmitter CreateMethod (string name, MethodAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return new CustomMethodEmitter (this, name, attributes);
    }

    public CustomPropertyEmitter CreateProperty (string name, PropertyKind propertyKind, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      return new CustomPropertyEmitter (this, name, propertyKind, propertyType, Type.EmptyTypes, PropertyAttributes.None);
    }

    public CustomPropertyEmitter CreateProperty (
        string name, PropertyKind propertyKind, Type propertyType, Type[] indexParameters, PropertyAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNull ("indexParameters", indexParameters);

      return new CustomPropertyEmitter (this, name, propertyKind, propertyType, indexParameters, attributes);
    }

    public CustomEventEmitter CreateEvent (string name, EventKind eventKind, Type eventType, EventAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("eventType", eventType);

      return new CustomEventEmitter (this, name, eventKind, eventType, attributes);
    }

    public CustomEventEmitter CreateEvent (string name, EventKind eventKind, Type eventType)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("eventType", eventType);

      return CreateEvent (name, eventKind, eventType, EventAttributes.None);
    }

    internal void RegisterEventEmitter (CustomEventEmitter emitter)
    {
      _eventEmitters.Add (emitter);
    }

    public CustomMethodEmitter CreateMethodOverride (MethodInfo baseMethod)
    {
      ArgumentUtility.CheckNotNull ("baseMethod", baseMethod);
      MethodAttributes oldVisibility = baseMethod.Attributes & MethodAttributes.MemberAccessMask;
      return CreateMethodOverrideOrInterfaceImplementation (baseMethod, true, MethodAttributes.ReuseSlot | oldVisibility);
    }

    /// <summary>
    /// Creates a private method override, i.e. a method override with private visibility whose name includes the name of the base method's
    /// declaring type, similar to an explicit interface implementation.
    /// </summary>
    /// <param name="baseMethod">The base method to override.</param>
    /// <returns>A <see cref="CustomMethodEmitter"/> for the private method override.</returns>
    /// <remarks>This method can be useful when overriding several (shadowed) methods of the same name inherited by different base types.</remarks>
    public CustomMethodEmitter CreatePrivateMethodOverride (MethodInfo baseMethod)
    {
      ArgumentUtility.CheckNotNull ("baseMethod", baseMethod);
      return CreateMethodOverrideOrInterfaceImplementation (
          baseMethod, false, MethodAttributes.NewSlot | MethodAttributes.Private | MethodAttributes.Final);
    }

    public CustomMethodEmitter CreateInterfaceMethodImplementation (MethodInfo interfaceMethod)
    {
      ArgumentUtility.CheckNotNull ("interfaceMethod", interfaceMethod);
      return CreateMethodOverrideOrInterfaceImplementation (
          interfaceMethod, false, MethodAttributes.NewSlot | MethodAttributes.Private | MethodAttributes.Final);
    }

    /// <summary>
    /// Creates a public interface method implementation, i.e. an interface implementation with public visibility whose name equals the name
    /// of the interface method (like a C# implicit interface implementation).
    /// </summary>
    /// <param name="interfaceMethod">The interface method to implement.</param>
    /// <returns>A <see cref="CustomMethodEmitter"/> for the interface implementation.</returns>
    /// <remarks>The generated method has public visibility and the <see cref="MethodAttributes.NewSlot"/> flag set. This means that the method
    /// will shadow methods from the base type with the same name and signature, not override them. Use <see cref="CreatePrivateMethodOverride"/> to
    /// explicitly create an override for such a method.</remarks>
    public CustomMethodEmitter CreatePublicInterfaceMethodImplementation (MethodInfo interfaceMethod)
    {
      ArgumentUtility.CheckNotNull ("interfaceMethod", interfaceMethod);
      return CreateMethodOverrideOrInterfaceImplementation (interfaceMethod, true, MethodAttributes.NewSlot | MethodAttributes.Public);
    }

    private CustomMethodEmitter CreateMethodOverrideOrInterfaceImplementation (
        MethodInfo baseOrInterfaceMethod,
        bool keepName,
        MethodAttributes visibilityFlags)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceMethod", baseOrInterfaceMethod);

      MethodAttributes methodDefinitionAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual | visibilityFlags;
      if (baseOrInterfaceMethod.IsSpecialName)
        methodDefinitionAttributes |= MethodAttributes.SpecialName;

      string methodName;
      if (keepName)
        methodName = baseOrInterfaceMethod.Name;
      else
        methodName = string.Format ("{0}.{1}", baseOrInterfaceMethod.DeclaringType.FullName, baseOrInterfaceMethod.Name);

      CustomMethodEmitter methodDefinition = CreateMethod (methodName, methodDefinitionAttributes);
      methodDefinition.CopyParametersAndReturnType (baseOrInterfaceMethod);

      TypeBuilder.DefineMethodOverride (methodDefinition.MethodBuilder, baseOrInterfaceMethod);

      return methodDefinition;
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePropertyOverride (PropertyInfo baseProperty)
    {
      ArgumentUtility.CheckNotNull ("baseProperty", baseProperty);
      return CreatePropertyOverrideOrInterfaceImplementation (baseProperty, true);
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreateInterfacePropertyImplementation (PropertyInfo interfaceProperty)
    {
      ArgumentUtility.CheckNotNull ("interfaceProperty", interfaceProperty);
      return CreatePropertyOverrideOrInterfaceImplementation (interfaceProperty, false);
    }

    // does not create the property's methods
    public CustomPropertyEmitter CreatePublicInterfacePropertyImplementation (PropertyInfo interfaceProperty)
    {
      ArgumentUtility.CheckNotNull ("interfaceProperty", interfaceProperty);
      return CreatePropertyOverrideOrInterfaceImplementation (interfaceProperty, true);
    }

    // does not create the property's methods
    private CustomPropertyEmitter CreatePropertyOverrideOrInterfaceImplementation (PropertyInfo baseOrInterfaceProperty, bool keepName)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceProperty", baseOrInterfaceProperty);

      string propertyName;
      if (keepName)
        propertyName = baseOrInterfaceProperty.Name;
      else
        propertyName = string.Format ("{0}.{1}", baseOrInterfaceProperty.DeclaringType.FullName, baseOrInterfaceProperty.Name);
      Type[] indexParameterTypes = Array.ConvertAll (baseOrInterfaceProperty.GetIndexParameters(), p => p.ParameterType);

      CustomPropertyEmitter newProperty = CreateProperty (
          propertyName,
          PropertyKind.Instance,
          baseOrInterfaceProperty.PropertyType,
          indexParameterTypes,
          PropertyAttributes.None);

      return newProperty;
    }

    // does not create the event's methods
    public CustomEventEmitter CreateEventOverride (EventInfo baseEvent)
    {
      ArgumentUtility.CheckNotNull ("baseEvent", baseEvent);
      return CreateEventOverrideOrInterfaceImplementation (baseEvent, true);
    }

    // does not create the event's methods
    public CustomEventEmitter CreateInterfaceEventImplementation (EventInfo interfaceEvent)
    {
      ArgumentUtility.CheckNotNull ("interfaceEvent", interfaceEvent);
      return CreateEventOverrideOrInterfaceImplementation (interfaceEvent, false);
    }

    // does not create the event's methods
    public CustomEventEmitter CreatePublicInterfaceEventImplementation (EventInfo interfaceEvent)
    {
      ArgumentUtility.CheckNotNull ("interfaceEvent", interfaceEvent);
      return CreateEventOverrideOrInterfaceImplementation (interfaceEvent, true);
    }

    /// <summary>
    /// Creates a nested class within the type emitted by this <see cref="IClassEmitter"/>.
    /// </summary>
    /// <param name="typeName">The name of the nested type.</param>
    /// <param name="baseType">The base type of the nested type.</param>
    /// <param name="interfaces">The interfaces to be implemented by the nested type.</param>
    /// <returns>
    /// A new <see cref="IClassEmitter"/> for the nested class.
    /// </returns>
    public IClassEmitter CreateNestedClass (string typeName, Type baseType, Type[] interfaces)
    {
      return new CustomClassEmitter (new NestedClassEmitter (InnerEmitter, "BaseCallProxy", typeof (object), interfaces));
    }

    // does not create the event's methods
    private CustomEventEmitter CreateEventOverrideOrInterfaceImplementation (EventInfo baseOrInterfaceEvent, bool keepName)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterfaceEvent", baseOrInterfaceEvent);

      string eventName;
      if (keepName)
        eventName = baseOrInterfaceEvent.Name;
      else
        eventName = string.Format ("{0}.{1}", baseOrInterfaceEvent.DeclaringType.FullName, baseOrInterfaceEvent.Name);
      CustomEventEmitter newEvent = CreateEvent (eventName, EventKind.Instance, baseOrInterfaceEvent.EventHandlerType, EventAttributes.None);
      return newEvent;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);
      TypeBuilder.SetCustomAttribute (customAttribute);
    }

    private static bool IsPublicOrProtected (MethodBase member)
    {
      return member.IsPublic || member.IsFamily || member.IsFamilyOrAssembly;
    }

    public void ReplicateBaseTypeConstructors (Action<ConstructorEmitter> preStatementsAdder, Action<ConstructorEmitter> postStatementsAdder)
    {
      ArgumentUtility.CheckNotNull ("preStatementsAdder", preStatementsAdder);
      ArgumentUtility.CheckNotNull ("postStatementsAdder", postStatementsAdder);

      ConstructorInfo[] constructors = BaseType.GetConstructors (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (ConstructorInfo constructor in constructors)
      {
        if (IsPublicOrProtected (constructor))
          ReplicateBaseTypeConstructor (constructor, preStatementsAdder, postStatementsAdder);
      }
    }

    private void ReplicateBaseTypeConstructor (ConstructorInfo constructor, Action<ConstructorEmitter> preStatementsAdder, Action<ConstructorEmitter> postStatementsAdder)
    {
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentUtility.CheckNotNull ("preStatementsAdder", preStatementsAdder);
      ArgumentUtility.CheckNotNull ("postStatementsAdder", postStatementsAdder);

      ArgumentReference[] arguments = ArgumentsUtil.ConvertToArgumentReference (constructor.GetParameters());
      ConstructorEmitter newConstructor = InnerEmitter.CreateConstructor (arguments);

      preStatementsAdder (newConstructor);
      Expression[] argumentExpressions = ArgumentsUtil.ConvertArgumentReferenceToExpression (arguments);
      newConstructor.CodeBuilder.AddStatement (new ConstructorInvocationStatement (constructor, argumentExpressions));
      postStatementsAdder (newConstructor);

      newConstructor.CodeBuilder.AddStatement (new ReturnStatement());
    }

    public MethodInfo GetPublicMethodWrapper (MethodInfo methodToBeWrapped)
    {
      ArgumentUtility.CheckNotNull ("methodToBeWrapped", methodToBeWrapped);

      return _publicMethodWrappers.GetOrCreateValue (methodToBeWrapped, CreatePublicMethodWrapper).MethodBuilder;
    }

    private CustomMethodEmitter CreatePublicMethodWrapper (MethodInfo methodToBeWrapped)
    {
      const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      
      CustomMethodEmitter wrapper = CreateMethod ("__wrap__" + methodToBeWrapped.Name, attributes);
      wrapper.CopyParametersAndReturnType (methodToBeWrapped);
      wrapper.ImplementByDelegating (new TypeReferenceWrapper (SelfReference.Self, TypeBuilder), methodToBeWrapped);
      
      var moduleBuilder = (ModuleBuilder) InnerEmitter.TypeBuilder.Module;
      int metadataToken = moduleBuilder.GetMethodToken (methodToBeWrapped).Token;
      Type[] genericArguments = methodToBeWrapped.DeclaringType.IsGenericType ? methodToBeWrapped.DeclaringType.GetGenericArguments () : Type.EmptyTypes;
      var attributeBuilder = new CustomAttributeBuilder (s_generatedMethodWrapperAttributeCtor, new object[] { metadataToken, genericArguments });
      wrapper.AddCustomAttribute (attributeBuilder);

      return wrapper;
    }

    public Type BuildType ()
    {
      _hasBeenBuilt = true;
      foreach (CustomEventEmitter eventEmitter in _eventEmitters)
        eventEmitter.EnsureValid();

      return InnerEmitter.BuildType();
    }
  }
}
