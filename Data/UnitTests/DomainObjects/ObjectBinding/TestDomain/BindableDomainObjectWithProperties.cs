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
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [Instantiable]
  public abstract class BindableDomainObjectWithProperties : BindableBaseDomainObject
  {
    [StorageClassNone]
    public bool RequiredPropertyNotInMapping { get { return true; } }
    [StorageClassNone]
    public bool? NonRequiredPropertyNotInMapping { get { return true; } }

    protected abstract string ProtectedStringProperty { get; }


    [StringProperty (IsNullable = false)]
    public abstract string RequiredStringProperty { get; set; }
    [StringProperty (IsNullable = true)]
    public abstract string NonRequiredStringProperty { get; set; }

    public abstract int RequiredValueProperty { get; set; }
    public abstract int? NonRequiredValueProperty { get; set; }

    [Mandatory]
    public abstract OppositeAnonymousBindableDomainObject RequiredRelatedObjectProperty { get; set; }
    public abstract OppositeAnonymousBindableDomainObject NonRequiredRelatedObjectProperty { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("OppositeRequiredRelatedObject")]
    public abstract OppositeBidirectionalBindableDomainObject RequiredBidirectionalRelatedObjectProperty { get; set; }
    [DBBidirectionalRelation ("OppositeNonRequiredRelatedObject")]
    public abstract OppositeBidirectionalBindableDomainObject NonRequiredBidirectionalRelatedObjectProperty { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("OppositeRequiredRelatedObjects")]
    public abstract ObjectList<OppositeBidirectionalBindableDomainObject> RequiredBidirectionalRelatedObjectsProperty { get; }
    [DBBidirectionalRelation ("OppositeNonRequiredRelatedObjects")]
    public abstract ObjectList<OppositeBidirectionalBindableDomainObject> NonRequiredBidirectionalRelatedObjectsProperty { get; }




    [StorageClassNone]
    public string NoMaxLengthStringPropertyNotInMapping { get { return ""; } set {Dev.Null = value; } }
    
    [StringProperty (MaximumLength = 7)]
    public abstract string MaxLength7StringProperty { get; set; }
    public abstract string NoMaxLengthStringProperty { get; set; }

    


    [StringProperty (MaximumLength = 33)]
    [DBColumn ("NewBasePropertyWithMaxLength3")]
    public new virtual string BasePropertyWithMaxLength3
    {
      get { return CurrentProperty.GetValue<string> (); }
      set { CurrentProperty.SetValue (value); }
    }
  }
}
