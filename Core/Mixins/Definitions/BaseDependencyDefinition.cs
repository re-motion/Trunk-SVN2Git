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

namespace Remotion.Mixins.Definitions
{
  public class BaseDependencyDefinition : DependencyDefinitionBase
  {
    public BaseDependencyDefinition (RequiredBaseCallTypeDefinition requiredType, MixinDefinition depender, BaseDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public override ClassDefinitionBase GetImplementer ()
    {
      ClassDefinitionBase implementer = base.GetImplementer ();
      // check for duck interface
      if (implementer == null && !RequiredType.IsEmptyInterface)
      {
        implementer = Depender.TargetClass; // duck interface
      }
      return implementer;
    }

    public new RequiredBaseCallTypeDefinition RequiredType
    {
      get { return (RequiredBaseCallTypeDefinition) base.RequiredType; }
    }
  }
}
