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
  [DebuggerDisplay ("{FullName}, not introduced by {Attribute.DeclaringDefinition.FullName}")]
  public class NonAttributeIntroductionDefinition : IVisitableDefinition
  {
    public NonAttributeIntroductionDefinition (AttributeDefinition attribute, bool explicitSuppression)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      Attribute = attribute;
      IsExplicitlySuppressed = explicitSuppression;
    }

    public AttributeDefinition Attribute { get; private set; }
    public bool IsExplicitlySuppressed { get; private set; }

    public Type AttributeType
    {
      get { return Attribute.AttributeType; }
    }

    public bool IsShadowed
    {
      get { return !IsExplicitlySuppressed; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public string FullName
    {
      get { return Attribute.AttributeType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Attribute.Parent; }
    }
  }
}
