using System;
using System.Xml.Serialization;

namespace Remotion.Web.Configuration
{

/// <summary> Configuration section entry for configuring the <b>Remotion.Web.ExecutionEngine</b>. </summary>
/// <include file='doc\include\Configuration\SmartNavigationConfiguration.xml' path='SmartNavigationConfiguration/Class/*' />
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class SmartNavigationConfiguration
{
  private bool _enableScrolling = true;
  private bool _enableFocusing = true;

  /// <summary> Gets or sets a flag that determines whether to enable smart scrolling. </summary>
  /// <value> <see langword="true"/> to enable smart scrolling. Defaults to <see langword="true"/>. </value>
  [XmlAttribute ("enableScrolling")]
  public bool EnableScrolling
  {
    get { return _enableScrolling; }
    set { _enableScrolling = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to enable smart focus. </summary>
  /// <value> <see langword="true"/> to enable smart focusing. Defaults to <see langword="true"/>. </value>
  [XmlAttribute ("enableFocusing")]
  public bool EnableFocusing
  {
    get { return _enableFocusing; }
    set { _enableFocusing = value; }
  }
}

}
