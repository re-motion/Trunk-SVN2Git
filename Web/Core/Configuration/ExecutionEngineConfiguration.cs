// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Xml.Serialization;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Configuration
{

/// <summary> Configuration section entry for configuring the <b>Remotion.Web.ExecutionEngine</b>. </summary>
/// <include file='doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/Class/*' />
[XmlType (Namespace = WebConfiguration.SchemaUri)]
public class ExecutionEngineConfiguration
{
  private int _functionTimeout = 20;
  private bool _enableSessionManagement = true;
  private int _refreshInterval = 10;
  private string _urlMappingFile = string.Empty;
  private int _maximumUrlLength = 1024;
  private string _defaultWxeHandler = string.Empty;

  /// <summary> Gets or sets the default timeout for individual functions within one session. </summary>
  /// <value> The timeout in mintues. Defaults to 20 minutes. Must greater than zero. </value>
  [XmlAttribute ("functionTimeout")]
  public int FunctionTimeout
  {
    get 
    {
      return _functionTimeout; 
    }
    set
    {
      if (value < 1)
        throw new ArgumentException ("The FunctionTimeout must be greater than zero.");
      _functionTimeout = value; 
    }
  }

  /// <summary> Gets or sets a flag that determines whether session management is employed. </summary>
  /// <include file='doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/EnableSessionManagement/*' />
  [XmlAttribute ("enableSessionManagement")]
  public bool EnableSessionManagement
  {
    get { return _enableSessionManagement; }
    set { _enableSessionManagement = value; }
  }

  /// <summary> Gets or sets the default refresh intervall for a function. </summary>
  /// <include file='doc\include\Configuration\ExecutionEngineConfiguration.xml' path='ExecutionEngineConfiguration/RefreshInterval/*' />
  [XmlAttribute ("refreshInterval")]
  public int RefreshInterval
  {
    get
    {
      return _refreshInterval; 
    }
    set
    {
      if (value < 0)
        throw new ArgumentException ("The RefreshInterval must not be a negative number.");
      _refreshInterval = value; 
    }
  }

  /// <summary> Gets or sets the path to the file holding the URL mapping configuration. </summary>
  /// <value> A string. Defaults to an empty string. </value>
  [XmlAttribute ("urlMappingFile")]
  public string UrlMappingFile
  {
    get { return _urlMappingFile; }
    set { _urlMappingFile = StringUtility.NullToEmpty (value); }
  }

  /// <summary> Gets or sets the maximum length of URLs generated by the Execution Engine. </summary>
  /// <value> An integer. Defaults to 1024. </value>
  [XmlAttribute ("maximumUrlLength")]
  public int MaximumUrlLength
  {
    get { return _maximumUrlLength; }
    set { _maximumUrlLength = value; }
  }

  /// <summary> Gets or sets the path to the default <see cref="WxeHandler"/>. </summary>
  /// <remarks> If not set, either a mapping is required or the function must be executed by a WxePage. </remarks>
  /// <value> 
  ///   A virtual path, relative to the application root. Will always start with <c>~/</c>. 
  ///   Defaults to an empty string if no handler is assigned. 
  /// </value>
  [XmlAttribute ("defaultWxeHandler")]
  public string DefaultWxeHandler
  {
    get { return _defaultWxeHandler; }
    set 
    {
      if (StringUtility.IsNullOrEmpty (value))
      {
        _defaultWxeHandler = string.Empty;
      }
      else
      {
        value = value.Trim();
        ArgumentUtility.CheckNotNullOrEmpty ("value", value);
        if (value.StartsWith ("/") || value.IndexOf (":") != -1)
          throw new ArgumentException (string.Format ("No absolute paths are allowed. Resource: '{0}'", value), "value");
        if (! value.StartsWith ("~/"))
          value = "~/" + value;
        _defaultWxeHandler = value; 
      }
    }
  }

}

}
