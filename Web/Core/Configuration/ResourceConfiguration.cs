using System;
using System.Xml.Serialization;

namespace Remotion.Web.Configuration
{

/// <summary> Configuration section entry for specifying the resources root. </summary>
/// <include file='doc\include\Configuration\ResourceConfiguration.xml' path='ResourceConfiguration/Class/*' />
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ResourceConfiguration
{
  private string _root = "res";

  /// <summary> Gets or sets the root folder for all resources. </summary>
  /// <include file='doc\include\Configuration\ResourceConfiguration.xml' path='ResourceConfiguration/Root/*' />
  [XmlAttribute ("root")]
  public string Root
  {
    get { return _root; }
    set { _root = Remotion.Utilities.StringUtility.NullToEmpty(value).Trim ('/'); }
  }

}

}
