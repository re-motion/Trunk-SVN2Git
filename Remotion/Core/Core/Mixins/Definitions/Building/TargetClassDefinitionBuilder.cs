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
using Remotion.Collections;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.Text;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions.Building
{
  public class TargetClassDefinitionBuilder
  {
    private readonly DependentObjectSorter<MixinDefinition> _sorter = new DependentObjectSorter<MixinDefinition> (new MixinDependencyAnalyzer());
    private readonly DependentMixinGrouper _grouper = new DependentMixinGrouper();

    public TargetClassDefinitionBuilder ()
    {
    }

    public TargetClassDefinition Build (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      if (classContext.Type.ContainsGenericParameters)
      {
        string message = string.Format ("The base class {0} contains generic parameters. This is not supported.", classContext.Type.FullName);
        throw new ConfigurationException (message);
      }

      TargetClassDefinition classDefinition = new TargetClassDefinition (classContext);

      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      MemberDefinitionBuilder membersBuilder = new MemberDefinitionBuilder(classDefinition, IsVisibleToInheritorsOrExplicitInterfaceImpl,
          bindingFlags);
      membersBuilder.Apply (classDefinition.Type);

      AttributeDefinitionBuilder attributesBuilder = new AttributeDefinitionBuilder (classDefinition);
      attributesBuilder.Apply (classDefinition.Type);

      ApplyExplicitFaceInterfaces(classDefinition, classContext);

      ApplyMixins (classDefinition, classContext);
      ApplyMethodRequirements (classDefinition);

      AnalyzeOverrides (classDefinition);
      AnalyzeAttributeIntroductions (classDefinition);
      return classDefinition;
    }

    private void ApplyExplicitFaceInterfaces (TargetClassDefinition classDefinition, ClassContext classContext)
    {
      foreach (Type faceInterface in classContext.CompleteInterfaces)
        classDefinition.RequiredFaceTypes.Add (new RequiredFaceTypeDefinition (classDefinition, faceInterface));
    }

    private void ApplyMixins (TargetClassDefinition classDefinition, ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      MixinDefinitionBuilder mixinDefinitionBuilder = new MixinDefinitionBuilder (classDefinition);

      using (IEnumerator<MixinContext> enumerator = classContext.Mixins.GetEnumerator())
      {
        for (int i = 0; enumerator.MoveNext (); ++i)
          mixinDefinitionBuilder.Apply (enumerator.Current, i);
      }

      // It's important to have a list before clearing the mixins. If we were working with lazy enumerator streams here, we would clear the input for
      // the sorting algorithm before actually executing it...
      List<MixinDefinition> sortedMixins = SortMixins (classDefinition.Mixins);
      classDefinition.Mixins.Clear();

      using (IEnumerator<MixinDefinition> mixinEnumerator = sortedMixins.GetEnumerator())
      {
        for (int i = 0; mixinEnumerator.MoveNext(); ++i)
        {
          mixinEnumerator.Current.MixinIndex = i;
          classDefinition.Mixins.Add (mixinEnumerator.Current);
        }
      }
    }

    private List<MixinDefinition> SortMixins (IEnumerable<MixinDefinition> unsortedMixins)
    {
      List<List<MixinDefinition>> sortedMixinGroups = new List<List<MixinDefinition>>();
      
      // partition mixins into independent groups
      foreach (Set<MixinDefinition> mixinGroup in _grouper.GroupMixins (unsortedMixins))
      {
        try
        {
          IEnumerable<MixinDefinition> sortedGroup = _sorter.SortDependencies (mixinGroup);
          sortedMixinGroups.Add (new List<MixinDefinition> (sortedGroup));
        }
        catch (CircularDependenciesException<MixinDefinition> ex)
        {
          string message = string.Format ("The following group of mixins contains circular dependencies: {0}.",
              SeparatedStringBuilder.Build (", ", ex.Circulars, delegate (MixinDefinition m) { return m.FullName; }));
          throw new ConfigurationException (message, ex);
        }
      }

      // order groups alphabetically
      sortedMixinGroups.Sort (delegate (List<MixinDefinition> one, List<MixinDefinition> two) { return one[0].FullName.CompareTo (two[0].FullName); });
      
      // flatten ordered groups of sorted mixins
      List<MixinDefinition> result = new List<MixinDefinition>();
      foreach (List<MixinDefinition> mixinGroup in sortedMixinGroups)
      {
        foreach (MixinDefinition mixin in mixinGroup)
          result.Add (mixin);
      }
      return result;
    }

    private void ApplyMethodRequirements (TargetClassDefinition classDefinition)
    {
      RequiredMethodDefinitionBuilder methodRequirementBuilder = new RequiredMethodDefinitionBuilder (classDefinition);
      foreach (RequirementDefinitionBase requirement in classDefinition.RequiredFaceTypes)
        methodRequirementBuilder.Apply (requirement);

      foreach (RequirementDefinitionBase requirement in classDefinition.RequiredBaseCallTypes)
        methodRequirementBuilder.Apply (requirement);
    }

    private void AnalyzeOverrides (TargetClassDefinition definition)
    {
      OverridesAnalyzer<MethodDefinition> methodAnalyzer = new OverridesAnalyzer<MethodDefinition> (typeof (OverrideMixinAttribute), definition.GetAllMixinMethods());
      foreach (Tuple<MethodDefinition, MethodDefinition> methodOverride in methodAnalyzer.Analyze (definition.Methods))
        InitializeOverride (methodOverride.A, methodOverride.B);

      OverridesAnalyzer<PropertyDefinition> propertyAnalyzer = new OverridesAnalyzer<PropertyDefinition> (typeof (OverrideMixinAttribute), definition.GetAllMixinProperties());
      foreach (Tuple<PropertyDefinition, PropertyDefinition> propertyOverride in propertyAnalyzer.Analyze (definition.Properties))
        InitializeOverride (propertyOverride.A, propertyOverride.B);

      OverridesAnalyzer<EventDefinition> eventAnalyzer = new OverridesAnalyzer<EventDefinition> (typeof (OverrideMixinAttribute), definition.GetAllMixinEvents());
      foreach (Tuple<EventDefinition, EventDefinition> eventOverride in eventAnalyzer.Analyze (definition.Events))
        InitializeOverride (eventOverride.A, eventOverride.B);
    }

    private void InitializeOverride (MemberDefinitionBase overrider, MemberDefinitionBase baseMember)
    {
      overrider.BaseAsMember = baseMember;
      baseMember.AddOverride (overrider);
    }

    private bool IsVisibleToInheritorsOrExplicitInterfaceImpl (MethodInfo method)
    {
      return ReflectionUtility.IsPublicOrProtectedOrExplicit (method);
    }

    private void AnalyzeAttributeIntroductions (TargetClassDefinition classDefinition)
    {
      AttributeIntroductionDefinitionBuilder builder = new AttributeIntroductionDefinitionBuilder (classDefinition);
      builder.AddPotentialSuppressors (classDefinition.CustomAttributes);
      foreach (MixinDefinition mixin in classDefinition.Mixins)
        builder.AddPotentialSuppressors (mixin.CustomAttributes);
      foreach (MixinDefinition mixin in classDefinition.Mixins)
        builder.Apply (mixin);

      AnalyzeMemberAttributeIntroductions(classDefinition);
    }

    private void AnalyzeMemberAttributeIntroductions (TargetClassDefinition classDefinition)
    {
      const AttributeTargets memberTargets = AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field;
      Assertion.IsTrue ((AttributeUtility.GetAttributeUsage (typeof (SuppressAttributesAttribute)).ValidOn & memberTargets) == 0, 
          "must be updated with AddPotentialSuppressors once SuppressAttributesAttribute supports members");

      foreach (MemberDefinitionBase member in classDefinition.GetAllMembers ())
      {
        if (member.Overrides.Count != 0)
        {
          AttributeIntroductionDefinitionBuilder introductionBuilder = new AttributeIntroductionDefinitionBuilder (member);
          foreach (MemberDefinitionBase overrider in member.Overrides)
            introductionBuilder.Apply (overrider);
        }
      }
    }
  }
}

