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