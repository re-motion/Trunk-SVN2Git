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
using System.Reflection;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;
using BT1Mixin1=Remotion.UnitTests.Mixins.SampleTypes.BT1Mixin1;
using System.Linq;

namespace Remotion.UnitTests.Mixins
{
  public static class DefinitionObjectMother
  {
    public static TargetClassDefinition CreateTargetClassDefinition (Type classType, params Type[] mixinTypes)
    {
      var result = new TargetClassDefinition (new ClassContext (classType, mixinTypes));
      foreach (var type in mixinTypes)
        CreateMixinDefinition (result, type);
      return result;
    }

    public static MixinDefinition CreateMixinDefinition (TargetClassDefinition targetClassDefinition, Type mixinType)
    {
      var mixinDefinition = new MixinDefinition (MixinKind.Used, mixinType, targetClassDefinition, true);
      PrivateInvoke.InvokeNonPublicMethod (targetClassDefinition.Mixins, "Add", mixinDefinition);
      return mixinDefinition;
    }

    public static MixinDependencyDefinition CreateMixinDependencyDefinition(MixinDefinition definition)
    {
      var mixinDependency = new MixinDependencyDefinition (new RequiredMixinTypeDefinition (definition.TargetClass, typeof (IBaseType2)), definition, null);
      PrivateInvoke.InvokeNonPublicMethod (definition.MixinDependencies, "Add", mixinDependency);
      return mixinDependency;
    }

    public static BaseDependencyDefinition CreateBaseDependencyDefinition (MixinDefinition definition)
    {
      var baseDependency = new BaseDependencyDefinition (new RequiredBaseCallTypeDefinition (definition.TargetClass, typeof (IBaseType2)), definition, null);
      PrivateInvoke.InvokeNonPublicMethod (definition.BaseDependencies, "Add", baseDependency);
      return baseDependency;
    }

    public static ThisDependencyDefinition CreateThisDependencyDefinition (MixinDefinition definition)
    {
      var thisDependency = new ThisDependencyDefinition (new RequiredFaceTypeDefinition (definition.TargetClass, typeof (IBaseType2)), definition, null);
      PrivateInvoke.InvokeNonPublicMethod (definition.ThisDependencies, "Add", thisDependency);
      return thisDependency;
    }

    public static SuppressedAttributeIntroductionDefinition CreateSuppressedAttributeIntroductionDefinition (MixinDefinition definition)
    {
      var attributeDefinitionFake = CreateAttributeDefinition (definition);
      var suppressedAttributeIntroduction = new SuppressedAttributeIntroductionDefinition (MockRepository.GenerateMock<IAttributeIntroductionTarget>(), attributeDefinitionFake, attributeDefinitionFake);
      PrivateInvoke.InvokeNonPublicMethod (definition.SuppressedAttributeIntroductions, "Add", suppressedAttributeIntroduction);
      return suppressedAttributeIntroduction;
    }

    public static NonAttributeIntroductionDefinition CreateNonAttributeIntroductionDefinition (MixinDefinition definition)
    {
      var attributeDefinitionFake = CreateAttributeDefinition (definition);
      var nonAttributeIntroduction = new NonAttributeIntroductionDefinition (attributeDefinitionFake, true);
      PrivateInvoke.InvokeNonPublicMethod (definition.NonAttributeIntroductions, "Add", nonAttributeIntroduction);
      return nonAttributeIntroduction;
    }

    public static AttributeIntroductionDefinition CreateAttributeIntroductionDefinition (MixinDefinition definition)
    {
      var attributeDefinitionFake = CreateAttributeDefinition(definition);
      var attributeIntroduction = new AttributeIntroductionDefinition (MockRepository.GenerateMock<IAttributeIntroductionTarget>(), attributeDefinitionFake);
      PrivateInvoke.InvokeNonPublicMethod (definition.AttributeIntroductions, "Add", attributeIntroduction);
      return attributeIntroduction;
    }

