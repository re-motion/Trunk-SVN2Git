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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class AttributeIntroductionDefinitionBuilder
  {
    private readonly IAttributeIntroductionTargetDefinition _target;
    private readonly List<AttributeDefinition> _suppressors;

    public AttributeIntroductionDefinitionBuilder (IAttributeIntroductionTargetDefinition target)
    {
      _target = target;
      _suppressors = new List<AttributeDefinition> ();
    }

    public List<AttributeDefinition> Suppressors
    {
      get { return _suppressors; }
    }

    public IAttributeIntroductionTargetDefinition Target
    {
      get { return _target; }
    }

    public void AddPotentialSuppressors (IEnumerable<AttributeDefinition> attributes)
    {
      foreach (AttributeDefinition attribute in attributes)
      {
        if (typeof (SuppressAttributesAttribute).IsAssignableFrom (attribute.AttributeType))
          _suppressors.Add (attribute);
      }
    }

    public void Apply (IAttributableDefinition attributeSource)
    {
      foreach (AttributeDefinition attribute in attributeSource.CustomAttributes)
      {
        if (ShouldBeIntroduced (attribute))
        {
          AttributeDefinition suppressor = GetSuppressor (attribute);
          if (suppressor == null)
            _target.IntroducedAttributes.Add (new AttributeIntroductionDefinition (_target, attribute));
          else
            _target.SuppressedIntroducedAttributes.Add (new SuppressedAttributeIntroductionDefinition (_target, attribute, suppressor));
        }
      }
    }

    public bool ShouldBeIntroduced (AttributeDefinition attribute)
    {
      return (AttributeUtility.IsAttributeInherited (attribute.AttributeType) || attribute.IsCopyTemplate)
          && (AttributeUtility.IsAttributeAllowMultiple (attribute.AttributeType) || !_target.CustomAttributes.ContainsKey (attribute.AttributeType));
    }

    public AttributeDefinition GetSuppressor (AttributeDefinition attribute)
    {
      ICustomAttributeProvider declaringEntity = attribute.DeclaringDefinition.DeclaringEntity;
      foreach (AttributeDefinition suppressor in _suppressors)
      {
        SuppressAttributesAttribute suppressorInstance = (SuppressAttributesAttribute)suppressor.Instance;
        ICustomAttributeProvider suppressingEntity = suppressor.DeclaringDefinition.DeclaringEntity;
        if (suppressorInstance.IsSuppressed (attribute.AttributeType, declaringEntity, suppressingEntity))
          return suppressor;
      }
      return null;
    }
  }
}
