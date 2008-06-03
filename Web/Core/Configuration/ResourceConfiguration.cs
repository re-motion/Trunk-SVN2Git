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
