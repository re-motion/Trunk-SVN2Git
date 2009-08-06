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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
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
    private static readonly MethodInfo s_concreteTypeInitializationMethod =
        typeof (GeneratedClassInstanceInitializer).GetMethod ("InitializeMixinTarget", new[] { typeof (IInitializableMixinTarget), typeof (bool) });

    private static readonly MethodInfo s_initializeMethod = typeof (IInitializableMixinTarget).GetMethod ("Initialize");
    private static readonly MethodInfo s_createBaseCallProxyMethod = typeof (IInitializableMixinTarget).GetMethod ("CreateBaseCallProxy");
    private static readonly MethodInfo s_setFirstBaseCallProxyMethod = typeof (IInitializableMixinTarget).GetMethod ("SetFirstBaseCallProxy");
    private readonly MethodInfo s_setExtensionsMethod = typeof (IInitializableMixinTarget).GetMethod ("SetExtensions");

    private readonly FieldReference _extensionsField;
    private readonly FieldReference _firstField;

    public InitializationCodeGenerator (FieldReference extensionsField, FieldReference firstField)
    {
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("firstField", firstField);
      
      _extensionsField = extensionsField;
      _firstField = firstField;
    }

    public Statement GetInitializationStatement ()
    {
      var initializationMethodCall = new ExpressionStatement (
          new VirtualMethodInvocationExpression (
              new TypeReferenceWrapper (SelfReference.Self, typeof (IInitializableMixinTarget)),
              s_initializeMethod,
              new ConstReference (false).ToExpression()));

      var condition = new SameConditionExpression (_extensionsField.ToExpression(), NullExpression.Instance);
      return new IfStatement (condition, initializationMethodCall);
    }

    public void ImplementIInitializableMixinTarget (IClassEmitter classEmitter, BaseCallProxyGenerator baseCallProxyGenerator)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);
      ArgumentUtility.CheckNotNull ("baseCallProxyGenerator", baseCallProxyGenerator);

      ImplementInitializeMethod (classEmitter);

      CustomMethodEmitter createProxyMethod =
          classEmitter.CreateInterfaceMethodImplementation (s_createBaseCallProxyMethod);
      createProxyMethod.ImplementByReturning (new NewInstanceExpression(baseCallProxyGenerator.Ctor,
          SelfReference.Self.ToExpression(), createProxyMethod.ArgumentReferences[0].ToExpression()));

      CustomMethodEmitter setProxyMethod =
          classEmitter.CreateInterfaceMethodImplementation (s_setFirstBaseCallProxyMethod);
      setProxyMethod.AddStatement (new AssignStatement (_firstField, 
          new ConvertExpression(baseCallProxyGenerator.TypeBuilder, setProxyMethod.ArgumentReferences[0].ToExpression ())));
      setProxyMethod.ImplementByReturningVoid ();

      CustomMethodEmitter setExtensionsMethod =
          classEmitter.CreateInterfaceMethodImplementation (s_setExtensionsMethod);
      setExtensionsMethod.AddStatement (new AssignStatement (_extensionsField, setExtensionsMethod.ArgumentReferences[0].ToExpression ()));
      setExtensionsMethod.ImplementByReturningVoid ();
    }

    private void ImplementInitializeMethod (IClassEmitter classEmitter)
    {
      CustomMethodEmitter createInitializeMethod = classEmitter.CreateInterfaceMethodImplementation (s_initializeMethod);

      //// object[] mixinInstances = MixedObjectInstantiationScope.Current.SuppliedMixinInstances;

      //var mixinInstancesLocal = createInitializeMethod.DeclareLocal (typeof (object[]));

      //var currentMixedObjectInstantiationScope = new PropertyReference (typeof (MixedObjectInstantiationScope).GetProperty ("Current"));
      //var suppliedMixinInstances = new PropertyReference (
      //    currentMixedObjectInstantiationScope,
      //    typeof (MixedObjectInstantiationScope).GetProperty ("SuppliedMixinInstances"));

      //createInitializeMethod.AddStatement (new AssignStatement (mixinInstancesLocal, suppliedMixinInstances.ToExpression ()));

      //// ((IInitializableMixinTarget)this).SetFirstBaseCallProxy (((IInitializableMixinTarget)this).CreateBaseCallProxy (0));

      //var selfAsInitializableTarget = new TypeReferenceWrapper (SelfReference.Self, typeof (IInitializableMixinTarget));
      //var firstBaseCallProxyExpression = new VirtualMethodInvocationExpression (
      //    selfAsInitializableTarget,
      //    s_createBaseCallProxyMethod,
      //    new ConstReference (0).ToExpression ());

      //createInitializeMethod.AddStatement (
      //    new ExpressionStatement (
      //        new VirtualMethodInvocationExpression (
      //            selfAsInitializableTarget,
      //            s_setFirstBaseCallProxyMethod,
      //            firstBaseCallProxyExpression)));

      // TODO 1482: Start here <=> GeneratedClassInstanceInitializer.PrepareExtensionsWithGivenMixinInstances

      createInitializeMethod.AddStatement (
          new ExpressionStatement (
              new MethodInvocationExpression (
                  null,
                  s_concreteTypeInitializationMethod,
                  new ConvertExpression (typeof (IInitializableMixinTarget), SelfReference.Self.ToExpression()),
                  createInitializeMethod.ArgumentReferences[0].ToExpression())));
      
      
      createInitializeMethod.AddStatement (new ReturnStatement ());
    }
  }
}