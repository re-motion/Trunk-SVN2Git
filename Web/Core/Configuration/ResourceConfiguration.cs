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
