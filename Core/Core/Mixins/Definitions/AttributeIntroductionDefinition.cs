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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{FullName} introduced via {Attribute.DeclaringDefinition.FullName}")]
  public class AttributeIntroductionDefinition : IVisitableDefinition
  {
    private readonly IAttributeIntroductionTarget _target;
    private readonly AttributeDefinition _attribute;

    public AttributeIntroductionDefinition (IAttributeIntroductionTarget target, AttributeDefinition attribute)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attribute", attribute);

      _target = target;
      _attribute = attribute;
    }

    public Type AttributeType
    {
      get { return _attribute.AttributeType; }
    }

    public string FullName
    {
      get { return _attribute.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return _attribute.Parent; }
    }

    public IAttributeIntroductionTarget Target
    {
      get { return _target; }
    }

    public AttributeDefinition Attribute
    {
      get { return _attribute; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
    }
  }
}
