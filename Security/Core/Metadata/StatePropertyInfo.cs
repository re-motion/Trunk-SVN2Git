using System;
using System.Collections.Generic;

namespace Remotion.Security.Metadata
{

  public class StatePropertyInfo : MetadataInfo
  {
    // types

    // static members

    // member fields

    private List<EnumValueInfo> _values = new List<EnumValueInfo>();
	
    // construction and disposing

    public StatePropertyInfo ()
    {
    }

    // methods and properties

    public List<EnumValueInfo> Values
    {
      get { return _values; }
      set { _values = value; }
    }
  }

}