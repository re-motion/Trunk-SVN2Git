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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Generates the initialization code used to initialize a concrete mixed type and its mixins.
  /// </summary>
  public class InitializationCodeGenerator
  {
    private static readonly MethodInfo s_initializeMethod = typeof (IInitializableMixinTarget).GetMethod ("Initialize");
    private static readonly MethodInfo s_initializeAfterDeserializationMethod = typeof (IInitializableMixinTarget).GetMethod ("InitializeAfterDeserialization");

    private static readonly MethodInfo s_createMixinArrayMethod = typeof (MixinArrayInitializer).GetMethod ("CreateMixinArray");
    private static readonly MethodInfo s_checkMixinArrayMethod = typeof (MixinArrayInitializer).GetMethod ("CheckMixinArray");

    private static readonly MethodInfo s_initializeMixinMethod = typeof (IInitializableMixin).GetMethod ("Initialize");

    private readonly TargetClassDefinition _configuration;
    private readonly FieldReference _extensionsField;
    private readonly FieldReference _firstField;
    private readonly FieldReference _classContextField;
    private readonly ConstructorInfo _baseCallProxyCtor;

    public InitializationCodeGenerator (
        TargetClassDefinition configuration, 
        FieldReference extensionsField, 
        FieldReference firstField, 
        FieldReference classContextField,
        ConstructorInfo baseCallProxyCtor)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("firstField", firstField);
      ArgumentUtility.CheckNotNull ("classContextField", classContextField);
      ArgumentUtility.CheckNotNull ("baseCallProxyCtor", baseCallProxyCtor);

      _configuration = configuration;
      _extensionsField = extensionsField;
      _firstField = firstField;
      _classContextField = classContextField;
      _baseCallProxyCtor = baseCallProxyCtor;
    }

    /// <summary>
    /// Gets a <see cref="Statement"/> that causes a <see cref="MixinArrayInitializer"/> to be created and assigned to 
    /// <paramref name="mixinArrayInitializerField"/>.
    /// </summary>
    /// <param name="expectedMixinInfosLocal">A <see cref="LocalReference"/> used as a temporary to hold an array of 
    /// <see cref="MixinArrayInitializer.ExpectedMixinInfo"/>.</param>
    /// <param name="mixinArrayInitializerField">The target field to assign the new <see cref="MixinArrayInitializer"/> to.</param>
    public Statement GetAssignMixinArrayInitializerStatement (LocalReference expectedMixinInfosLocal, FieldReference mixinArrayInitializerField)
    {
      var statements = new List<Statement> ();

      // var expectedMixinInfos = new MixinArrayInitializer.ExpectedMixinInfo[<configuration.Mixins.Count>];
      statements.Add (new AssignStatement (
          expectedMixinInfosLocal, 
          new NewArrayExpression (_configuration.Mixins.Count, typeof (MixinArrayInitializer.ExpectedMixinInfo))));

      for (int i = 0; i < _configuration.Mixins.Count; ++i)
      {
        // expectedMixinInfos[i] = new MixinArrayInitializer.ExpectedMixinInfo (
        //   <configuration.Mixins[i].Type>, <configuration.Mixins[i].NeedsDerivedMixin>)

        var newMixinInfoExpression = new NewInstanceExpression (
            typeof (MixinArrayInitializer.ExpectedMixinInfo),
            new[] { typeof (Type), typeof (bool) },
            new TypeTokenExpression (_configuration.Mixins[i].Type),
            new ConstReference (_configuration.Mixins[i].NeedsDerivedMixinType ()).ToExpression ());
        statements.Add (new AssignArrayStatement (expectedMixinInfosLocal, i, newMixinInfoExpression));
      }

      // <targetField> = MixinArrayInitializer (<configuration.Type>, expectedMixinInfos, <configuration>);

      var newInitializerExpression = new NewInstanceExpression (
          typeof (MixinArrayInitializer), 
          new[] { typeof (Type), typeof (MixinArrayInitializer.ExpectedMixinInfo[]), typeof (ClassContext) },
          new TypeTokenExpression (_configuration.Type),
          expectedMixinInfosLocal.ToExpression(),
          _classContextField.ToExpression());

      statements.Add (new AssignStatement (mixinArrayInitializerField, newInitializerExpression));
      return new BlockStatement (statements.ToArray ());
    }

    /// <summary>
    /// Gets a <see cref="Statement"/> that causes the mixin to initialize itself. This effectively calls the 
    /// <see cref="IInitializableMixinTarget.Initialize"/> method added by <see cref="ImplementIInitializableMixinTarget"/>.
    /// </summary>
    public Statement GetInitializationStatement ()
    {
      // ((IInitializableMixinTarget) this).Initialize ()

      var initializationMethodCall = new ExpressionStatement (
          new VirtualMethodInvocationExpression (
              new TypeReferenceWrapper (SelfReference.Self, typeof (IInitializableMixinTarget)),
              s_initializeMethod));

      var condition = new SameConditionExpression (_extensionsField.ToExpression(), NullExpression.Instance);
      return new IfStatement (condition, initializationMethodCall);
    }

    /// <summary>
    /// Implements the <see cref="IInitializableMixinTarget"/> interface on the given <paramref name="classEmitter"/>.
    /// </summary>
    /// <param name="classEmitter">The class emitter to generate <see cref="IInitializableMixinTarget"/> on.</param>
    /// <param name="mixinArrayInitializerField">The field holding the <see cref="MixinArrayInitializer"/> used to initialize the mixins.
    /// This field can be initialized using <see cref="GetAssignMixinArrayInitializerStatement"/>, e.g. in the generated class' type initializer.</param>
    public void ImplementIInitializableMixinTarget (IClassEmitter classEmitter, FieldReference mixinArrayInitializerField)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);
      ArgumentUtility.CheckNotNull ("mixinArrayInitializerField", mixinArrayInitializerField);

      CustomMethodEmitter initializeMethod = classEmitter.CreateInterfaceMethodImplementation (s_initializeMethod);

      ImplementSettingFirstBaseCallProxy (initializeMethod);
      ImplementCreatingMixinInstances (initializeMethod, mixinArrayInitializerField);
      ImplementInitializingMixins (initializeMethod, new ConstReference (false).ToExpression ());

      initializeMethod.AddStatement (new ReturnStatement ());

      CustomMethodEmitter initializeAfterDeserializationMethod = 
          classEmitter.CreateInterfaceMethodImplementation (s_initializeAfterDeserializationMethod);

      ImplementSettingFirstBaseCallProxy (initializeAfterDeserializationMethod);
      ImplementSettingMixinInstances (initializeAfterDeserializationMethod, mixinArrayInitializerField);
      ImplementInitializingMixins (initializeAfterDeserializationMethod, new ConstReference (true).ToExpression ());

      initializeAfterDeserializationMethod.AddStatement (new ReturnStatement ());
    }

    private void ImplementSettingFirstBaseCallProxy (CustomMethodEmitter initializeMethod)
    {
      // __first = <NewBaseCallProxy (0)>;

      NewInstanceExpression newBaseCallProxyExpression = GetNewBaseCallProxyExpression (0);
      initializeMethod.AddStatement (new AssignStatement (_firstField, newBaseCallProxyExpression));
    }

    private void ImplementCreatingMixinInstances (CustomMethodEmitter initializeMethod, FieldReference mixinArrayInitializerField)
    {
      // __extensions = <mixinArrayInitializerField>.CreateMixinArray (MixedObjectInstantiationScope.Current.SuppliedMixinInstances);

      var currentMixedObjectInstantiationScope = new PropertyReference (null, typeof (MixedObjectInstantiationScope).GetProperty ("Current"));
      var suppliedMixinInstances = new PropertyReference (
          currentMixedObjectInstantiationScope,
          typeof (MixedObjectInstantiationScope).GetProperty ("SuppliedMixinInstances"));
      
      var allMixinInstances = new VirtualMethodInvocationExpression (
          new TypeReferenceWrapper (mixinArrayInitializerField, typeof (MixinArrayInitializer)),
          s_createMixinArrayMethod,
          suppliedMixinInstances.ToExpression());

      initializeMethod.AddStatement (new AssignStatement (_extensionsField, allMixinInstances));
    }

    private void ImplementSettingMixinInstances (CustomMethodEmitter initializeMethod, FieldReference mixinArrayInitializerField)
    {
      // <mixinArrayInitializerField>.CheckMixinArray (<arguments[0]>)
      // __extensions = <arguments[0]>;

      initializeMethod.AddStatement (new ExpressionStatement (new VirtualMethodInvocationExpression (
          new TypeReferenceWrapper (mixinArrayInitializerField, typeof (MixinArrayInitializer)),
          s_checkMixinArrayMethod,
          initializeMethod.ArgumentReferences[0].ToExpression())));

      initializeMethod.AddStatement (new AssignStatement (_extensionsField, initializeMethod.ArgumentReferences[0].ToExpression()));
    }

    private void ImplementInitializingMixins (CustomMethodEmitter initializeMethod, Expression deserialization)
    {
      var initializableMixinLocal = initializeMethod.DeclareLocal (typeof (IInitializableMixin));
      for (int i = 0; i < _configuration.Mixins.Count; ++i)
      {
        if (typeof (IInitializableMixin).IsAssignableFrom (_configuration.Mixins[i].Type))
        {
          // var initializableMixin = (IInitializableMixin) __extensions[i];
          var castMixinExpression = new ConvertExpression (
              typeof (IInitializableMixin),
              typeof (object),
              new LoadArrayElementExpression (i, _extensionsField, typeof (object)));

          initializeMethod.AddStatement (new AssignStatement (initializableMixinLocal, castMixinExpression));

          // initializableMixin.Initialize (mixinTargetInstance, <NewBaseCallProxy (i + 1)>, deserialization);
          initializeMethod.AddStatement (
              new ExpressionStatement (
                  new VirtualMethodInvocationExpression (
                      initializableMixinLocal,
                      s_initializeMixinMethod,
                      SelfReference.Self.ToExpression (),
                      GetNewBaseCallProxyExpression (i + 1),
                      deserialization)));
        }
      }
    }

    private NewInstanceExpression GetNewBaseCallProxyExpression (int depth)
    {
      // new <BaseCallProxy> (this, <depth>)
      return new NewInstanceExpression (
          _baseCallProxyCtor,
          SelfReference.Self.ToExpression (),
          new ConstReference (depth).ToExpression ());
    }
  }
}