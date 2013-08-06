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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class MixinTypeGenerator : IMixinTypeGenerator
  {
    private static readonly ConstructorInfo s_debuggerDisplayAttributeConstructor = 
        typeof (DebuggerDisplayAttribute).GetConstructor (new[] { typeof (string) });

    private readonly ICodeGenerationModule _module;
    private readonly ConcreteMixinTypeIdentifier _identifier;

    private readonly IClassEmitter _emitter;
    private readonly FieldReference _identifierField;

    public MixinTypeGenerator (
        ICodeGenerationModule module, 
        ConcreteMixinTypeIdentifier identifier, 
        IConcreteMixinTypeNameProvider nameProvider)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);

      Assertion.IsFalse (identifier.MixinType.ContainsGenericParameters);

      _module = module;
      _identifier = identifier;

      string typeName = nameProvider.GetNameForConcreteMixinType (identifier);
      typeName = CustomClassEmitter.FlattenTypeName (typeName);

      var interfaces = new[] { typeof (ISerializable), typeof (IGeneratedMixinType) };
      const TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

      bool forceUnsigned = !ReflectionUtility.IsReachableFromSignedAssembly (identifier.MixinType);
      _emitter = _module.CreateClassEmitter (typeName, identifier.MixinType, interfaces, flags, forceUnsigned);

      _identifierField = _emitter.CreateStaticField ("__identifier", typeof (ConcreteMixinTypeIdentifier));
    }

    public ConcreteMixinTypeIdentifier Identifier
    {
      get { return _identifier; }
    }

    public IClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public bool IsAssemblySigned
    {
      get { return ReflectionUtility.IsAssemblySigned (Emitter.TypeBuilder.Assembly); }
    }

    public ConcreteMixinType GetBuiltType ()
    {
      GenerateTypeFeatures ();
      OverrideInterfaceGenerator overrideInterfaceGenerator = GenerateOverrides ();
      Dictionary<MethodInfo, MethodInfo> methodWrappers = GenerateMethodWrappers ();

      Type generatedType = Emitter.BuildType();
      Type generatedOverrideInterface = overrideInterfaceGenerator.GetBuiltType();
      
      Dictionary<MethodInfo, MethodInfo> interfaceMethodsForOverriddenMethods = TranslateMethodBuildersToActualMethods(
          overrideInterfaceGenerator.GetInterfaceMethodsForOverriddenMethods (), 
          generatedOverrideInterface);

      return new ConcreteMixinType (
          _identifier, 
          generatedType, 
          generatedOverrideInterface,
          interfaceMethodsForOverriddenMethods,
          methodWrappers);
    }

    protected virtual void GenerateTypeFeatures ()
    {
      AddTypeInitializer ();

      _emitter.ReplicateBaseTypeConstructors (delegate { }, delegate { });

      ImplementGetObjectData();

      AddMixinTypeAttribute();
      AddDebuggerAttributes();
    }

    private void AddTypeInitializer ()
    {
      var typeInitializerEmitter = _emitter.CreateTypeConstructor ();

      var identifierSerializer = new CodeGenerationConcreteMixinTypeIdentifierSerializer (typeInitializerEmitter.CodeBuilder);
      _identifier.Serialize (identifierSerializer);
      typeInitializerEmitter.CodeBuilder.AddStatement (
          new AssignStatement (
              _identifierField,
              identifierSerializer.GetConstructorInvocationExpression ()));

      typeInitializerEmitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    protected virtual Dictionary<MethodInfo, MethodInfo> GenerateMethodWrappers ()
    {
      var wrappers = from m in _identifier.Overriders
                     where !m.IsPublic
                     select new { Method = m, Wrapper = Emitter.GetPublicMethodWrapper (m) };
      return wrappers.ToDictionary (pair => pair.Method, pair => pair.Wrapper);
    }

    private void AddMixinTypeAttribute ()
    {
      CustomAttributeBuilder attributeBuilder = ConcreteMixinTypeAttributeUtility.CreateAttributeBuilder (_identifier);
      Emitter.AddCustomAttribute (attributeBuilder);
    }

    private void AddDebuggerAttributes ()
    {
      string debuggerString = "Derived mixin: " + _identifier;
      var debuggerAttribute = new CustomAttributeBuilder (s_debuggerDisplayAttributeConstructor, new object[] { debuggerString });
      Emitter.AddCustomAttribute (debuggerAttribute);
    }

    protected virtual OverrideInterfaceGenerator GenerateOverrides ()
    {
      var overrideInterfaceGenerator = OverrideInterfaceGenerator.CreateNestedGenerator (Emitter, "IOverriddenMethods");

      PropertyReference targetReference = GetTargetReference();
      foreach (MethodInfo method in _identifier.Overridden)
      {
        var methodOverride = _emitter.CreateMethodOverride (method);
        var methodToCall = overrideInterfaceGenerator.AddOverriddenMethod (method);
          
        AddCallToOverrider (methodOverride, targetReference, methodToCall);
      }

      return overrideInterfaceGenerator;
    }

    private PropertyReference GetTargetReference()
    {
      PropertyInfo targetProperty = MixinReflector.GetTargetProperty (Emitter.TypeBuilder.BaseType);
      if (targetProperty == null)
      {
        throw new NotSupportedException (
            "The code generator does not support mixins with overridden methods or non-public overriders if the mixin doesn't derive from the "
            + "generic Mixin base classes.");
      }

      return new PropertyReference (SelfReference.Self, targetProperty);
    }

    private void AddCallToOverrider (IMethodEmitter methodOverride, Reference targetReference, MethodInfo targetMethod)
    {
      LocalReference castTargetLocal = methodOverride.DeclareLocal (targetMethod.DeclaringType);
      methodOverride.AddStatement (
          new AssignStatement (
              castTargetLocal,
              new CastClassExpression (targetMethod.DeclaringType, targetReference.ToExpression())));

      methodOverride.ImplementByDelegating (castTargetLocal, targetMethod);
    }

    private void ImplementGetObjectData ()
    {
      SerializationImplementer.ImplementGetObjectDataByDelegation (
          Emitter,
          (newMethod, baseIsISerializable) =>
          new MethodInvocationExpression (
              null,
              typeof (MixinSerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
              newMethod.ArgumentReferences[0].ToExpression (),
              newMethod.ArgumentReferences[1].ToExpression (),
              SelfReference.Self.ToExpression (),
              _identifierField.ToExpression (),
              new ReferenceExpression (new ConstReference (!baseIsISerializable))));
    }

    private Dictionary<MethodInfo, MethodInfo> TranslateMethodBuildersToActualMethods (
        Dictionary<MethodInfo, MethodBuilder> methodBuilderDictionary, 
        Type typeToGetMethodsFrom)
    {
      // since the overrideInterfaceGenerator only knows
      var actualMethodsByToken = typeToGetMethodsFrom.GetMethods ().ToDictionary (m => m.MetadataToken);
      return methodBuilderDictionary.ToDictionary (pair => pair.Key, pair => actualMethodsByToken[pair.Value.GetToken().Token]);
    }
  }
}
