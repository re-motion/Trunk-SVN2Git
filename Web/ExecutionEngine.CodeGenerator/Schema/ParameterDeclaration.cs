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
using System.ComponentModel;
using System.Xml.Serialization;

namespace Remotion.Web.ExecutionEngine.CodeGenerator.Schema
{
  [XmlType (Namespace = FunctionDeclaration.SchemaUri)]
  public class ParameterDeclaration : VariableDeclaration
  {
    // private bool _isReturnValue;

    /// <summary> Used internally. </summary>
    [XmlAttribute ("required")]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public bool xml_isRequired
    {
      get
      {
        throw new NotSupportedException (
            "Get accessor is only defined because set accessor is ignored by XML deserialization otherwise. Do not call.");
      }
      set { IsRequired = value; }
    }

    [XmlIgnore]
    public bool? IsRequired { get; set; }

    [XmlAttribute ("direction")]
    public WxeParameterDirection Direction { get; set; }

    //[XmlAttribute("returnValue")]
    //public bool IsReturnValue
    //{
    //  get { return _isReturnValue; }
    //  set { _isReturnValue = value; }
    //}
  }
}
