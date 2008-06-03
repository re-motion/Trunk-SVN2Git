/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Remotion.Xml;

namespace Remotion.Web.Configuration
{

/// <summary> The configuration section for <b>Remotion.Web</b>. </summary>
/// <include file='doc\include\Configuration\WebConfiguration.xml' path='WebConfiguration/Class/*' />
[XmlType (WebConfiguration.ElementName, Namespace = WebConfiguration.SchemaUri)]
public class WebConfiguration: IConfigurationSectionHandler
{
  /// <summary> The name of the configuration section in the configuration file. </summary>
  /// <remarks> <c>remotion.web</c> </remarks>
  public const string ElementName = "remotion.web";

  /// <summary> The namespace of the configuration section schema. </summary>
  /// <remarks> <c>http://www.re-motion.org/web/configuration</c> </remarks>
  public const string SchemaUri = "http://www.re-motion.org/web/configuration";

  /// <summary> Gets an <see cref="XmlReader"/> reader for the schema embedded in the assembly. </summary>
  public static XmlReader GetSchemaReader ()
  {
    return new XmlTextReader (Assembly.GetExecutingAssembly().GetManifestResourceStream (typeof(WebConfiguration), "WebConfiguration.xsd"));
  }

  private static WebConfiguration s_current = null;

  /// <summary> Gets the <see cref="WebConfiguration"/>. </summary>
  public static WebConfiguration Current
  {
    get
    {
      if (s_current == null)
      {
        lock (typeof (WebConfiguration))
        {
          if (s_current == null)
          {
            XmlNode section = (XmlNode) ConfigurationManager.GetSection (ElementName);
            if (section != null)
            {
              s_current = (WebConfiguration) XmlSerializationUtility.DeserializeUsingSchema (
                  new XmlNodeReader (section),
                  // "web.config/configuration/" + ElementName,  // TODO: context is no longer supported, verify that node has correct BaseURI
                  typeof (WebConfiguration),
                  SchemaUri, 
                  GetSchemaReader());
            }
            else
            {
              s_current = new WebConfiguration();
            }
          }
        }
      }
      return s_current;
    }
  }

  private ExecutionEngineConfiguration _executionEngine = new ExecutionEngineConfiguration();
  private WcagConfiguration _wcag = new WcagConfiguration();
  private ResourceConfiguration _resources = new ResourceConfiguration();
  private SmartNavigationConfiguration _smartNavigation = new SmartNavigationConfiguration();

  /// <summary> Gets or sets the <see cref="ExecutionEngineConfiguration"/> entry. </summary>
  [XmlElement ("executionEngine")]
  public ExecutionEngineConfiguration ExecutionEngine
  {
    get { return _executionEngine; }
    set { _executionEngine = value; }
  }

  /// <summary> Gets or sets the <see cref="WcagConfiguration"/> entry. </summary>
  [XmlElement ("wcag")]
  public WcagConfiguration Wcag
  {
    get { return _wcag; }
    set { _wcag = value; }
  }

  /// <summary> Gets or sets the <see cref="ResourceConfiguration"/> entry. </summary>
  [XmlElement ("resources")]
  public ResourceConfiguration Resources
  {
    get { return _resources; }
    set { _resources = value; }
  }

  /// <summary> Gets or sets the <see cref="SmartNavigationConfiguration"/> entry. </summary>
  [XmlElement ("smartNavigation")]
  public SmartNavigationConfiguration SmartNavigation
  {
    get { return _smartNavigation; }
    set { _smartNavigation = value; }
  }

  object IConfigurationSectionHandler.Create (object parent, object configContext, XmlNode section)
  {
    // instead of the WebConfiguration instance, the xml node is returned. this prevents version 
    // conflicts when multiple versions of this assembly are loaded within one AppDomain.
    return section;
  }
}


}
