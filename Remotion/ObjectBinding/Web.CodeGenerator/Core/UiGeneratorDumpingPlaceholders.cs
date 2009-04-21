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

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	public partial class UiGenerator
	{
		private void OutputPlaceholder(DefinedPlaceholder placeholder, string value)
		{
			if (value == string.Empty)
				_output(string.Format("  {0,-32}", Placeholder.ToString(placeholder)));
			else
				_output(string.Format("  {0,-32}: {1,-32}", Placeholder.ToString(placeholder), value));
		}

		private void OutputPlaceholder(DefinedPlaceholder placeholder)
		{
			OutputPlaceholder(placeholder, string.Empty);
		}

		private void OutputPlaceholder(string placeholder, string value)
		{
			_output(string.Format("  {0,-32}: {1,-32}", placeholder, value));
		}

		public void DumpPlaceholders()
		{
			_output("Dumping placeholders...");

			ClassInfo[] classInfos = GetClassInfos();
			EnumInfo[] enumInfos = GetEnumInfos();

			if (classInfos.Length == 0)
			{
				_output("Nothing to do.");
				return;
			}

			#region get sample values
			Type sampleType = classInfos[0].type;
			IBusinessObjectClass sampleClass = classInfos[0].objectClass;
			EnumInfo sampleEnum = (enumInfos.Length > 0) ? enumInfos[0] : new EnumInfo(null);
			IBusinessObjectProperty sampleProperty = null;
			string sampleControlName = string.Empty;
			string sampleAdditionalAttributes = string.Empty;
			string sampleAdditionalElement = string.Empty;

			if (sampleClass.GetPropertyDefinitions().Length > 0)
				sampleProperty = sampleClass.GetPropertyDefinitions()[0];

			if (_configuration.ControlMappings.Length > 0)
			{
				sampleControlName = _configuration.ControlMappings[0].controlName;

				foreach (Configuration.ControlInfo controlInfo in _configuration.ControlMappings)
				{
					if (sampleAdditionalAttributes == string.Empty && controlInfo.additionalAttributes != string.Empty)
					{
						sampleAdditionalAttributes = controlInfo.additionalAttributes;
					}

					if (sampleAdditionalElement == string.Empty && controlInfo.additionalElements != string.Empty)
					{
						sampleAdditionalElement = controlInfo.additionalElements;

						sampleAdditionalElement = sampleAdditionalElement.Replace("\n", "\\n");
						sampleAdditionalElement = sampleAdditionalElement.Replace("\r", "\\r");
					}
				}
			}
			#endregion

			_output("\n\nUIGen defined");
			_output("-------------");

			_output("\nIncludes:");
			OutputPlaceholder(DefinedPlaceholder.INCLUDE_FOREACHCLASS);
			OutputPlaceholder(DefinedPlaceholder.INCLUDE_FOREACHPROPERTY);

			_output("\nRepeators:");
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHCLASS_BEGIN);
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHCLASS_END);
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHENUM_BEGIN);
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHENUM_END);
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHPROPERTY_BEGIN);
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHPROPERTY_END);
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHREFERENCEDPROPERTY_BEGIN);
			OutputPlaceholder(DefinedPlaceholder.REPEAT_FOREACHREFERENCEDPROPERTY_END);

			_output("\nConfig:");
			OutputPlaceholder(DefinedPlaceholder.PROJECT_ROOTNAMESPACE, _configuration.ProjectNamespaceRoot);
			OutputPlaceholder(DefinedPlaceholder.DOMAIN_ROOTNAMESPACE, _configuration.DomainNamespaceRoot);
			OutputPlaceholder(DefinedPlaceholder.CONTROLTYPE, sampleControlName);
			OutputPlaceholder(DefinedPlaceholder.ADDITIONALATTRIBUTES, sampleAdditionalAttributes);
			OutputPlaceholder(DefinedPlaceholder.ADDITIONALELEMENTS, sampleAdditionalElement);
			OutputPlaceholder(DefinedPlaceholder.ROOT_TEMPLATE_DIR, _configuration.TemplateRoot);
			OutputPlaceholder(DefinedPlaceholder.ROOT_TARGET_DIR, _configuration.TargetRoot);

			_output("\nAssembly:");
			OutputPlaceholder(DefinedPlaceholder.DOMAIN_ASSEMBLYNAME, GetAssemblyName(sampleType));
			OutputPlaceholder(DefinedPlaceholder.DOMAIN_QUALIFIEDCLASSTYPENAME, GetTypeName(sampleType));
			OutputPlaceholder(DefinedPlaceholder.DOMAIN_CLASSNAME, GetName(sampleClass.Identifier));
			OutputPlaceholder(DefinedPlaceholder.DOMAIN_ENUMNAME, sampleEnum.type != null ? sampleEnum.type.Name : string.Empty);
			OutputPlaceholder(DefinedPlaceholder.DOMAIN_REFERENCEDCLASSNAME, GetName(sampleClass.Identifier));
			OutputPlaceholder(DefinedPlaceholder.DOMAIN_PROPERTYNAME, (sampleProperty != null) ? sampleProperty.Identifier : string.Empty);

			_output("\n\nUser defined");
			_output("------------");

			_output("\nConfig:");
			foreach (ApplicationConfiguration.ReplaceInfo replaceInfo in _configuration.ProjectReplaceInfos)
				OutputPlaceholder(replaceInfo.from, replaceInfo.to);

			_output(string.Empty);
		}
	}
}
