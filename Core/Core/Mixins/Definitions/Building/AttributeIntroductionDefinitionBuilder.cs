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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class AttributeIntroductionDefinitionBuilder
  {
    private readonly IAttributeIntroductionTargetDefinition _target;

    public AttributeIntroductionDefinitionBuilder (IAttributeIntroductionTargetDefinition target)
    {
      _target = target;
    }

    public void Apply (IAttributableDefinition attributeSource)
    {
      foreach (AttributeDefinition attribute in attributeSource.CustomAttributes)
      {
        if (ShouldBeIntroduced (attribute))
          _target.IntroducedAttributes.Add (new AttributeIntroductionDefinition (_target, attribute));
      }
    }

    private bool ShouldBeIntroduced (AttributeDefinition attribute)
    {
      return (AttributeUtility.IsAttributeInherited (attribute.AttributeType) || attribute.IsCopyTemplate)
          && (AttributeUtility.IsAttributeAllowMultiple (attribute.AttributeType) || !_target.CustomAttributes.ContainsKey (attribute.AttributeType));
    }
  }
}
