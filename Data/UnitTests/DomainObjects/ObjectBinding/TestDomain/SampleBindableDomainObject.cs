// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class SampleBindableDomainObject : BindableDomainObject
  {
    public static SampleBindableDomainObject NewObject ()
    {
      return NewObject<SampleBindableDomainObject> ().With ();
    }

    public static SampleBindableDomainObject NewObject (IBindableDomainObjectImplementation implementation)
    {
      return NewObject<SampleBindableDomainObject> ().With (implementation);
    }

    public static SampleBindableDomainObject GetObject (ObjectID id)
    {
      return GetObject<SampleBindableDomainObject> (id);
    }

    protected SampleBindableDomainObject()
    {
    }

    protected SampleBindableDomainObject (IBindableDomainObjectImplementation implementation)
      : base (implementation)
    {
    }
  
    public abstract string Name { get; set; }
    public abstract int Int32 { get; set; }

    [StorageClassNone]
    public PropertyIndexer PublicProperties
    {
      get { return base.Properties; }
    }

    [StorageClassTransaction]
    [DBBidirectionalRelation ("OppositeSampleObject")]
    public abstract OppositeBidirectionalBindableDomainObject RelatedObjectProperty1 { get; set; }
    [StorageClassTransaction]
    [DBBidirectionalRelation ("OppositeSampleObjects")]
    public abstract OppositeBidirectionalBindableDomainObject RelatedObjectProperty2 { get; set; }
  }
}
