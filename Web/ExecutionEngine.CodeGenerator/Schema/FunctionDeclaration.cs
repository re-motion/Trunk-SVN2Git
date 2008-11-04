/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Remotion.Web.ExecutionEngine.CodeGenerator.Schema
{
  [XmlRoot (ElementName, Namespace = SchemaUri)]
  public class FunctionDeclaration
  {
    public const string ElementName = "WxePageFunction";

    /// <summary> The namespace of the function declaration schema. </summary>
    /// <remarks> <c>http://www.re-motion.org/web/wxefunctiongenerator</c> </remarks>
    public const string SchemaUri = "http://www.re-motion.org/web/wxefunctiongenerator";

    public static XmlReader GetSchemaReader ()
    {
      return new XmlTextReader (Assembly.GetExecutingAssembly().GetManifestResourceStream (typeof (FunctionDeclaration), "FunctionDeclaration.xsd"));
    }

    private ParameterDeclaration[] _parameters = new ParameterDeclaration[0];
    private VariableDeclaration[] _variables = new VariableDeclaration[0];

    [XmlAttribute ("pageType")]
    public string PageType { get; set; }

    [XmlAttribute ("aspxFile")]
    public string AspxFile { get; set; }

    [XmlAttribute ("functionName")]
    public string FunctionName { get; set; }

    [XmlAttribute ("functionBaseType")]
    public string FunctionBaseType { get; set; }

    [XmlElement ("Parameter")]
    public ParameterDeclaration[] Parameters
    {
      get
      {
        List<ParameterDeclaration> parameters = new List<ParameterDeclaration> (_parameters);
        if (ReturnValue != null)
          parameters.Add (ReturnValue);
        return parameters.ToArray();
      }
      set
      {
        if (value == null)
          _parameters = new ParameterDeclaration[0];
        else
          _parameters = value;
      }
    }

    [XmlElement ("ReturnValue")]
    public ReturnValueDeclaration ReturnValue { get; set; }

    [XmlElement ("Variable")]
    public VariableDeclaration[] Variables
    {
      get { return _variables; }
      set
      {
        if (value == null)
          _variables = new VariableDeclaration[0];
        else
          _variables = value;
      }
    }

    [XmlIgnore]
    public VariableDeclaration[] ParametersAndVariables
    {
      get
      {
        List<VariableDeclaration> result = new List<VariableDeclaration>();
        result.AddRange (Parameters);
        result.AddRange (Variables);
        return result.ToArray();
      }
    }
  }
}