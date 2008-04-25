using System;
using System.Xml.Serialization;

namespace Remotion.Web.Configuration
{

/// <summary> Enumeration listing the possible WCAG conformance levels. </summary>
[Flags]
public enum WaiConformanceLevel
{
  /// <summary> The web application is not required to follow the WAI guidelines. </summary>
  Undefined = 0,
  /// <summary> WAI conformance level A, all Priority 1 checkpoints are satisfied. </summary>
  A = 1, 
  /// <summary> WAI conformance level Double-A, all Priority 1 and 2 checkpoints are satisfied. </summary>
  DoubleA = 3,
  /// <summary> WAI conformance level Triple-A, all Priority 1, 2, and 3 checkpoints are satisfied. </summary>
  TripleA = 7
}

/// <summary> Enumeration listing the possible modes for debugging WCG conformance. </summary>
public enum WcagDebugMode
{
  Disabled,
  Logging,
  Exception
}

/// <summary> Configuration section entry for specifying the application wide WAI level. </summary>
/// <include file='doc\include\Configuration\WcagConfiguration.xml' path='WcagConfiguration/Class/*' />
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class WcagConfiguration
{
  private WaiConformanceLevel _conformanceLevel = WaiConformanceLevel.Undefined;
  private WcagDebugMode _debugging = WcagDebugMode.Disabled;

  /// <summary> Gets or sets the WCAG conformance level required in this web-application. </summary>
  /// <value> A value of the <see cref="WaiConformanceLevel"/> enumeration. Defaults to <see cref="WaiConformanceLevel.Undefined"/>. </value>
  [XmlAttribute ("conformanceLevel")]
  public WaiConformanceLevel ConformanceLevel
  {
    get { return _conformanceLevel; }
    set { _conformanceLevel = value; }
  }

  /// <summary>
  ///   Gets or sets a value specifying if and how the developer will be notified on WAI compliancy issues in the 
  ///   controls' configuration.
  /// </summary>
  /// <include file='doc\include\Configuration\WcagConfiguration.xml' path='WcagConfiguration/Debugging/*' />
  [XmlAttribute ("debugging")]
  public WcagDebugMode Debugging
  {
    get { return _debugging; }
    set { _debugging = value; }
  }
}

}
