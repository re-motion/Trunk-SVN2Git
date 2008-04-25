using System;
using System.Xml.Serialization;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
/// <summary> Represents the common information all configuration classes provide. </summary>
public class ConfigurationBase
{
  // types

  // static members and constants

  // member fields

  private string _applicationName;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>ConfigurationBase</b> type. </summary>
  protected ConfigurationBase (string applicationName)
  {
    _applicationName = applicationName;
  }

  /// <summary> XML Deserialization contructor. </summary>
  /// <exclude />
  protected ConfigurationBase()
  {
  }

  // methods and properties

  /// <summary> Gets the application name that is specified in the XML configuration file.  </summary>
  [XmlAttribute ("application")]
  public string ApplicationName
  {
    get { return _applicationName; }
  }
}
}
