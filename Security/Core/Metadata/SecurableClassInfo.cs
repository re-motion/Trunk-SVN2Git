using System;
using System.Collections.Generic;

namespace Remotion.Security.Metadata
{

  public class SecurableClassInfo : MetadataInfo
  {
    // types

    // static members

    // member fields

    private List<StatePropertyInfo> _properties = new List<StatePropertyInfo>();
    private List<EnumValueInfo> _accessTypes = new List<EnumValueInfo>();
    private SecurableClassInfo _baseClass;
    private List<SecurableClassInfo> _derivedClasses = new List<SecurableClassInfo> ();

    // construction and disposing

    public SecurableClassInfo ()
    {
    }

    // methods and properties

    public List<StatePropertyInfo> Properties
    {
      get { return _properties; }
    }

    public List<EnumValueInfo> AccessTypes
    {
      get { return _accessTypes; }
    }

    public SecurableClassInfo BaseClass
    {
      get { return _baseClass; }
      set { _baseClass = value; }
    }

    public List<SecurableClassInfo> DerivedClasses
    {
      get { return _derivedClasses; }
    }
	
  }
}