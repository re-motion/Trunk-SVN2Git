// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Xml;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	// TODO: Refactoring: Use Serialization later
	public class Configuration
	{
		public enum CopyInfoMode {
			CopyAlways,
			CopyIfNewer
		}

		public struct CopyInfo {
			public string source;
			public string target;
			public CopyInfoMode mode;
			public bool recursive;
		}

		public struct FileInfo {
			public string template;
			public string target;
			public bool overwrite;
		}

		public struct MultipleFileInfo {
			public string pattern;
			public string remove;
			public string target;
			public bool overwrite;
		}

		public struct ControlInfo {
			public string propertyType;
			public string controlName;
			public bool isList;
			public string additionalAttributes;
			public string additionalElements;
		}

		#region private "settings" fields
		private string _placeholderPrefix;
		private string _placeholderPostfix;
		private CopyInfo[] _globalCopyInfos;
		private FileInfo[] _globalFileInfos;
		private MultipleFileInfo[] _globalMultipleFileInfos;
		private FileInfo[] _classFileInfos;
		private FileInfo[] _enumFileInfos;
		private ControlInfo[] _controlMappings;
		private string[] _ignoreClasses;
		private string[] _ignoreProperties;
		#endregion

		#region public read-only "settings" properties
		public string PlaceholderPrefix
		{
			get { return _placeholderPrefix; }
		}

		public string PlaceholderPostfix
		{
			get { return _placeholderPostfix; }
		}

		public CopyInfo[] GlobalCopyFileInfos
		{
			get { return _globalCopyInfos; }
		}

		public FileInfo[] GlobalFileInfos
		{
			get { return _globalFileInfos; }
		}

		public MultipleFileInfo[] GlobalMultipleFileInfos
		{
			get { return _globalMultipleFileInfos; }
		}

		public FileInfo[] ClassFileInfos
		{
			get { return _classFileInfos; }
		}

		public FileInfo[] EnumFileInfos
		{
			get { return _enumFileInfos; }
		}

		public ControlInfo[] ControlMappings
		{
			get { return _controlMappings; }
		}

		public string[] IgnoreClasses
		{
			get { return _ignoreClasses; }
		}

		public string[] IgnoreProperties
		{
			get { return _ignoreProperties; }
		}
		#endregion

		protected void Init(string filename)
		{
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(filename);

			XmlElement generatorElement = xmldoc.DocumentElement;

			// TODO: use XSD file for validation of XML configuration file
			if (generatorElement.Name != "generator")
				throw new CodeGeneratorException(ErrorCode.ConfigFormatInvalid, "Root element generator is missing.");

			// <generator/>
			_placeholderPrefix = GetValue(generatorElement, "placeholderPrefix", "$");
			_placeholderPostfix = GetValue(generatorElement, "placeholderPostfix", "$");

			// <global/>
			XmlNode globalNode = generatorElement.SelectSingleNode("global");

			// <copy>
			XmlNodeList globalCopyNodeList = generatorElement.SelectNodes("global/copy");
			_globalCopyInfos = new CopyInfo[globalCopyNodeList.Count];
			_globalCopyInfos = GetCopyInfos(globalCopyNodeList);

			// <file>
			XmlNodeList globalFileNodeList = generatorElement.SelectNodes("global/file");
			_globalFileInfos = new FileInfo[globalFileNodeList.Count];
			_globalFileInfos = GetFileInfos(globalFileNodeList);

			// <files>
			XmlNodeList globalMultipleFileNodeList = generatorElement.SelectNodes("global/files");
			_globalMultipleFileInfos = new MultipleFileInfo[globalMultipleFileNodeList.Count];
			_globalMultipleFileInfos = GetMultipleFileInfos(globalMultipleFileNodeList);

			// <forEachClass/>
			XmlNode forEachClassNode = generatorElement.SelectSingleNode("forEachClass");
			_ignoreClasses = GetValue(forEachClassNode, "ignore", string.Empty).Split(';');

			XmlNodeList classFileNodeList = generatorElement.SelectNodes("forEachClass/file");
			_classFileInfos = new FileInfo[classFileNodeList.Count];
			_classFileInfos = GetFileInfos(classFileNodeList);

			// <forEachEnum/>
			XmlNode forEachEnumNode = generatorElement.SelectSingleNode("forEachEnum");

			XmlNodeList enumFileNodeList = generatorElement.SelectNodes("forEachEnum/file");
			_enumFileInfos = new FileInfo[enumFileNodeList.Count];
			_enumFileInfos = GetFileInfos(enumFileNodeList);

			// <properties/>
			XmlNode propertiesNode = generatorElement.SelectSingleNode("properties");
			_ignoreProperties = GetValue(propertiesNode, "ignore", string.Empty).Split(';');

			// <controlMappings/>
			//   <mapping/>
			//     <additionalAttributes/>
			//     <additionalElements/>
			XmlNodeList controlMappingNodeList = propertiesNode.SelectNodes("controlMappings/mapping");

			_controlMappings = new ControlInfo[controlMappingNodeList.Count];

			for (int index = 0; index < controlMappingNodeList.Count; index++)
			{
				_controlMappings[index].propertyType = GetValue(controlMappingNodeList[index], "propertyType");
				_controlMappings[index].controlName = GetValue(controlMappingNodeList[index], "control");
				_controlMappings[index].isList = GetValue(controlMappingNodeList[index], "isList", false);

				XmlNode additionalAttributesNode = controlMappingNodeList[index].SelectSingleNode("additionalAttributes");
				_controlMappings[index].additionalAttributes = (additionalAttributesNode != null) ? additionalAttributesNode.InnerText : string.Empty;
				NormalizeSingleLine(ref _controlMappings[index].additionalAttributes);

				XmlNode additionalElementsNode = controlMappingNodeList[index].SelectSingleNode("additionalElements");
				_controlMappings[index].additionalElements = (additionalElementsNode != null) ? additionalElementsNode.InnerText : string.Empty;
				NormalizeMultipleLine(ref _controlMappings[index].additionalElements);
			}
		}

		#region GetValue methods
		protected static string GetValue(XmlElement element, string name, string defaultValue)
		{
			if (element == null)
				return defaultValue;

			if (element.Attributes[name] == null)
				return defaultValue;

			return element.Attributes[name].Value;
		}

		protected static string GetValue(XmlElement element, string name)
		{
			string value = GetValue(element, name, null);

			if (value != null)
				return value;
			else
				throw new CodeGeneratorException(ErrorCode.ConfigFormatInvalid, string.Format("The attribute '{0}' is mandatory.", name));
		}

		protected static string GetValue(XmlNode node, string name, string defaultValue)
		{
			if (node == null)
				return defaultValue;
			else
			{
				XmlAttribute attribute = node.Attributes[name];

				if (attribute == null)
					return defaultValue;
				else
					return attribute.Value;
			}
		}

		protected static string GetValue(XmlNode node, string name)
		{
			string value = GetValue(node, name, null);

			if (value != null)
				return value;
			else
				throw new CodeGeneratorException(ErrorCode.ConfigFormatInvalid, string.Format("The attribute '{0}' is mandatory.", name));
		}

		protected static bool GetValue(XmlNode node, string name, bool defaultValue)
		{
			string stringValue = GetValue(node, name, defaultValue ? "true" : "false");
			string stringValueLower = stringValue.ToLower();

			if (stringValueLower == "true" || stringValueLower == "yes" || stringValueLower == "1")
				return true;
			else if (stringValueLower == "false" || stringValueLower == "no" || stringValueLower == "0")
				return false;
			else
				throw new CodeGeneratorException(ErrorCode.ConfigFormatInvalid, string.Format("Invalid boolean value: {0}", stringValue));
		}

		private static CopyInfoMode GetCopyModeValue(XmlNode xmlNode, string name)
		{
			string valueAsString = GetValue(xmlNode, name);

			switch (valueAsString)
			{
				case "CopyAlways":  return CopyInfoMode.CopyAlways;
				case "CopyIfNewer": return CopyInfoMode.CopyIfNewer;
			}

			throw new CodeGeneratorException(ErrorCode.InvalidCopyMode);
		}
		#endregion

		private static void NormalizeSingleLine(ref string text)
		{
			if (text == string.Empty)
				return;

			text = text.Replace("\n", " ");
			text = text.Replace("\r", " ");
			text = text.Replace("\t", " ");

			text = text.Trim(' ');

			while (text.IndexOf("  ") != -1)
				text = text.Replace("  ", " ");
		}

		private static void NormalizeMultipleLine(ref string text)
		{
			if (text == string.Empty)
				return;

			// TODO: implement
			text = text.Trim(new char[] { ' ', '\r', '\n', '\t' });
		}

		private static CopyInfo[] GetCopyInfos(XmlNodeList nodes)
		{
			CopyInfo[] copyInfos = new CopyInfo[nodes.Count];

			for (int index = 0; index < nodes.Count; index++)
			{
				copyInfos[index].source = GetValue(nodes[index], "source");
				copyInfos[index].target = GetValue(nodes[index], "target");
				copyInfos[index].mode = GetCopyModeValue(nodes[index], "mode");
				copyInfos[index].recursive = GetValue(nodes[index], "recursive", false);
			}

			return copyInfos;
		}

		private static FileInfo[] GetFileInfos(XmlNodeList nodes)
		{
			FileInfo[] fileInfos = new FileInfo[nodes.Count];

			for (int index = 0; index < nodes.Count; index++)
			{
				fileInfos[index].template = GetValue(nodes[index], "template");
				fileInfos[index].target = GetValue(nodes[index], "target");
				fileInfos[index].overwrite = GetValue(nodes[index], "overwrite", false);
			}

			return fileInfos;
		}

		private static MultipleFileInfo[] GetMultipleFileInfos(XmlNodeList nodes)
		{
			MultipleFileInfo[] multipleFileInfos = new MultipleFileInfo[nodes.Count];

			for (int index = 0; index < nodes.Count; index++)
			{
				multipleFileInfos[index].pattern = GetValue(nodes[index], "pattern", "*.*");
				multipleFileInfos[index].remove = GetValue(nodes[index], "remove", string.Empty);
				multipleFileInfos[index].target = GetValue(nodes[index], "target", string.Empty);
				multipleFileInfos[index].overwrite = GetValue(nodes[index], "overwrite", false);
			}

			return multipleFileInfos;
		}
	}
}
