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
using Remotion.Collections;
using Remotion.Mixins.Context;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions.Building
{
  public class MixinDefinitionBuilder
  {
    private readonly TargetClassDefinition _targetClass;
    private readonly RequirementsAnalyzer _faceRequirementsAnalyzer; 
    private readonly RequirementsAnalyzer _baseRequirementsAnalyzer;
    private readonly AttributeIntroductionDefinitionBuilder _attributeIntroductionBuilder;

    public MixinDefinitionBuilder (TargetClassDefinition targetClass)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);
      _targetClass = targetClass;
      _faceRequirementsAnalyzer = new RequirementsAnalyzer (targetClass, typeof (ThisAttribute));
      _baseRequirementsAnalyzer = new RequirementsAnalyzer (targetClass, typeof (BaseAttribute));
      _attributeIntroductionBuilder = new AttributeIntroductionDefinitionBuilder (_targetClass);
    }

    public TargetClassDefinition TargetClass
    {
      get { return _targetClass; }
    }

    public void Apply (MixinContext mixinContext, int index)
    {
      ArgumentUtility.CheckNotNull ("mixinContext", mixinContext);
      ArgumentUtility.CheckNotNull ("index", index);

      MixinDefinition mixin = CreateMixinDefinition(mixinContext);

      AnalyzeMembers(mixin);
      AnalyzeAttributes(mixin);
      AnalyzeAttributeIntroductions(mixin);
      AnalyzeInterfaceIntroductions (mixin, mixinContext.IntroducedMemberVisibility);
      AnalyzeOverrides (mixin);
      AnalyzeDependencies(mixin, mixinContext.ExplicitDependencies);
    }

    private MixinDefinition CreateMixinDefinition (MixinContext mixinContext)
    {
      Type mixinType = TargetClass.MixinTypeInstantiator.GetClosedMixinType (mixinContext.MixinType);
      bool acceptsAlphabeticOrdering = AcceptsAlphabeticOrdering (mixinType);
      MixinDefinition mixin = new MixinDefinition (mixinContext.MixinKind, mixinType, TargetClass, acceptsAlphabeticOrdering);
      TargetClass.Mixins.Add (mixin);
      return mixin;
    }

    private bool AcceptsAlphabeticOrdering (Type mixinType)
    {
      return mixinType.IsDefined (typeof (AcceptsAlphabeticOrderingAttribute), false);
    }

    private void AnalyzeMembers (MixinDefinition mixin)
    {
      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      MemberDefinitionBuilder membersBuilder = new MemberDefinitionBuilder (mixin, IsVisibleToInheritorsOrExplicitInterfaceImpl, bindingFlags);
      membersBuilder.Apply (mixin.Type);
    }

    private bool IsVisibleToInheritorsOrExplicitInterfaceImpl (MethodInfo method)
    {
      return ReflectionUtility.IsPublicOrProtectedOrExplicit (method);
    }

    private void AnalyzeAttributes (MixinDefinition mixin)
    {
      AttributeDefinitionBuilder attributesBuilder = new AttributeDefinitionBuilder (mixin);
      attributesBuilder.Apply (mixin.Type);
    }

    private void AnalyzeAttributeIntroductions (MixinDefinition mixin)
    {
      _attributeIntroductionBuilder.Apply (mixin);
    }
    
    private void AnalyzeInterfaceIntroductions (MixinDefinition mixin, MemberVisibility defaultVisibility)
    {
      InterfaceIntroductionDefinitionBuilder introductionBuilder = new InterfaceIntroductionDefinitionBuilder (mixin, defaultVisibility);
      introductionBuilder.Apply ();
    }

    private void AnalyzeOverrides (MixinDefinition mixin)
    {
      OverridesAnalyzer<MethodDefinition> methodAnalyzer = new OverridesAnalyzer<MethodDefinition> (typeof (OverrideTargetAttribute), _targetClass.Methods);
      foreach (Tuple<MethodDefinition, MethodDefinition> methodOverride in methodAnalyzer.Analyze (mixin.Methods))
        InitializeOverride (methodOverride.A, methodOverride.B);

      OverridesAnalyzer<PropertyDefinition> propertyAnalyzer = new OverridesAnalyzer<PropertyDefinition> (typeof (OverrideTargetAttribute), _targetClass.Properties);
      foreach (Tuple<PropertyDefinition, PropertyDefinition> propertyOverride in propertyAnalyzer.Analyze (mixin.Properties))
        InitializeOverride (propertyOverride.A, propertyOverride.B);

      OverridesAnalyzer<EventDefinition> eventAnalyzer = new OverridesAnalyzer<EventDefinition> (typeof (OverrideTargetAttribute), _targetClass.Events);
      foreach (Tuple<EventDefinition, EventDefinition> eventOverride in eventAnalyzer.Analyze (mixin.Events))
        InitializeOverride (eventOverride.A, eventOverride.B);

      AnalyzeMemberAttributeIntroductions (mixin);
    }

    private void InitializeOverride (MemberDefinition overrider, MemberDefinition baseMember)
    {
      overrider.BaseAsMember = baseMember;
      if (baseMember.Overrides.ContainsKey (overrider.DeclaringClass.Type))
      {
        string message = string.Format ("Mixin {0} overrides method {1} twice: {2} and {3} both target the same method.",
            overrider.DeclaringClass.FullName, baseMember.FullName, overrider.FullName, baseMember.Overrides[overrider.DeclaringClass.Type].FullName);
        throw new ConfigurationException (message);
      }
      baseMember.AddOverride (overrider);
    }

    private void AnalyzeMemberAttributeIntroductions (MixinDefinition mixin)
    {
      foreach (MemberDefinition mixinMember in mixin.GetAllMembers ())
      {
        if (mixinMember.BaseAsMember != null)
        {
          AttributeIntroductionDefinitionBuilder introductionBuilder = new AttributeIntroductionDefinitionBuilder (mixinMember.BaseAsMember);
          introductionBuilder.Apply (mixinMember);
        }
      }
    }

    private void AnalyzeDependencies (MixinDefinition mixin, IEnumerable<Type> additionalDependencies)
    {
      ThisDependencyDefinitionBuilder thisDependencyBuilder = new ThisDependencyDefinitionBuilder (mixin);
      thisDependencyBuilder.Apply (_faceRequirementsAnalyzer.Analyze (mixin));

      BaseDependencyDefinitionBuilder baseDependencyBuilder = new BaseDependencyDefinitionBuilder (mixin);
      baseDependencyBuilder.Apply (_baseRequirementsAnalyzer.Analyze (mixin));
      
      MixinDependencyDefinitionBuilder mixinDependencyBuilder = new MixinDependencyDefinitionBuilder (mixin);
      mixinDependencyBuilder.Apply (additionalDependencies);
    }
  }
}
