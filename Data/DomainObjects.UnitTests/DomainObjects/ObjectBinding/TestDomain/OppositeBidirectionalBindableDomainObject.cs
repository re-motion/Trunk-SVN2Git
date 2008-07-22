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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [TestDomain]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class OppositeBidirectionalBindableDomainObject : DomainObject
  {
    [DBBidirectionalRelation ("RequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObject { get; set; }
    [DBBidirectionalRelation ("NonRequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObject { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("RequiredBidirectionalRelatedObjectsProperty")]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObjects { get; set; }
    [DBBidirectionalRelation ("NonRequiredBidirectionalRelatedObjectsProperty")]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObjects { get; set; }
  }
}
