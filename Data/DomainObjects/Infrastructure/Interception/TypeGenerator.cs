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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  public class TypeGenerator
  {
    private readonly Type _publicDomainObjectType;
    private readonly Type _baseType;
    private const BindingFlags _infrastructureBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags _staticInfrastructureBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    private static readonly MethodInfo s_getPublicDomainObjectTypeMethod =
        typeof (DomainObject).GetMethod ("GetPublicDomainObjectType", _infrastructureBindingFlags);
    private static readonly MethodInfo s_preparePropertyAccessMethod =
        typeof (DomainObject).GetMethod ("PreparePropertyAccess", _infrastructureBindingFlags);
    private static readonly MethodInfo s_propertyAccessFinishedMethod =
        typeof (DomainObject).GetMethod ("PropertyAccessFinished", _infrastructureBindingFlags);
    private static readonly MethodInfo s_getPropertiesMethod =
        typeof (DomainObject).GetMethod ("get_Properties", _infrastructureBindingFlags);
    private static readonly MethodInfo s_getPropertyAccessorMethod =
        typeof (PropertyIndexer).GetMethod ("get_Item", _infrastructureBindingFlags, null, new Type[] {typeof (string)}, null);
    private static readonly MethodInfo s_propertyGetValueMethod =
        typeof (PropertyAccessor).GetMethod ("GetValue", _infrastructureBindingFlags);
    private static readonly MethodInfo s_propertySetValueMethod =
        typeof (PropertyAccessor).GetMethod ("SetValue", _infrastructureBindingFlags);
    private static readonly MethodInfo s_getObjectDataForGeneratedTypesMethod =
        typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes", _staticInfrastructureBindingFlags);

    private readonly CustomClassEmitter _classEmitter;

    static TypeGenerator ()
    {
      Assertion.IsNotNull (s_getPublicDomainObjectTypeMethod);
      Assertion.IsNotNull (s_preparePropertyAccessMethod);
      Assertion.IsNotNull (s_propertyAccessFinishedMethod);
      Assertion.IsNotNull (s_getPropertiesMethod);
      Assertion.IsNotNull (s_getPropertyAccessorMethod);
      Assertion.IsNotNull (s_propertyGetValueMethod);
      Assertion.IsNotNull (s_propertySetValueMethod);
      Assertion.IsNotNull (s_getObjectDataForGeneratedTypesMethod);
    }

    public TypeGenerator (Type publicDomainObjectType, Type typeToDeriveFrom, ModuleScope scope)
    {
      ArgumentUtility.CheckNotNull ("publicDomainObjectType", publicDomainObjectType);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("typeToDeriveFrom", typeToDeriveFrom, publicDomainObjectType);
      ArgumentUtility.CheckNotNull ("scope", scope);

      _publicDomainObjectType = publicDomainObjectType;
      _baseType = typeToDeriveFrom;

      // Analyze type before creating the class emitter; that way, we won't have half-created types lying around in case of configuration errors
      Set<Tuple<PropertyInfo, string>> properties = new InterceptedPropertyCollector (publicDomainObjectType).GetProperties ();

      string typeName = typeToDeriveFrom.FullName + "_WithInterception_ " + Guid.NewGuid ().ToString ("N");
      Type[] interfaces = new Type[] { typeof (IInterceptedDomainObject), typeof (ISerializable) };
      TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Serializable;

      _classEmitter = new CustomClassEmitter (scope, typeName, typeToDeriveFrom, interfaces, flags, false);

      _classEmitter.ReplicateBaseTypeConstructors ();
      OverrideGetPublicDomainObjectType ();
      ProcessProperties (properties);
      ImplementISerializable ();
    }

    public Type BuildType ()
    {
      return _classEmitter.BuildType ();
    }

    private void OverrideGetPublicDomainObjectType ()
    {
      _classEmitter.CreateMethodOverride (s_getPublicDomainObjectTypeMethod).ImplementByReturning (new TypeTokenExpression (_publicDomainObjectType));
    }

    private void ProcessProperties (IEnumerable<Tuple<PropertyInfo, string>> properties)
    {
      foreach (Tuple<PropertyInfo, string> property in properties)
        ProcessProperty (property.A, property.B);
    }

    private void ProcessProperty (PropertyInfo property, string propertyIdentifier)
    {
      MethodInfo getMethod = property.GetGetMethod (true);
      MethodInfo setMethod = property.GetSetMethod (true);

      MethodInfo topMostGetOverride = getMethod != null ? GetTopMostOverrideOfMethod (getMethod) : null;
      MethodInfo topMostSetOverride = setMethod != null ? GetTopMostOverrideOfMethod (setMethod) : null;

      if (IsOverridable (topMostGetOverride) || IsOverridable (topMostSetOverride))
      {
        if (topMostGetOverride != null)
        {
          if (IsAutomaticProperty(topMostGetOverride))
            ImplementAbstractGetAccessor (topMostGetOverride, propertyIdentifier);
          else if (IsOverridable (topMostGetOverride))
            OverrideAccessor (topMostGetOverride, propertyIdentifier);
        }

        if (topMostSetOverride != null)
        {
          if (IsAutomaticProperty (topMostSetOverride))
            ImplementAbstractSetAccessor (topMostSetOverride, propertyIdentifier, property.PropertyType);
          else if (IsOverridable (topMostSetOverride))
            OverrideAccessor (topMostSetOverride, propertyIdentifier);
        }
      }
    }

    private bool IsAutomaticProperty (MethodInfo accessorMethod)
    {
      return accessorMethod.IsAbstract || (IsOverridable (accessorMethod) && accessorMethod.IsDefined (typeof (CompilerGeneratedAttribute), false));
    }

    private bool IsOverridable (MethodInfo method)
    {
      return method != null && method.IsVirtual && !method.IsFinal;
    }

    private MethodInfo GetTopMostOverrideOfMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsTrue (method.DeclaringType.IsAssignableFrom (_baseType), "only methods declared on the base type (or below) are processed");
      if (method.DeclaringType == _baseType)
        return method;
      else
        return GetTopMostOverrideOfMethod (method.GetBaseDefinition(), _baseType);
    }

    private MethodInfo GetTopMostOverrideOfMethod (MethodInfo baseDefinition, Type typeToSearch)
    {
      if (baseDefinition.DeclaringType == typeToSearch)
        return baseDefinition;
      else
      {
        foreach (MethodInfo methodOnTypeToSearch in typeToSearch.GetMethods (_infrastructureBindingFlags | BindingFlags.DeclaredOnly))
        {
          if (methodOnTypeToSearch.GetBaseDefinition ().Equals (baseDefinition))
            return methodOnTypeToSearch;
        }

        Assertion.IsNotNull (typeToSearch.BaseType, "we have to get to the base definition at some point");
        return GetTopMostOverrideOfMethod (baseDefinition, typeToSearch.BaseType);
      }
    }

    private CustomMethodEmitter OverrideAccessor (MethodInfo accessor, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Assertion.IsFalse (accessor.IsAbstract);
      Assertion.IsTrue (IsOverridable (accessor));

      CustomMethodEmitter emitter = _classEmitter.CreatePrivateMethodOverride (accessor);
      MethodInvocationExpression baseCallExpression =
          new MethodInvocationExpression (SelfReference.Self, accessor, emitter.GetArgumentExpressions());

      ImplementWrappedAccessor (emitter, propertyIdentifier, baseCallExpression, accessor.ReturnType);

      return emitter;
    }

    private CustomMethodEmitter ImplementAbstractGetAccessor (MethodInfo accessor, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Assertion.IsTrue (accessor.ReturnType != typeof (void));

      CustomMethodEmitter emitter = _classEmitter.CreatePrivateMethodOverride (accessor);

      ExpressionReference propertyAccessorReference = CreatePropertyAccessorReference (propertyIdentifier, emitter);
      TypedMethodInvocationExpression getValueMethodCall = new TypedMethodInvocationExpression (
          propertyAccessorReference,
          s_propertyGetValueMethod.MakeGenericMethod (accessor.ReturnType));

      ImplementWrappedAccessor (emitter, propertyIdentifier, getValueMethodCall, accessor.ReturnType);

      return emitter;
    }

    private CustomMethodEmitter ImplementAbstractSetAccessor (MethodInfo accessor, string propertyIdentifier, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      Assertion.IsTrue (accessor.ReturnType == typeof (void));

      CustomMethodEmitter emitter = _classEmitter.CreatePrivateMethodOverride (accessor);

      Assertion.IsTrue (emitter.ArgumentReferences.Length > 0);
      Reference valueArgumentReference = emitter.ArgumentReferences[emitter.ArgumentReferences.Length - 1];

      ExpressionReference propertyAccessorReference = CreatePropertyAccessorReference (propertyIdentifier, emitter);
      TypedMethodInvocationExpression setValueMethodCall = new TypedMethodInvocationExpression (
          propertyAccessorReference,
          s_propertySetValueMethod.MakeGenericMethod (propertyType),
          valueArgumentReference.ToExpression());

      ImplementWrappedAccessor (emitter, propertyIdentifier, setValueMethodCall, typeof (void));

      return emitter;
    }

    private ExpressionReference CreatePropertyAccessorReference (string propertyIdentifier, CustomMethodEmitter emitter)
    {
      ExpressionReference propertiesReference = new ExpressionReference (
          typeof (PropertyIndexer),
          new MethodInvocationExpression (SelfReference.Self, s_getPropertiesMethod),
          emitter);
      return new ExpressionReference (
          typeof (PropertyAccessor),
          new TypedMethodInvocationExpression (
              propertiesReference, s_getPropertyAccessorMethod, new ConstReference (propertyIdentifier).ToExpression()),
          emitter);
    }


    private void ImplementWrappedAccessor (CustomMethodEmitter emitter, string propertyIdentifier, Expression implementation, Type returnType)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);
      ArgumentUtility.CheckNotNull ("implementation", implementation);
      ArgumentUtility.CheckNotNull ("returnType", returnType);

      emitter.AddStatement (
          new ExpressionStatement (
              new MethodInvocationExpression (
                  SelfReference.Self, s_preparePropertyAccessMethod, new ConstReference (propertyIdentifier).ToExpression())));

      Statement baseCallStatement;
      LocalReference returnValueLocal = null;
      if (returnType != typeof (void))
      {
        returnValueLocal = emitter.DeclareLocal (returnType);
        baseCallStatement = new AssignStatement (returnValueLocal, implementation);
      }
      else
        baseCallStatement = new ExpressionStatement (implementation);

      Statement propertyAccessFinishedStatement = new ExpressionStatement (
          new MethodInvocationExpression (SelfReference.Self, s_propertyAccessFinishedMethod));

      emitter.AddStatement (new TryFinallyStatement (new Statement[] {baseCallStatement}, new Statement[] {propertyAccessFinishedStatement}));

      if (returnType != typeof (void))
        emitter.AddStatement (new ReturnStatement (returnValueLocal));
      else
        emitter.AddStatement (new ReturnStatement());
    }

    private void ImplementISerializable ()
    {
      SerializationImplementer.ImplementGetObjectDataByDelegation (_classEmitter,
          delegate (CustomMethodEmitter methodEmitter, bool baseIsISerializable)
          {
            return new MethodInvocationExpression (
                null,
                s_getObjectDataForGeneratedTypesMethod,
                methodEmitter.ArgumentReferences[0].ToExpression(),
                methodEmitter.ArgumentReferences[1].ToExpression(),
                SelfReference.Self.ToExpression(),
                new ConstReference (!baseIsISerializable).ToExpression());
          });

      // Implement dummy ISerializable constructor if we haven't already replicated it
      SerializationImplementer.ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (_classEmitter);
    }
  }
}
