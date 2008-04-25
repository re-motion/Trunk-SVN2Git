using System;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class EnumValueInfo : MetadataInfo
  {
    // types

    // static members

    // member fields

    private int _value;
    private string _typeName;

    // construction and disposing

    public EnumValueInfo (string typeName, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      _value = value;
      _typeName = typeName;
      Name = name;
    }

    // methods and properties

    public int Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public string TypeName
    {
      get
      {
        return _typeName;
      }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("TypeName", value);
        _typeName = value;
      }
    }

    public override string Description
    {
      get { return TypeName; }
    }
  }
}