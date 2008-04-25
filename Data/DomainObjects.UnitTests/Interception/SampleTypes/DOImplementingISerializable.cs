using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  [Serializable]
  public abstract class DOImplementingISerializable : DomainObject, ISerializable
  {
    private string _memberHeldAsField;

    public DOImplementingISerializable (string memberHeldAsField)
    {
      _memberHeldAsField = memberHeldAsField;
    }

    protected DOImplementingISerializable (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _memberHeldAsField = info.GetString ("_memberHeldAsField") + "-Ctor";
    }

    public abstract int PropertyWithGetterAndSetter { get; set; }

    public string MemberHeldAsField
    {
      get { return _memberHeldAsField; }
      set { _memberHeldAsField = value; }
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("_memberHeldAsField", _memberHeldAsField + "-GetObjectData");
      BaseGetObjectData (info, context);
    }
  }
}