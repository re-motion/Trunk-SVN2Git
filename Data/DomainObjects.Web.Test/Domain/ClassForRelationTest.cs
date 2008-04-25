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