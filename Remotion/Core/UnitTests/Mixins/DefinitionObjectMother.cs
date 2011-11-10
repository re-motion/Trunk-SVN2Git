// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.UnitTests.Mixins.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins
{
  /// <summary>
  /// Contains utility methods to retrieve and instantiate <see cref="TargetClassDefinition"/> and related objects.
  /// </summary>
  public static class DefinitionObjectMother
  {
    public static TargetClassDefinition CreateTargetClassDefinition (Type classType, params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);

      var result = new TargetClassDefinition (new ClassContext (classType, mixinTypes));
      foreach (var type in mixinTypes)
        CreateMixinDefinition (result, type);
      return result;
    }

    public static MixinDefinition CreateMixinDefinition (TargetClassDefinition targetClassDefinition, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      var mixinDefinition = new MixinDefinition (MixinKind.Used, mixinType, targetClassDefinition, true);
      PrivateInvoke.InvokeNonPublicMethod (targetClassDefinition.Mixins, "Add", mixinDefinition);
      return mixinDefinition;
    }

    public static MixinDefinition CreateMixinDefinition (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      var targetClassDefinition = CreateTargetClassDefinition (typeof (NullTarget));
      return CreateMixinDefinition (targetClassDefinition, mixinType);
    }

    public static MixinDependencyDefinition CreateMixinDependencyDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var mixinDependency = new MixinDependencyDefinition (new RequiredMixinTypeDefinition (definition.TargetClass, typeof (IBaseType2)), definition, null);
      PrivateInvoke.InvokeNonPublicMethod (definition.MixinDependencies, "Add", mixinDependency);
      return mixinDependency;
    }

    public static NextCallDependencyDefinition CreateNextCallDependencyDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var nextCallDependency = new NextCallDependencyDefinition (new RequiredNextCallTypeDefinition (definition.TargetClass, typeof (IBaseType2)), definition, null);
      PrivateInvoke.InvokeNonPublicMethod (definition.NextCallDependencies, "Add", nextCallDependency);
      return nextCallDependency;
    }

    public static TargetCallDependencyDefinition CreateTargetCallDependencyDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var targetCallDependency = new TargetCallDependencyDefinition (new RequiredTargetCallTypeDefinition (definition.TargetClass, typeof (IBaseType2)), definition, null);
      PrivateInvoke.InvokeNonPublicMethod (definition.TargetCallDependencies, "Add", targetCallDependency);
      return targetCallDependency;
    }

    public static RequiredTargetCallTypeDefinition CreateRequiredTargetCallTypeDefinition (TargetClassDefinition definition, Type requiredType)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("requiredType", requiredType);

      var requiredTargetCallType = new RequiredTargetCallTypeDefinition (definition, requiredType);
      PrivateInvoke.InvokeNonPublicMethod (definition.RequiredTargetCallTypes, "Add", requiredTargetCallType);
      return requiredTargetCallType;
    }

    public static RequiredNextCallTypeDefinition CreateRequiredNextCallTypeDefinition (TargetClassDefinition definition, Type requiredType)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("requiredType", requiredType);

      var requiredNextCallType = new RequiredNextCallTypeDefinition (definition, requiredType);
      PrivateInvoke.InvokeNonPublicMethod (definition.RequiredNextCallTypes, "Add", requiredNextCallType);
      return requiredNextCallType;
    }

    public static RequiredMixinTypeDefinition CreateRequiredMixinTypeDefinition (TargetClassDefinition definition, Type requiredType)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("requiredType", requiredType);

      var requiredMixinType = new RequiredMixinTypeDefinition (definition, requiredType);
      PrivateInvoke.InvokeNonPublicMethod (definition.RequiredMixinTypes, "Add", requiredMixinType);
      return requiredMixinType;
    }

    public static SuppressedAttributeIntroductionDefinition CreateSuppressedAttributeIntroductionDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var attributeDefinitionFake = CreateAttributeDefinition (definition);
      var suppressedAttributeIntroduction = new SuppressedAttributeIntroductionDefinition (MockRepository.GenerateMock<IAttributeIntroductionTarget>(), attributeDefinitionFake, attributeDefinitionFake);
      PrivateInvoke.InvokeNonPublicMethod (definition.SuppressedAttributeIntroductions, "Add", suppressedAttributeIntroduction);
      return suppressedAttributeIntroduction;
    }

    public static NonAttributeIntroductionDefinition CreateNonAttributeIntroductionDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var attributeDefinitionFake = CreateAttributeDefinition (definition);
      var nonAttributeIntroduction = new NonAttributeIntroductionDefinition (attributeDefinitionFake, true);
      PrivateInvoke.InvokeNonPublicMethod (definition.NonAttributeIntroductions, "Add", nonAttributeIntroduction);
      return nonAttributeIntroduction;
    }

    public static AttributeIntroductionDefinition CreateAttributeIntroductionDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var attributeDefinitionFake = CreateAttributeDefinition(definition);
      var attributeIntroduction = new AttributeIntroductionDefinition (MockRepository.GenerateMock<IAttributeIntroductionTarget>(), attributeDefinitionFake);
      PrivateInvoke.InvokeNonPublicMethod (definition.AttributeIntroductions, "Add", attributeIntroduction);
      return attributeIntroduction;
    }

    public static AttributeDefinition CreateAttributeDefinition(IAttributableDefinition declaringDefinition)
    {
      ArgumentUtility.CheckNotNull ("declaringDefinition", declaringDefinition);

      var attributeData = CustomAttributeData.GetCustomAttributes (typeof (BaseType1))
          .Where (a => a.Constructor.DeclaringType == typeof (BT1Attribute))
          .Single();

      var attributeDefinition = new AttributeDefinition (declaringDefinition, attributeData, true);
      PrivateInvoke.InvokeNonPublicMethod (declaringDefinition.CustomAttributes, "Add", attributeDefinition);
      return attributeDefinition;
    }

    public static NonInterfaceIntroductionDefinition CreateNonInterfaceIntroductionDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var nonInterfaceIntroduction = new NonInterfaceIntroductionDefinition (typeof (IBT1Mixin1), definition, true);
      PrivateInvoke.InvokeNonPublicMethod (definition.NonInterfaceIntroductions, "Add", nonInterfaceIntroduction);
      return nonInterfaceIntroduction;
    }

    public static InterfaceIntroductionDefinition CreateInterfaceIntroductionDefinition (MixinDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      var interfaceIntroduction = new InterfaceIntroductionDefinition (typeof (IBT1Mixin1), definition);
      PrivateInvoke.InvokeNonPublicMethod (definition.InterfaceIntroductions, "Add", interfaceIntroduction);
      return interfaceIntroduction;
    }

    public static MethodDefinition CreateMethodDefinition (ClassDefinitionBase declaringClass, MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("declaringClass", declaringClass);
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      var methodDefinition = new MethodDefinition (methodInfo, declaringClass);
      PrivateInvoke.InvokeNonPublicMethod (declaringClass.Methods, "Add", methodDefinition);
      return methodDefinition;
    }

    public static PropertyDefinition CreatePropertyDefinition (ClassDefinitionBase declaringClass, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("declaringClass", declaringClass);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var getMethod = propertyInfo.CanRead ? new MethodDefinition (propertyInfo.GetGetMethod(true), declaringClass) : null;
      var setMethod = propertyInfo.CanWrite ? new MethodDefinition (propertyInfo.GetSetMethod(true), declaringClass) : null;
      var propertyDefinition = new PropertyDefinition (propertyInfo, declaringClass, getMethod, setMethod);
      PrivateInvoke.InvokeNonPublicMethod (declaringClass.Properties, "Add", propertyDefinition);
      return propertyDefinition;
    }

    public static EventDefinition CreateEventDefinition (ClassDefinitionBase declaringClass, EventInfo eventInfo)
    {
      ArgumentUtility.CheckNotNull ("declaringClass", declaringClass);
      ArgumentUtility.CheckNotNull ("eventInfo", eventInfo);

      var addMethod = eventInfo.GetAddMethod (true) != null ? new MethodDefinition (eventInfo.GetAddMethod (true), declaringClass) : null;
      var removeMethod = eventInfo.GetRemoveMethod (true) != null ? new MethodDefinition (eventInfo.GetRemoveMethod (true), declaringClass) : null;
      var eventDefinition = new EventDefinition (eventInfo, declaringClass, addMethod, removeMethod);
      PrivateInvoke.InvokeNonPublicMethod (declaringClass.Events, "Add", eventDefinition);
      return eventDefinition;
    }

    public static void DeclareOverride (MemberDefinitionBase memberOverride, MemberDefinitionBase overriddenMember)
    {
      ArgumentUtility.CheckNotNull ("memberOverride", memberOverride);
      ArgumentUtility.CheckNotNull ("overriddenMember", overriddenMember);

      typeof (MemberDefinitionBase).GetProperty ("BaseAsMember").SetValue (memberOverride, overriddenMember, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
      var overridesCollection = PrivateInvoke.GetNonPublicField (overriddenMember.Overrides, "_items");
      PrivateInvoke.InvokeNonPublicMethod (overridesCollection, "Add", memberOverride);
    }

    public static TargetClassDefinition GetActiveTargetClassDefinition (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (type);

      Assert.That (classContext, Is.Not.Null, "The given type '" + type.Name + "' must be configured as a mixin target.");
      return TargetClassDefinitionFactory.CreateTargetClassDefinition (classContext);
    }

    public static TargetClassDefinition GetActiveTargetClassDefinition_Force (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext classContext = MixinConfiguration.ActiveConfiguration.GetContextForce (type);
      return GetTargetClassDefinition(classContext);
    }

    public static TargetClassDefinition GetTargetClassDefinition (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      return TargetClassDefinitionFactory.CreateTargetClassDefinition (classContext);
    }

    public static TargetClassDefinition BuildUnvalidatedDefinition (Type baseType, params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);

      var context = new ClassContext (baseType, mixinTypes);
      return BuildUnvalidatedDefinition(context);
    }

    public static TargetClassDefinition BuildUnvalidatedDefinition (ClassContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var builder = new TargetClassDefinitionBuilder();
      return builder.Build (context);
    }

    public static void AddRequiringDependency (RequirementDefinitionBase requirement, DependencyDefinitionBase dependency)
    {
      PrivateInvoke.InvokeNonPublicMethod (requirement.RequiringDependencies, "Add", dependency);
    }
  }
}
