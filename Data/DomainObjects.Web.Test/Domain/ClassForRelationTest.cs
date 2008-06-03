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
using Remotion.ObjectBinding;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Globalization;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [MultiLingualResources ("Remotion.Data.DomainObjects.Web.Test.Globalization.ClassForRelationTest")]
  [Serializable]
  [DBTable ("TableForRelationTest")]
  [Instantiable]
  [DBStorageGroup]
  public abstract class ClassForRelationTest: BindableDomainObject
  {
    public static ClassForRelationTest NewObject()
    {
      return DomainObject.NewObject<ClassForRelationTest> ().With ();
    }


    public ClassForRelationTest()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public override string DisplayName
    {
      get { return Name; }
    }

    [StorageClassNone]
    public ClassWithAllDataTypes.EnumType EnumProperty
    {
      get { return ClassWithAllDataTypes.EnumType.Value3; }
    }

    [ItemType (typeof (ClassWithAllDataTypes))]
    [ObjectBinding (ReadOnly = true)]
    [StorageClassNone]
    public DomainObjectCollection ComputedList
    {
      get { return new DomainObjectCollection(); }
    }

    [DBColumn ("TableWithAllDataTypesMandatory")]
    [DBBidirectionalRelation ("ClassesForRelationTestMandatoryNavigateOnly")]
    [Mandatory]
    public abstract ClassWithAllDataTypes ClassWithAllDataTypesMandatory {get; set;}

    [DBColumn ("TableWithAllDataTypesOptional")]
    [DBBidirectionalRelation ("ClassesForRelationTestOptionalNavigateOnly")]
    public abstract ClassWithAllDataTypes ClassWithAllDataTypesOptional { get; set;}

    [DBBidirectionalRelation ("ClassForRelationTestMandatory")]
    [Mandatory]
    [ObjectBinding (ReadOnly = true)]
    public abstract ObjectList<ClassWithAllDataTypes> ClassesWithAllDataTypesMandatoryNavigateOnly { get; }

    [DBBidirectionalRelation ("ClassForRelationTestOptional")]
    [ObjectBinding (ReadOnly = true)]
    public abstract ObjectList<ClassWithAllDataTypes> ClassesWithAllDataTypesOptionalNavigateOnly { get; }
  }
}
