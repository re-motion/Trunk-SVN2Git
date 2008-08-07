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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.SampleTypes
{
  public class BaseForMixinAddingPersistentProperties : DomainObjectMixin<DomainObject>
  {
    [DBBidirectionalRelation ("RelationProperty5", ContainsForeignKey = true)]
    private RelationTargetForPersistentMixin PrivateBaseRelationProperty
    {
      get { return Properties[typeof (BaseForMixinAddingPersistentProperties), "PrivateBaseRelationProperty"].GetValue<RelationTargetForPersistentMixin> (); }
      set { Properties[typeof (BaseForMixinAddingPersistentProperties), "PrivateBaseRelationProperty"].SetValue (value); }
    }
  }
}