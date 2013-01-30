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
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Data.DomainObjects.Mapping;
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
        typeof (DomainObject).GetMethod ("GetPublicDomainObjectTypeImplementation", _infrastructureBindingFlags);
    private static readonly MethodInfo s_performConstructorCheckMethod =
        typeof (DomainObject).GetMethod ("PerformConstructorCheck", _infrastructureBindingFlags);
    private static readonly MethodInfo s_preparePropertyAccessMethod =
        typeof (CurrentPropertyManager).GetMethod ("PreparePropertyAccess", _staticInfrastructureBindingFlags);
    private static readonly MethodInfo s_propertyAccessFinishedMethod =
        typeof (CurrentPropertyManager).GetMethod ("PropertyAccessFinished", _staticInfrastructureBindingFlags);
    private static readonly MethodInfo s_getPropertiesMethod =
        typeof (DomainObject).GetMethod ("get_Properties", _infrastructureBindingFlags);
    private static readonly MethodInfo s_getPropertyAccessorMethod =
        typeof (PropertyIndexer).GetMethod ("get_Item", _infrastructureBindingFlags, null, new[] {typeof (string)}, null);
    private static readonly MethodInfo s_propertyGetValueMethod =
        typeof (PropertyAccessor).GetMethod ("GetValue", _infrastructureBindingFlags);
    private static readonly MethodInfo s_propertySetValueMethod =
        typeof (PropertyAccessor).GetMethod ("SetValue", _infrastructureBindingFlags);
    private static readonly MethodInfo s_getObjectDataForGeneratedTypesMethod =
        typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes", _staticInfrastructureBindingFlags);

    private readonly IClassEmitter _classEmitter;

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

    public TypeGenerator (Type publicDomainObjectType, Type typeToDeriveFrom, ModuleScope scope, TypeConversionProvider typeConversionProvider)
    {
      ArgumentUtility.CheckNotNull ("publicDomainObjectType", publicDomainObjectType);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("typeToDeriveFrom", typeToDeriveFrom, publicDomainObjectType);
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("typeConversionProvider", typeConversionProvider);

      _publicDomainObjectType = publicDomainObjectType;
      _baseType = typeToDeriveFrom;

      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (
          publicDomainObjectType,
          t => new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0} as it is not part of the mapping.", t.FullName), t));

      // Analyze type before creating the class emitter; that way, we won't have half-created types lying around in case of configuration errors
      Set<Tuple<PropertyInfo, string>> properties = new InterceptedPropertyCollector (classDefinition, typeConversionProvider).GetProperties();

      string typeName = typeToDeriveFrom.FullName + "_WithInterception_" + Guid.NewGuid ().ToString ("N");
      var interfaces = new[] { typeof (IInterceptedDomainObject), typeof (ISerializable) };
      const TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Serializable;

      _classEmitter = new CustomClassEmitter (scope, typeName, typeToDeriveFrom, interfaces, flags, false);

      _classEmitter.ReplicateBaseTypeConstructors (delegate { }, delegate { });
      OverrideGetPublicDomainObjectType ();
      OverridePerformConstructorCheck ();
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

    private void OverridePerformConstructorCheck ()
    {
      _classEmitter.CreateMethodOverride (s_performConstructorCheckMethod).ImplementByReturningVoid ();
    }

    private void ProcessProperties (IEnumerable<Tuple<PropertyInfo, string>> properties)
    {
      foreach (Tuple<PropertyInfo, string> property in properties)
        ProcessProperty (property.Item1, property.Item2);
    }

    private void ProcessProperty (PropertyInfo property, string propertyIdentifier)
    {
      MethodInfo getMethod = property.GetGetMethod (true);
      MethodInfo setMethod = property.GetSetMethod (true);

      MethodInfo mostDerivedGetOverride = getMethod != null ? GetMostDerivedMethodOverride (getMethod) : null;
      MethodInfo mostDerivedSetOverride = setMethod != null ? GetMostDerivedMethodOverride (setMethod) : null;

      if (InterceptedPropertyCollector.IsOverridable (mostDerivedGetOverride))
      {
        if (InterceptedPropertyCollector.IsAutomaticPropertyAccessor (mostDerivedGetOverride))
          ImplementAbstractGetAccessor (mostDerivedGetOverride, propertyIdentifier);
        else
          OverrideAccessor (mostDerivedGetOverride, propertyIdentifier);
      }

      if (InterceptedPropertyCollector.IsOverridable (mostDerivedSetOverride))
      {
        if (InterceptedPropertyCollector.IsAutomaticPropertyAccessor (mostDerivedSetOverride))
          ImplementAbstractSetAccessor (mostDerivedSetOverride, propertyIdentifier, property.PropertyType);
        else
          OverrideAccessor (mostDerivedSetOverride, propertyIdentifier);
      }
    }

    private MethodInfo GetMostDerivedMethodOverride (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsTrue (method.DeclaringType.IsAssignableFrom (_baseType), "only methods declared on the base type (or below) are processed");
      if (method.DeclaringType == _baseType)
        return method;
      else
        return GetMostDerivedMethodOverride (method.GetBaseDefinition(), _baseType);
    }

    private MethodInfo GetMostDerivedMethodOverride (MethodInfo baseDefinition, Type typeToSearch)
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
        return GetMostDerivedMethodOverride (baseDefinition, typeToSearch.BaseType);
      }
    }

    private void OverrideAccessor (MethodInfo accessor, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Assertion.IsFalse (accessor.IsAbstract);
      Assertion.IsTrue (InterceptedPropertyCollector.IsOverridable (accessor));

      var emitter = _classEmitter.CreateFullNamedMethodOverride (accessor);
      var baseCallExpression = new MethodInvocationExpression (SelfReference.Self, accessor, emitter.GetArgumentExpressions());

      ImplementWrappedAccessor (emitter, propertyIdentifier, baseCallExpression, accessor.ReturnType);
    }

    private void ImplementAbstractGetAccessor (MethodInfo accessor, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Assertion.IsTrue (accessor.ReturnType != typeof (void));

      var emitter = _classEmitter.CreateFullNamedMethodOverride (accessor);

      ExpressionReference propertyAccessorReference = CreatePropertyAccessorReference (propertyIdentifier, emitter);
      var getValueMethodCall = 
          new TypedMethodInvocationExpression (propertyAccessorReference, s_propertyGetValueMethod.MakeGenericMethod (accessor.ReturnType));

      ImplementWrappedAccessor (emitter, propertyIdentifier, getValueMethodCall, accessor.ReturnType);
    }

    private void ImplementAbstractSetAccessor (MethodInfo accessor, string propertyIdentifier, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("accessor", accessor);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      Assertion.IsTrue (accessor.ReturnType == typeof (void));

      var emitter = _classEmitter.CreateFullNamedMethodOverride (accessor);

      Assertion.IsTrue (emitter.ArgumentReferences.Length > 0);
      Reference valueArgumentReference = emitter.ArgumentReferences[emitter.ArgumentReferences.Length - 1];

      ExpressionReference propertyAccessorReference = CreatePropertyAccessorReference (propertyIdentifier, emitter);
      var setValueMethodCall = new TypedMethodInvocationExpression (
          propertyAccessorReference,
          s_propertySetValueMethod.MakeGenericMethod (propertyType),
          valueArgumentReference.ToExpression());

      ImplementWrappedAccessor (emitter, propertyIdentifier, setValueMethodCall, typeof (void));

      return;
    }

    private ExpressionReference CreatePropertyAccessorReference (string propertyIdentifier, IMethodEmitter emitter)
    {
      var propertiesReference = new ExpressionReference (
          typeof (PropertyIndexer),
          new MethodInvocationExpression (SelfReference.Self, s_getPropertiesMethod),
          emitter);
      return new ExpressionReference (
          typeof (PropertyAccessor),
          new TypedMethodInvocationExpression (
              propertiesReference, s_getPropertyAccessorMethod, new ConstReference (propertyIdentifier).ToExpression()),
          emitter);
    }


    private void ImplementWrappedAccessor (IMethodEmitter emitter, string propertyIdentifier, Expression implementation, Type returnType)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);
      ArgumentUtility.CheckNotNull ("implementation", implementation);
      ArgumentUtility.CheckNotNull ("returnType", returnType);

      emitter.AddStatement (
          new ExpressionStatement (
              new MethodInvocationExpression (
                  null,
                  s_preparePropertyAccessMethod, 
                  new ConstReference (propertyIdentifier).ToExpression())));

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
          new MethodInvocationExpression (null, s_propertyAccessFinishedMethod));

      emitter.AddStatement (new TryFinallyStatement (new[] {baseCallStatement}, new[] {propertyAccessFinishedStatement}));

      if (returnType != typeof (void))
        emitter.AddStatement (new ReturnStatement (returnValueLocal));
      else
        emitter.AddStatement (new ReturnStatement());
    }

    private void ImplementISerializable ()
    {
      SerializationImplementer.ImplementGetObjectDataByDelegation (_classEmitter,
          (methodEmitter, baseIsISerializable) => new MethodInvocationExpression (
                null,
                s_getObjectDataForGeneratedTypesMethod,
                methodEmitter.ArgumentReferences[0].ToExpression(),
                methodEmitter.ArgumentReferences[1].ToExpression(),
                SelfReference.Self.ToExpression(),
                new ConstReference (!baseIsISerializable).ToExpression())
          );

      // Implement dummy ISerializable constructor if we haven't already replicated it
      SerializationImplementer.ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (_classEmitter);
    }
  }
}
