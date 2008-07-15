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
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.ExecutionEngine.CodeGenerator.Schema
{
	[XmlRoot(FunctionDeclaration.ElementName, Namespace = FunctionDeclaration.SchemaUri)]
	public class FunctionDeclaration
	{
		public const string ElementName = "WxePageFunction";

		/// <summary> The namespace of the function declaration schema. </summary>
    /// <remarks> <c>http://www.re-motion.org/web/wxefunctiongenerator</c> </remarks>
    public const string SchemaUri = "http://www.re-motion.org/web/wxefunctiongenerator";

		public static XmlReader GetSchemaReader ()
		{
			return new XmlTextReader (Assembly.GetExecutingAssembly ().GetManifestResourceStream (typeof (FunctionDeclaration), "FunctionDeclaration.xsd"));
		}

		private string _pageType;
		private string _aspxFile;
		private string _functionName = null;
	  private string _functionBaseType = null;

		private ParameterDeclaration[] _parameters = new ParameterDeclaration[0];
    private ReturnValueDeclaration _returnValue;
		private VariableDeclaration[] _variables = new VariableDeclaration[0];

		[XmlAttribute("pageType")]
		public string PageType
		{
			get { return _pageType; }
			set { _pageType = value; }
		}

		[XmlAttribute("aspxFile")]
		public string AspxFile
		{
			get { return _aspxFile; }
			set { _aspxFile = value; }
		}

		[XmlAttribute ("functionName")]
		public string FunctionName
		{
			get { return _functionName; }
			set { _functionName = value; }
		}

		[XmlAttribute ("functionBaseType")]
		public string FunctionBaseType
		{
			get { return _functionBaseType; }
			set { _functionBaseType = value; }
		}

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
					_parameters = value; }
		}

	  [XmlElement ("ReturnValue")]
    public ReturnValueDeclaration ReturnValue
	  {
      get { return _returnValue; }
      set { _returnValue = value; }
	  }

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
				List<VariableDeclaration> result = new List<VariableDeclaration> ();
				result.AddRange (Parameters);
				result.AddRange (Variables);
				return result.ToArray ();
			}
		}
	}

	[XmlType (Namespace=FunctionDeclaration.SchemaUri)]
	public class VariableDeclaration
	{
		private string _name;
		private string _typeName;

		[XmlAttribute("name")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[XmlAttribute("type")]
		public string TypeName
		{
			get { return _typeName; }
			set { _typeName = value; }
		}

    public virtual bool IsReturnValue
    {
      get { return false; }
    }
	}

  [XmlType (Namespace = FunctionDeclaration.SchemaUri)]
  public class ReturnValueDeclaration : ParameterDeclaration
  {
    public ReturnValueDeclaration()
    {
      Name = "ReturnValue";
      Direction = WxeParameterDirection.Out;
    }

    public override bool IsReturnValue
    {
      get { return true; }
    }
  }

	[XmlType(Namespace = FunctionDeclaration.SchemaUri)]
	public class ParameterDeclaration: VariableDeclaration
	{
		private bool? _isRequired;
		private WxeParameterDirection _direction;
		// private bool _isReturnValue;

    /// <summary> Used internally. </summary>
		[XmlAttribute ("required")]
		[System.ComponentModel.EditorBrowsable (System.ComponentModel.EditorBrowsableState.Never)]
		public bool xml_isRequired
		{
      get { throw new NotSupportedException ("Get accessor is only defined because set accessor is ignored by XML deserialization otherwise. Do not call."); }
			set { _isRequired = value; }
		}

		[XmlIgnore]
		public bool? IsRequired
		{
			get { return _isRequired; }
			set { _isRequired = value; }
		}

		[XmlAttribute("direction")]
		public WxeParameterDirection Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

    //[XmlAttribute("returnValue")]
    //public bool IsReturnValue
    //{
    //  get { return _isReturnValue; }
    //  set { _isReturnValue = value; }
    //}
	}

}
