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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [Instantiable]
  public abstract class BindableDomainObjectWithProperties : BindableBaseDomainObject, IBindableDomainObjectWithProperties
  {
    public enum Enum
    {
      First,
      Second
    }

    [UndefinedEnumValue (Undefined)]
    public enum UndefinedEnum
    {
      Undefined,
      First,
      Second
    }

    public static BindableDomainObjectWithProperties NewObject ()
    {
      return NewObject<BindableDomainObjectWithProperties>().With();
    }

    [StorageClassNone]
    public bool RequiredPropertyNotInMapping { get { return true; } }
    [StorageClassNone]
    public bool? NonRequiredPropertyNotInMapping { get { return true; } }

    protected abstract string ProtectedStringProperty { get; }


    [StringProperty (IsNullable = false)]
    public abstract string RequiredStringProperty { get; set; }
    [StringProperty (IsNullable = false)]
    public abstract string RequiredStringPropertyInInterface { get; set; }

    [StorageClass (StorageClass.Persistent)]
    string IBindableDomainObjectWithProperties.RequiredStringPropertyExplicitInInterface
    {
      get 
      { 
        return Properties[typeof (BindableDomainObjectWithProperties),
            typeof (IBindableDomainObjectWithProperties).FullName + ".RequiredStringPropertyExplicitInInterface"].GetValue<string>(); 
      }
      set 
      {
        Properties[typeof (BindableDomainObjectWithProperties), 
            typeof (IBindableDomainObjectWithProperties).FullName + ".RequiredStringPropertyExplicitInInterface"].SetValue (value);
      }
    }

    [StringProperty (IsNullable = true)]
    public abstract string NonRequiredStringProperty { get; set; }

    public abstract int RequiredValueProperty { get; set; }
    public abstract int? NonRequiredValueProperty { get; set; }

    public abstract Enum RequiredEnumProperty { get; set; }
    public abstract Enum? NonRequiredEnumProperty { get; set; }
    public abstract UndefinedEnum NonRequiredUndefinedEnumProperty { get; set; }

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
