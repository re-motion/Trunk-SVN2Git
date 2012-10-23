// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Implements <see cref="IMutableMemberFactory"/>.
  /// </summary>
  public class MutableMemberFactory : IMutableMemberFactory
  {
    private readonly IMemberSelector _memberSelector;
    private readonly IRelatedMethodFinder _relatedMethodFinder;

    public MutableMemberFactory (IMemberSelector memberSelector, IRelatedMethodFinder relatedMethodFinder)
    {
      _memberSelector = memberSelector;
      _relatedMethodFinder = relatedMethodFinder;
    }

    public MutableFieldInfo CreateMutableField (MutableType declaringType, Type type, string name, FieldAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var signature = new FieldSignature (type);
      if (declaringType.AllMutableFields.Any (f => f.Name == name && FieldSignature.Create (f).Equals (signature)))
        throw new ArgumentException ("Field with equal name and signature already exists.", "name");

      var descriptor = UnderlyingFieldInfoDescriptor.Create (type, name, attributes);
      var field = new MutableFieldInfo (declaringType, descriptor);

      return field;
    }

    public MutableConstructorInfo CreateMutableConstructor (
        MutableType declaringType,
        MethodAttributes attributes,
        IEnumerable<ParameterDeclaration> parameterDeclarations,
        Func<ConstructorBodyCreationContext, Expression> bodyProvider)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull ("bodyProvider", bodyProvider);

      var invalidAttributes =
          new[]
          {
              MethodAttributes.Abstract, MethodAttributes.HideBySig, MethodAttributes.PinvokeImpl,
              MethodAttributes.RequireSecObject, MethodAttributes.UnmanagedExport, MethodAttributes.Virtual
          };
      CheckForInvalidAttributes ("constructor", invalidAttributes, attributes);

      if ((attributes & MethodAttributes.Static) != 0)
        throw new ArgumentException ("Adding static constructors is not (yet) supported.", "attributes");

      var parameterDescriptors = UnderlyingParameterInfoDescriptor.CreateFromDeclarations (parameterDeclarations).ConvertToCollection();

      var signature = new MethodSignature (typeof (void), parameterDescriptors.Select (pd => pd.Type), 0);
      if (declaringType.AllMutableConstructors.Any (ctor => signature.Equals (MethodSignature.Create (ctor))))
        throw new ArgumentException ("Constructor with equal signature already exists.", "parameterDeclarations");

      var parameterExpressions = parameterDescriptors.Select (pd => pd.Expression);
      var context = new ConstructorBodyCreationContext (declaringType, parameterExpressions, _memberSelector);
      var body = BodyProviderUtility.GetTypedBody (typeof (void), bodyProvider, context);

      var descriptor = UnderlyingConstructorInfoDescriptor.Create (attributes, parameterDescriptors, body);
      var constructor = new MutableConstructorInfo (declaringType, descriptor);

      return constructor;
    }

    public MutableMethodInfo CreateMutableMethod (
        MutableType declaringType,
        string name,
        MethodAttributes attributes,
        Type returnType,
        IEnumerable<ParameterDeclaration> parameterDeclarations,
        Func<MethodBodyCreationContext, Expression> bodyProvider,
        Action notifyMethodWasImplemented)
    {
      // TODO XXXX: if it is an implicit method override, it needs the same visibility (or more public visibility?)!
      // TODO 5099: add check attributes to be virtual if also abstract
      // TODO 5099: check bodyProvider for null if attributes doesn't contain Abstract flag
      // bodyProvider is null for abstract methods

      var invalidAttributes = new[] { MethodAttributes.PinvokeImpl, MethodAttributes.RequireSecObject, MethodAttributes.UnmanagedExport };
      CheckForInvalidAttributes ("method", invalidAttributes, attributes);

      var isVirtual = attributes.IsSet (MethodAttributes.Virtual);
      var isNewSlot = attributes.IsSet (MethodAttributes.NewSlot);
      if (!isVirtual && isNewSlot)
        throw new ArgumentException ("NewSlot methods must also be virtual.", "attributes");

      var parameterDescriptors = UnderlyingParameterInfoDescriptor.CreateFromDeclarations (parameterDeclarations).ConvertToCollection();

      var signature = new MethodSignature (returnType, parameterDescriptors.Select (pd => pd.Type), 0);
      // Fix code duplication?
      if (declaringType.AllMutableMethods.Any (m => m.Name == name && signature.Equals (MethodSignature.Create (m))))
      {
        var message = string.Format ("Method '{0}' with equal signature already exists.", name);
        throw new ArgumentException (message, "name");
      }

      var baseMethod = isVirtual && !isNewSlot ? _relatedMethodFinder.GetMostDerivedVirtualMethod (name, signature, declaringType.BaseType) : null;
      if (baseMethod != null)
        CheckNotFinalForOverride (baseMethod);

      var parameterExpressions = parameterDescriptors.Select (pd => pd.Expression);
      var isStatic = attributes.IsSet (MethodAttributes.Static);
      var context = new MethodBodyCreationContext (declaringType, parameterExpressions, isStatic, baseMethod, _memberSelector);
      var body = bodyProvider == null ? null : BodyProviderUtility.GetTypedBody (returnType, bodyProvider, context);

      var descriptor = UnderlyingMethodInfoDescriptor.Create (
          name, attributes, returnType, parameterDescriptors, baseMethod, false, false, false, body);
      var method = new MutableMethodInfo (declaringType, descriptor, notifyMethodWasImplemented);

      return method;
    }

    public MutableMethodInfo CreateMutableMethodOverride (MutableType declaringType, MethodInfo method, Action notifyMethodWasImplemented)
    {
      // TODO 4972: Use TypeEqualityComparer (for Equals and IsSubclassOf)
      if (!declaringType.UnderlyingSystemType.Equals (method.DeclaringType) && !declaringType.IsSubclassOf (method.DeclaringType))
      {
        var message = string.Format ("Method is declared by a type outside of this type's class hierarchy: '{0}'.", method.DeclaringType.Name);
        throw new ArgumentException (message, "method");
      }

      if (!method.IsVirtual)
        throw new NotSupportedException ("A method declared in a base type must be virtual in order to be modified.");

      var baseDefinition = method.GetBaseDefinition();
      var existingMutableOverride = _relatedMethodFinder.GetOverride (baseDefinition, declaringType.AllMutableMethods);
      if (existingMutableOverride != null)
        return existingMutableOverride;

      var methods = declaringType.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      var needsExplicitOverride = _relatedMethodFinder.IsShadowed (baseDefinition, methods);
      var baseMethod = _relatedMethodFinder.GetMostDerivedOverride (baseDefinition, declaringType.BaseType);
      CheckNotFinalForOverride (baseMethod);

      var name = needsExplicitOverride ? MethodOverrideUtility.GetNameForExplicitOverride (baseMethod) : baseMethod.Name;
      var attributes = needsExplicitOverride
                           ? MethodOverrideUtility.GetAttributesForExplicitOverride (baseMethod)
                           : MethodOverrideUtility.GetAttributesForImplicitOverride (baseMethod);
      var returnType = baseMethod.ReturnType;
      var parameterDeclarations = ParameterDeclaration.CreateForEquivalentSignature (baseMethod).ConvertToCollection();
      var bodyProvider = baseMethod.IsAbstract
                             ? null
                             : new Func<MethodBodyCreationContext, Expression> (
                                   ctx => ctx.GetBaseCall (baseMethod, ctx.Parameters.Cast<Expression>()));

      var addedOverride = CreateMutableMethod (
          declaringType, name, attributes, returnType, parameterDeclarations, bodyProvider, notifyMethodWasImplemented);
      if (needsExplicitOverride)
        addedOverride.AddExplicitBaseDefinition (baseDefinition);

      return addedOverride;
    }

    private void CheckForInvalidAttributes (string memberKind, MethodAttributes[] invalidAttributes, MethodAttributes attributes)
    {
      var hasInvalidAttributes = invalidAttributes.Any (x => attributes.IsSet (x));
      if (hasInvalidAttributes)
      {
        var invalidAttributeList = string.Join (", ", invalidAttributes.Select (x => Enum.GetName (typeof (MethodAttributes), x)).ToArray());
        var message = string.Format ("The following MethodAttributes are not supported for {0}s: {1}.", memberKind, invalidAttributeList);
        throw new ArgumentException (message, "attributes");
      }
    }

    private void CheckNotFinalForOverride (MethodInfo overridenMethod)
    {
      if (overridenMethod.IsFinal)
      {
        var message = string.Format ("Cannot override final method '{0}.{1}'.", overridenMethod.DeclaringType.Name, overridenMethod.Name);
        throw new NotSupportedException (message);
      }
    }
  }
}