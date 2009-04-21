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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [TestDomain]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class OppositeBidirectionalBindableDomainObject : BindableDomainObject
  {
    public static OppositeBidirectionalBindableDomainObject NewObject ()
    {
      return NewObject<OppositeBidirectionalBindableDomainObject> ();
    }

    [DBBidirectionalRelation ("RequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObject { get; set; }
    [DBBidirectionalRelation ("NonRequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObject { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("RequiredBidirectionalRelatedObjectsProperty")]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObjects { get; set; }
    [DBBidirectionalRelation ("NonRequiredBidirectionalRelatedObjectsProperty")]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObjects { get; set; }

    [DBBidirectionalRelation ("RelatedObjectProperty1", ContainsForeignKey = true)]
    public abstract SampleBindableDomainObject OppositeSampleObject { get; set; }
    [DBBidirectionalRelation ("RelatedObjectProperty2")]
    public abstract ObjectList<SampleBindableDomainObject> OppositeSampleObjects { get; set; }
  }
}
