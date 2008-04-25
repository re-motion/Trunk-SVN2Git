using System;

namespace Remotion.Security.Metadata
{
  public class MetadataInfo
  {
    // types

    // static members

    // member fields

    private string _id;
    private string _name;

    // construction and disposing

    // methods and properties

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public virtual string Description
    {
      get { return _name; }
    }
  }
}