    public static AttributeDefinition CreateAttributeDefinition(IAttributableDefinition declaringDefinition)
    {
      var attributeDefinition = new AttributeDefinition (declaringDefinition, CustomAttributeData.GetCustomAttributes (typeof (MixinDefinitionTest))[0], true);
      PrivateInvoke.InvokeNonPublicMethod (declaringDefinition.CustomAttributes, "Add", attributeDefinition);
      return attributeDefinition;
    }

    public static NonInterfaceIntroductionDefinition CreateNonInterfaceIntroductionDefinition (MixinDefinition definition)
    {
      var nonInterfaceIntroduction = new NonInterfaceIntroductionDefinition (typeof (IBT1Mixin1), definition, true);
      PrivateInvoke.InvokeNonPublicMethod (definition.NonInterfaceIntroductions, "Add", nonInterfaceIntroduction);
      return nonInterfaceIntroduction;
    }

    public static InterfaceIntroductionDefinition CreateInterfaceIntroductionDefinition (MixinDefinition definition)
    {
      var interfaceIntroduction = new InterfaceIntroductionDefinition (typeof (IBT1Mixin1), definition);
      PrivateInvoke.InvokeNonPublicMethod (definition.InterfaceIntroductions, "Add", interfaceIntroduction);
      return interfaceIntroduction;
    }

    public static MethodDefinition CreateMethodDefinition (ClassDefinitionBase declaringClass, MethodInfo methodInfo)
    {
      var methodDefinition = new MethodDefinition (methodInfo, declaringClass);
      PrivateInvoke.InvokeNonPublicMethod (declaringClass.Methods, "Add", methodDefinition);
      return methodDefinition;
    }

    public static PropertyDefinition CreatePropertyDefinition (ClassDefinitionBase declaringClass, PropertyInfo propertyInfo)
    {
      var getMethod = propertyInfo.CanRead ? new MethodDefinition (propertyInfo.GetGetMethod(true), declaringClass) : null;
      var setMethod = propertyInfo.CanWrite ? new MethodDefinition (propertyInfo.GetSetMethod(true), declaringClass) : null;
      var propertyDefinition = new PropertyDefinition (propertyInfo, declaringClass, getMethod, setMethod);
      PrivateInvoke.InvokeNonPublicMethod (declaringClass.Properties, "Add", propertyDefinition);
      return propertyDefinition;
    }

    public static EventDefinition CreateEventDefinition (ClassDefinitionBase declaringClass, EventInfo eventInfo)
    {
      var addMethod = eventInfo.GetAddMethod (true) != null ? new MethodDefinition (eventInfo.GetAddMethod (true), declaringClass) : null;
      var removeMethod = eventInfo.GetRemoveMethod (true) != null ? new MethodDefinition (eventInfo.GetRemoveMethod (true), declaringClass) : null;
      var eventDefinition = new EventDefinition (eventInfo, declaringClass, addMethod, removeMethod);
      PrivateInvoke.InvokeNonPublicMethod (declaringClass.Events, "Add", eventDefinition);
      return eventDefinition;
    }

    public static void DeclareOverride (MemberDefinitionBase memberOverride, MemberDefinitionBase overriddenMember)
    {
      typeof (MemberDefinitionBase).GetProperty ("BaseAsMember").SetValue (memberOverride, overriddenMember, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
      var overridesCollection = PrivateInvoke.GetNonPublicField (overriddenMember.Overrides, "_items");
      PrivateInvoke.InvokeNonPublicMethod (overridesCollection, "Add", memberOverride);
    }

    public static TargetClassDefinition GetActiveTargetClassDefinition (Type type)
    {
      return GetActiveTargetClassDefinition (type, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    public static TargetClassDefinition GetActiveTargetClassDefinition (Type type, GenerationPolicy generationPolicy)
    {
      var classContext = TargetClassDefinitionUtility.GetContext (
          type, 
          MixinConfiguration.ActiveConfiguration, 
          generationPolicy);
      return TargetClassDefinitionCache.Current.GetTargetClassDefinition (classContext);
    }
  }
}
