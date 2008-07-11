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
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  public class ValidatingVisitor : IDefinitionVisitor
  {
    private readonly IValidationLog _validationLog;

    private readonly List<IValidationRule<TargetClassDefinition>> _targetClassRules = new List<IValidationRule<TargetClassDefinition>> ();
    private readonly List<IValidationRule<MixinDefinition>> _mixinRules = new List<IValidationRule<MixinDefinition>> ();
    private readonly List<IValidationRule<InterfaceIntroductionDefinition>> _interfaceIntroductionRules = new List<IValidationRule<InterfaceIntroductionDefinition>> ();
    private readonly IList<IValidationRule<SuppressedInterfaceIntroductionDefinition>> _suppressedInterfaceIntroductionRules = new List<IValidationRule<SuppressedInterfaceIntroductionDefinition>> ();
    private readonly List<IValidationRule<MethodIntroductionDefinition>> _methodIntroductionRules = new List<IValidationRule<MethodIntroductionDefinition>> ();
    private readonly List<IValidationRule<PropertyIntroductionDefinition>> _propertyIntroductionRules = new List<IValidationRule<PropertyIntroductionDefinition>> ();
    private readonly List<IValidationRule<EventIntroductionDefinition>> _eventIntroductionRules = new List<IValidationRule<EventIntroductionDefinition>> ();
    private readonly List<IValidationRule<MethodDefinition>> _methodRules = new List<IValidationRule<MethodDefinition>> ();
    private readonly List<IValidationRule<PropertyDefinition>> _propertyRules = new List<IValidationRule<PropertyDefinition>> ();
    private readonly List<IValidationRule<EventDefinition>> _eventRules = new List<IValidationRule<EventDefinition>> ();
    private readonly List<IValidationRule<RequiredFaceTypeDefinition>> _requiredFaceTypeRules = new List<IValidationRule<RequiredFaceTypeDefinition>> ();
    private readonly List<IValidationRule<RequiredBaseCallTypeDefinition>> _requiredBaseCallTypeRules = new List<IValidationRule<RequiredBaseCallTypeDefinition>> ();
    private readonly List<IValidationRule<RequiredMixinTypeDefinition>> _requiredMixinTypeRules = new List<IValidationRule<RequiredMixinTypeDefinition>> ();
    private readonly List<IValidationRule<RequiredMethodDefinition>> _requiredMethodRules = new List<IValidationRule<RequiredMethodDefinition>> ();
    private readonly List<IValidationRule<ThisDependencyDefinition>> _thisDependencyRules = new List<IValidationRule<ThisDependencyDefinition>> ();
    private readonly List<IValidationRule<BaseDependencyDefinition>> _baseDependencyRules = new List<IValidationRule<BaseDependencyDefinition>> ();
    private readonly List<IValidationRule<MixinDependencyDefinition>> _mixinDependencyRules = new List<IValidationRule<MixinDependencyDefinition>> ();
    private readonly List<IValidationRule<AttributeDefinition>> _attributeRules = new List<IValidationRule<AttributeDefinition>> ();
    private readonly List<IValidationRule<AttributeIntroductionDefinition>> _attributeIntroductionRules = new List<IValidationRule<AttributeIntroductionDefinition>> ();
    private readonly List<IValidationRule<SuppressedAttributeIntroductionDefinition>> _suppressedAttributeIntroductionRules = new List<IValidationRule<SuppressedAttributeIntroductionDefinition>> ();

    public ValidatingVisitor(IValidationLog validationLog)
    {
      ArgumentUtility.CheckNotNull ("validationLog", validationLog);
      _validationLog = validationLog;
    }

    public IList<IValidationRule<TargetClassDefinition>> TargetClassRules
    {
      get { return _targetClassRules; }
    }

    public IList<IValidationRule<MixinDefinition>> MixinRules
    {
      get { return _mixinRules; }
    }

    public IList<IValidationRule<InterfaceIntroductionDefinition>> InterfaceIntroductionRules
    {
      get { return _interfaceIntroductionRules; }
    }

    public IList<IValidationRule<SuppressedInterfaceIntroductionDefinition>> SuppressedInterfaceIntroductionRules
    {
      get { return _suppressedInterfaceIntroductionRules; }
    }

    public IList<IValidationRule<MethodIntroductionDefinition>> MethodIntroductionRules
    {
      get { return _methodIntroductionRules; }
    }

    public IList<IValidationRule<PropertyIntroductionDefinition>> PropertyIntroductionRules
    {
      get { return _propertyIntroductionRules; }
    }

    public IList<IValidationRule<EventIntroductionDefinition>> EventIntroductionRules
    {
      get { return _eventIntroductionRules; }
    }

    public IList<IValidationRule<MethodDefinition>> MethodRules
    {
      get { return _methodRules; }
    }

    public IList<IValidationRule<PropertyDefinition>> PropertyRules
    {
      get { return _propertyRules; }
    }

    public IList<IValidationRule<EventDefinition>> EventRules
    {
      get { return _eventRules; }
    }

    public IList<IValidationRule<RequiredFaceTypeDefinition>> RequiredFaceTypeRules
    {
      get { return _requiredFaceTypeRules; }
    }

    public IList<IValidationRule<RequiredBaseCallTypeDefinition>> RequiredBaseCallTypeRules
    {
      get { return _requiredBaseCallTypeRules; }
    }

    public IList<IValidationRule<RequiredMixinTypeDefinition>> RequiredMixinTypeRules
    {
      get { return _requiredMixinTypeRules; }
    }

    public IList<IValidationRule<RequiredMethodDefinition>> RequiredMethodRules
    {
      get { return _requiredMethodRules; }
    }

    public IList<IValidationRule<ThisDependencyDefinition>> ThisDependencyRules
    {
      get { return _thisDependencyRules; }
    }

    public IList<IValidationRule<BaseDependencyDefinition>> BaseDependencyRules
    {
      get { return _baseDependencyRules; }
    }

    public IList<IValidationRule<MixinDependencyDefinition>> MixinDependencyRules
    {
      get { return _mixinDependencyRules; }
    }

    public IList<IValidationRule<AttributeDefinition>> AttributeRules
    {
      get { return _attributeRules; }
    }

    public IList<IValidationRule<AttributeIntroductionDefinition>> AttributeIntroductionRules
    {
      get { return _attributeIntroductionRules; }
    }

    public IList<IValidationRule<SuppressedAttributeIntroductionDefinition>> SuppressedAttributeIntroductionRules
    {
      get { return _suppressedAttributeIntroductionRules; }
    }

    public void Visit (TargetClassDefinition targetClass)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);
      CheckRules (_targetClassRules, targetClass);
    }

    public void Visit (MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      CheckRules (_mixinRules, mixin);
    }

    public void Visit (InterfaceIntroductionDefinition interfaceIntroduction)
    {
      ArgumentUtility.CheckNotNull ("interfaceIntroduction", interfaceIntroduction);
      CheckRules (_interfaceIntroductionRules, interfaceIntroduction);
    }

    public void Visit (SuppressedInterfaceIntroductionDefinition suppressedInterfaceIntroduction)
    {
      ArgumentUtility.CheckNotNull ("suppressedInterfaceIntroduction", suppressedInterfaceIntroduction);
      CheckRules (_suppressedInterfaceIntroductionRules, suppressedInterfaceIntroduction);
    }

    public void Visit (MethodIntroductionDefinition methodIntroduction)
    {
      ArgumentUtility.CheckNotNull ("methodIntroduction", methodIntroduction);
      CheckRules (_methodIntroductionRules, methodIntroduction);
    }

    public void Visit (PropertyIntroductionDefinition propertyIntroduction)
    {
      ArgumentUtility.CheckNotNull ("propertyIntroduction", propertyIntroduction);
      CheckRules (_propertyIntroductionRules, propertyIntroduction);
    }

    public void Visit (EventIntroductionDefinition eventIntroduction)
    {
      ArgumentUtility.CheckNotNull ("eventIntroduction", eventIntroduction);
      CheckRules (_eventIntroductionRules, eventIntroduction);
    }

    public void Visit (MethodDefinition method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      CheckRules (_methodRules, method);
    }

    public void Visit (PropertyDefinition property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      CheckRules (_propertyRules, property);
    }

    public void Visit (EventDefinition eventDefinition)
    {
      ArgumentUtility.CheckNotNull ("event", eventDefinition);
      CheckRules (_eventRules, eventDefinition);
    }

    public void Visit (RequiredFaceTypeDefinition requiredFaceType)
    {
      ArgumentUtility.CheckNotNull ("requiredFaceType", requiredFaceType);
      CheckRules (_requiredFaceTypeRules, requiredFaceType);
    }

    public void Visit (RequiredBaseCallTypeDefinition requiredBaseCallType)
    {
      ArgumentUtility.CheckNotNull ("requiredBaseCallType", requiredBaseCallType);
      CheckRules (_requiredBaseCallTypeRules, requiredBaseCallType);
    }

    public void Visit (RequiredMixinTypeDefinition requiredMixinType)
    {
      ArgumentUtility.CheckNotNull ("requiredMixinType", requiredMixinType);
      CheckRules (_requiredMixinTypeRules, requiredMixinType);
    }

    public void Visit (RequiredMethodDefinition requiredMethod)
    {
      ArgumentUtility.CheckNotNull ("requiredMethod", requiredMethod);
      CheckRules (_requiredMethodRules, requiredMethod);
    }

    public void Visit (ThisDependencyDefinition dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      CheckRules (_thisDependencyRules, dependency);
    }

    public void Visit (BaseDependencyDefinition dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      CheckRules (_baseDependencyRules, dependency);
    }

    public void Visit (MixinDependencyDefinition dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      CheckRules (_mixinDependencyRules, dependency);
    }

    public void Visit (AttributeDefinition attribute)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      CheckRules (_attributeRules, attribute);
    }

    public void Visit (AttributeIntroductionDefinition attributeIntroduction)
    {
      ArgumentUtility.CheckNotNull ("attributeIntroduction", attributeIntroduction);
      CheckRules (_attributeIntroductionRules, attributeIntroduction);
    }

    public void Visit (SuppressedAttributeIntroductionDefinition suppressedAttributeIntroduction)
    {
      ArgumentUtility.CheckNotNull ("suppressedAttributeIntroduction", suppressedAttributeIntroduction);
      CheckRules (_suppressedAttributeIntroductionRules, suppressedAttributeIntroduction);
    }

    private void CheckRules<TDefinition> (IEnumerable<IValidationRule<TDefinition>> rules, TDefinition definition) where TDefinition : IVisitableDefinition
    {
      _validationLog.ValidationStartsFor (definition);
      foreach (IValidationRule<TDefinition> rule in rules)
      {
        try
        {
          rule.Execute (this, definition, _validationLog);
        }
        catch (Exception ex)
        {
          _validationLog.UnexpectedException (rule, ex);
        }
      }
      _validationLog.ValidationEndsFor (definition);
    }
  }
}
