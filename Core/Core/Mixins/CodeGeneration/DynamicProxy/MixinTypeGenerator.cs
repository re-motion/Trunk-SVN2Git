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
using Remotion.Mixins.Definitions;
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
        typeof (DebuggerDisplayAttribute).GetConstructor (new Type[] { typeof (string) });

    private readonly IModuleManager _module;
    private readonly ITypeGenerator _targetGenerator;
    private readonly MixinDefinition _configuration;
    private readonly IClassEmitter _emitter;
    private readonly FieldReference _configurationField;

    public MixinTypeGenerator (IModuleManager module, ITypeGenerator targetGenerator, MixinDefinition configuration, INameProvider nameProvider)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNull ("targetGenerator", targetGenerator);
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);

      Assertion.IsFalse (configuration.Type.ContainsGenericParameters);

      _module = module;
      _targetGenerator = targetGenerator;
      _configuration = configuration;

      string typeName = nameProvider.GetNewTypeName (configuration);
      typeName = CustomClassEmitter.FlattenTypeName (typeName);

      Type[] interfaces = new[] {typeof (ISerializable), typeof (IGeneratedMixinType)};
      TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

      bool forceUnsigned = !targetGenerator.IsAssemblySigned;
      _emitter = _module.CreateClassEmitter (typeName, configuration.Type, interfaces, flags, forceUnsigned);

      _configurationField = _emitter.CreateStaticField ("__configuration", typeof (MixinDefinition));
    }

    public IClassEmitter Emitter
    {
      get { return _emitter; }
    }

    public MixinDefinition Configuration
    {
      get { return _configuration; }
    }

    public bool IsAssemblySigned
    {
      get { return ReflectionUtility.IsAssemblySigned (Emitter.TypeBuilder.Assembly); }
    }

    public Type GetBuiltType ()
    {
      Generate ();
      Type builtType = Emitter.BuildType ();
      return builtType;
    }

    private void Generate ()
    {
      AddTypeInitializer ();

      _emitter.ReplicateBaseTypeConstructors ();

      ImplementGetObjectData ();

      AddMixinTypeAttribute ();
      AddDebuggerAttributes ();
      ReplicateAttributes (_configuration.CustomAttributes, _emitter);
      ImplementOverrides ();
    }

    private void AddTypeInitializer ()
    {
      ConstructorEmitter emitter = _emitter.CreateTypeConstructor ();

      Expression attributeExpression =
          ConcreteMixinTypeAttributeUtility.CreateNewAttributeExpression (Configuration.MixinIndex, Configuration.TargetClass.ConfigurationContext,
          emitter.CodeBuilder);
      LocalReference attributeLocal = emitter.CodeBuilder.DeclareLocal (typeof (ConcreteMixinTypeAttribute));
      emitter.CodeBuilder.AddStatement (new AssignStatement (attributeLocal, attributeExpression));
      
      MethodInfo getMixinDefinitionMethod = typeof (ConcreteMixinTypeAttribute).GetMethod ("GetMixinDefinition");
      Assertion.IsNotNull (getMixinDefinitionMethod);

      emitter.CodeBuilder.AddStatement (new AssignStatement (_configurationField,
          new VirtualMethodInvocationExpression (attributeLocal, getMixinDefinitionMethod)));

      emitter.CodeBuilder.AddStatement (new ReturnStatement ());
    }

    private void AddMixinTypeAttribute ()
    {
      CustomAttributeBuilder attributeBuilder = ConcreteMixinTypeAttributeUtility.CreateAttributeBuilder (Configuration.MixinIndex,
          Configuration.TargetClass.ConfigurationContext);
      Emitter.AddCustomAttribute (attributeBuilder);
    }

    private void AddDebuggerAttributes ()
    {
      string debuggerString = "Derived mixin: " + _configuration.Type.Name + " on class " + _configuration.TargetClass.Type.Name;
      CustomAttributeBuilder debuggerAttribute = new CustomAttributeBuilder (s_debuggerDisplayAttributeConstructor, new object[] { debuggerString });
      Emitter.AddCustomAttribute (debuggerAttribute);
    }

    private void ImplementOverrides ()
    {
      if (!_configuration.HasOverriddenMembers())
        return;

      PropertyInfo targetProperty = MixinReflector.GetTargetProperty (Emitter.TypeBuilder.BaseType);
      PropertyReference targetReference = new PropertyReference (SelfReference.Self, targetProperty);

      if (targetProperty == null)
      {
        throw new NotSupportedException (
            "The code generator does not support mixin methods being overridden if the mixin doesn't derive from the Mixin base classes.");
      }

      foreach (MethodDefinition method in _configuration.GetAllMethods())
      {
        if (method.Overrides.Count > 1)
          throw new NotSupportedException ("The code generator does not support mixin methods with more than one override.");
        else if (method.Overrides.Count == 1)
        {
          if (method.Overrides[0].DeclaringClass != Configuration.TargetClass)
            throw new NotSupportedException ("The code generator only supports mixin methods to be overridden by the mixin's target class.");

          CustomMethodEmitter methodOverride = _emitter.CreateMethodOverride (method.MethodInfo);
          MethodDefinition overrider = method.Overrides[0];
          MethodInfo methodToCall = GetOverriderMethodToCall (overrider);
          AddCallToOverrider (methodOverride, targetReference, methodToCall);
        }
      }
    }

    private MethodInfo GetOverriderMethodToCall (MethodDefinition overrider)
    {
      if (overrider.MethodInfo.IsPublic)
        return overrider.MethodInfo;
      else
        return _targetGenerator.GetPublicMethodWrapper (overrider);
    }

    private void AddCallToOverrider (CustomMethodEmitter methodOverride, Reference targetReference, MethodInfo targetMethod)
    {
      LocalReference castTargetLocal = methodOverride.DeclareLocal (targetMethod.DeclaringType);
      methodOverride.AddStatement (
          new AssignStatement (
              castTargetLocal,
              new ConvertExpression (targetMethod.DeclaringType, targetReference.ToExpression())));

      methodOverride.ImplementByDelegating (castTargetLocal, targetMethod);
    }

    private void ReplicateAttributes (IEnumerable<AttributeDefinition> attributes, IAttributableEmitter target)
    {
      foreach (AttributeDefinition attribute in attributes)
      {
        // only replicate those attributes from the base which are not inherited anyway
        if (!attribute.IsCopyTemplate && !AttributeUtility.IsAttributeInherited (attribute.AttributeType))
          AttributeReplicator.ReplicateAttribute (target, attribute.Data);
      }
    }

    private void ImplementGetObjectData ()
    {
      SerializationImplementer.ImplementGetObjectDataByDelegation (Emitter, delegate (CustomMethodEmitter newMethod, bool baseIsISerializable)
          {
            return new MethodInvocationExpression (
                null,
                typeof (MixinSerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes"),
                new ReferenceExpression (newMethod.ArgumentReferences[0]),
                new ReferenceExpression (newMethod.ArgumentReferences[1]),
                new ReferenceExpression (SelfReference.Self),
                new ReferenceExpression (_configurationField),
                new ReferenceExpression (new ConstReference (!baseIsISerializable)));
          });
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
