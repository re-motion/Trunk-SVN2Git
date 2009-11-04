// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Collections;
using System.Reflection;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	public partial class UiGenerator: IDisposable 
	{
		private struct ClassInfo {
			public Type type;
			public IBusinessObjectClass objectClass;
			public IBusinessObjectProperty[] properties;
		}

		private struct EnumInfo {
			public Type type;

			public EnumInfo(Type theType){
				type = theType;
			}
		}

		public delegate void OutputMethod(string text);

		private OutputMethod _output;
		private CodeGeneratorWarnings _warnings;
		private ApplicationConfiguration _configuration;
    private string _assemblyDirectory;

		// ctor
		public UiGenerator(OutputMethod output, string configurationFileName, string assemblyDirectory)
		{
			_output = output;
			_warnings = new CodeGeneratorWarnings();
			_configuration = new ApplicationConfiguration(configurationFileName, _warnings);
      _assemblyDirectory = assemblyDirectory;

			Placeholder.Prefix = _configuration.PlaceholderPrefix;
			Placeholder.Postfix = _configuration.PlaceholderPostfix;

      AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler (CurrentDomain_AssemblyResolve);
      InitializeConfiguration (assemblyDirectory);
    }

    protected virtual void InitializeConfiguration (string assemblyDirectory)
    {
      ApplicationAssemblyLoaderFilter filter = ApplicationAssemblyLoaderFilter.Instance;
      List<RootAssembly> assemblies = new List<RootAssembly> ();
      DirectoryInfo dir = new DirectoryInfo (assemblyDirectory);
      foreach (FileInfo file in dir.GetFiles ("*.dll"))
      {
        Assembly asm = Assembly.LoadFile (file.FullName);
        if (filter.ShouldConsiderAssembly (asm.GetName()) && filter.ShouldIncludeAssembly (asm))
          assemblies.Add (new RootAssembly (asm, true));
      }
      DomainObjectsConfiguration.SetCurrent (
          new FakeDomainObjectsConfiguration (DomainObjectsConfiguration.Current.MappingLoader, GetPersistenceConfiguration (), new QueryConfiguration()));

      FixedRootAssemblyFinder rootAssemblyFinder = new FixedRootAssemblyFinder (assemblies.ToArray());
      FilteringAssemblyLoader assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
      AssemblyFinder assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);
      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);

      MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeDiscoveryService)));
    }

    protected StorageConfiguration GetPersistenceConfiguration ()
    {
      StorageConfiguration storageConfiguration = DomainObjectsConfiguration.Current.Storage;
      if (storageConfiguration.DefaultStorageProviderDefinition == null)
      {
        ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition> ();
        RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Default", typeof (SqlProvider), "Initial Catalog=DatabaseName;");
        storageProviderDefinitionCollection.Add (providerDefinition);

        storageConfiguration = new StorageConfiguration (storageProviderDefinitionCollection, providerDefinition);
      }

      return storageConfiguration;
    }

    public void Dispose()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler (CurrentDomain_AssemblyResolve);
    }

    Assembly CurrentDomain_AssemblyResolve (object sender, ResolveEventArgs args)
    {
      if (_assemblyDirectory != null)
      {
        string filename = Path.Combine (_assemblyDirectory, args.Name + ".dll");
        if (File.Exists (filename))
          return Assembly.LoadFrom (filename);
      }
      return null;
    }

    private static bool InterfaceFilter (Type typeObject, Object criteriaObject)
		{
			return typeObject.ToString() == criteriaObject.ToString();
		}

		private static bool HasInterface(Type type, string name)
		{
			TypeFilter typeFilter = new TypeFilter(InterfaceFilter);
			Type[] interfaces = type.FindInterfaces(typeFilter, name);

			if (interfaces.Length != 1)
				return false;

			if (! interfaces[0].IsInterface) // should not occur
				return false;

			return true;
		}

		private ClassInfo[] GetClassInfos()
		{
			ArrayList classInfoList = new ArrayList();

			foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
			{
				if (! (IgnoreClass(classDefinition) || classDefinition.IsAbstract))
				{
					ClassInfo classInfo = new ClassInfo();

					classInfo.type = classDefinition.ClassType;
					classInfo.objectClass = BindableObjectProvider.GetBindableObjectClass (classDefinition.ClassType);
					classInfo.properties = GetProperties(classInfo.objectClass.GetPropertyDefinitions());

					classInfoList.Add(classInfo);
				}
			}

			#region Thanks to MK
			/*
			if (!File.Exists(_configuration.AssemblyName))
				throw new CodeGeneratorException(ErrorCode.AssemblyNotFound, _configuration.AssemblyName);

			Assembly assembly = Assembly.LoadFile(_configuration.AssemblyName);
			TypeFilter typeFilter = new TypeFilter(InterfaceFilter);
			Type[] types = assembly.GetTypes();
			ArrayList classInfoList = new ArrayList();

			foreach (Type type in types)
			{
				if (! type.IsClass)
					continue;

				if (! HasInterface(type, "Remotion.ObjectBinding.IBusinessObject"))
					continue;

				Type domainType = type;
				ClassProvider provider = null;

				while (domainType != typeof(Object))
				{
					provider = ClassProvider.GetProvider(domainType);

					if (provider != null)
						break;

					domainType = domainType.BaseType;
				}

				if (provider == null)
					throw new CodeGeneratorException(ErrorCode.TypeNotMapped, domainType.FullName);

				IBusinessObjectClass businessObjectClass = provider.GetClass(type);

				if (! IgnoreClass(businessObjectClass))
				{
					ClassInfo classInfo = new ClassInfo();

					classInfo.type = type;
					classInfo.objectClass = businessObjectClass;
					classInfo.properties = GetProperties(businessObjectClass.GetPropertyDefinitions());

					classInfoList.Add(classInfo);
				}
			}
			*/
			#endregion

			return (ClassInfo[])classInfoList.ToArray(typeof(ClassInfo));
		}

		private EnumInfo[] GetEnumInfos()
		{
			ArrayList enumInfoList = new ArrayList();

			foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
			{
				foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
				{
					if (propertyDefinition.PropertyType.IsEnum)
					{
						EnumInfo enumInfo = new EnumInfo();
						enumInfo.type = propertyDefinition.PropertyType;

						enumInfoList.Add(enumInfo);
					}
				}
			}

			#region Thanks to MK
			/*
			if (! File.Exists(_configuration.AssemblyName))
				throw new CodeGeneratorException(ErrorCode.AssemblyNotFound, _configuration.AssemblyName);

			Assembly assembly = Assembly.LoadFile(_configuration.AssemblyName);
			Type[] types = assembly.GetTypes();
			ArrayList enumInfoList = new ArrayList();

			foreach (Type type in types)
			{
				if (! type.IsEnum)
					continue;

				EnumInfo enumInfo = new EnumInfo();
				enumInfo.type = type;

				enumInfoList.Add(enumInfo);
			}
			*/
			#endregion

			return (EnumInfo[])enumInfoList.ToArray(typeof(EnumInfo));
		}

		#region Build
		public void Build()
		{
			try
			{
				ClassInfo[] classInfos = GetClassInfos();
				EnumInfo[] enumInfos = GetEnumInfos();

				// copy files
				CopyGlobalFiles();

				// generate files like NavigationTabs.cs.ascx, Web.config, etc.
				BuildGlobalFiles(classInfos, enumInfos);

				// generate .asxc, .aspx and code files
				BuildClassFiles(classInfos);

				// generate .resx, .Designer.cs files
				BuildEnumFiles(enumInfos);

				_warnings.PrintWarnings(new CodeGeneratorWarnings.OutputMethod(_output));
			}
			catch (CodeGeneratorException e)
			{
				_warnings.PrintWarnings(new CodeGeneratorWarnings.OutputMethod(_output));
				throw e;
			}
		}

		private void BuildGlobalFiles(ClassInfo[] classInfos, EnumInfo[] enumInfos)
		{
			foreach (Configuration.FileInfo fileInfo in _configuration.AllGlobalFileInfos)
			{
				string templateFileName = Path.Combine(_configuration.TemplateRoot, fileInfo.template);

				string targetFileName = Path.Combine(_configuration.TargetRoot, fileInfo.target);
				targetFileName = Replacer.Replace(_configuration.ProjectReplaceInfos, targetFileName);

				CheckExistenceTemplateFile(templateFileName);
				CheckExistenceDestinationFile(targetFileName, fileInfo.overwrite);

				IBusinessObjectClass[] businessObjectClasses = new IBusinessObjectClass[classInfos.Length];

				for (int index = 0; index < classInfos.Length; index++)
					businessObjectClasses[index] = classInfos[index].objectClass;

				Replacer replacer = new Replacer(templateFileName);

				// for each class
				replacer.Include(
						DefinedPlaceholder.INCLUDE_FOREACHCLASS,
						GetClassInfos(businessObjectClasses));

				replacer.Repeat(
						DefinedPlaceholder.REPEAT_FOREACHCLASS_BEGIN,
						DefinedPlaceholder.REPEAT_FOREACHCLASS_END,
						GetClassInfos(businessObjectClasses));

				// for each enum
				replacer.Include(
						DefinedPlaceholder.INCLUDE_FOREACHENUM,
						GetEnumInfos(enumInfos));

				replacer.Repeat(
						DefinedPlaceholder.REPEAT_FOREACHENUM_BEGIN,
						DefinedPlaceholder.REPEAT_FOREACHENUM_END,
						GetEnumInfos(enumInfos));

				// common
				ReplaceCommon(replacer, classInfos[0].type);

				replacer.Save(targetFileName);
			}
		}

		private void BuildClassFiles(ClassInfo[] classInfos)
		{
			foreach (ClassInfo classInfo in classInfos)
				BuildClassFile(classInfo.type, classInfo.objectClass, classInfo.properties);
		}

		private void BuildClassFile(Type type, IBusinessObjectClass businessObjectClass, IBusinessObjectProperty[] properties)
		{
			foreach (Configuration.FileInfo fileInfo in _configuration.ClassFileInfos)
			{
				string name = GetName(businessObjectClass.Identifier);
				string templateFileName = Path.Combine(_configuration.TemplateRoot, fileInfo.template);

				string targetFileName = Path.Combine(_configuration.TargetRoot, Replace(fileInfo.target, DefinedPlaceholder.DOMAIN_CLASSNAME, name));
				targetFileName = Replacer.Replace(_configuration.ProjectReplaceInfos, targetFileName);

				CheckExistenceTemplateFile(templateFileName);
				CheckExistenceDestinationFile(targetFileName, fileInfo.overwrite);

				Replacer replacer = new Replacer(templateFileName);

				// for each property
				replacer.Include(
						DefinedPlaceholder.INCLUDE_FOREACHPROPERTY,
						GetPropertyInfos(businessObjectClass, properties));

				replacer.Repeat(
						DefinedPlaceholder.REPEAT_FOREACHPROPERTY_BEGIN,
						DefinedPlaceholder.REPEAT_FOREACHPROPERTY_END,
						GetPropertyInfos(businessObjectClass, properties));

				replacer.Repeat(
						DefinedPlaceholder.REPEAT_FOREACHREFERENCEDPROPERTY_BEGIN,
						DefinedPlaceholder.REPEAT_FOREACHREFERENCEDPROPERTY_END,
						GetReferencedPropertyInfos(businessObjectClass, properties, true),
						"isList=true");

				replacer.Repeat(
						DefinedPlaceholder.REPEAT_FOREACHREFERENCEDPROPERTY_BEGIN,
						DefinedPlaceholder.REPEAT_FOREACHREFERENCEDPROPERTY_END,
						GetReferencedPropertyInfos(businessObjectClass, properties, false),
						"isList=false");

				// common
				replacer.Replace(DefinedPlaceholder.DOMAIN_CLASSNAME, name);
				ReplaceCommon(replacer, type);

				replacer.Save(targetFileName);
			}
		}

		private void BuildEnumFiles(EnumInfo[] enumInfos)
		{
			foreach (EnumInfo enumInfo in enumInfos)
				BuildEnumFile(enumInfo);
		}

		private void BuildEnumFile(EnumInfo enumInfo)
		{
			foreach (Configuration.FileInfo fileInfo in _configuration.EnumFileInfos)
			{
				string name = enumInfo.type.Name;
				string templateFileName = Path.Combine(_configuration.TemplateRoot, fileInfo.template);

				string targetFileName = Path.Combine(_configuration.TargetRoot, Replace(fileInfo.target, DefinedPlaceholder.DOMAIN_ENUMNAME, name));
				targetFileName = Replacer.Replace(_configuration.ProjectReplaceInfos, targetFileName);

				CheckExistenceTemplateFile(templateFileName);
				CheckExistenceDestinationFile(targetFileName, fileInfo.overwrite);

				Replacer replacer = new Replacer(templateFileName);

				// for each enum value
				/* --- still not needed at the moment --- */

				// common
				replacer.Replace(DefinedPlaceholder.DOMAIN_ENUMNAME, name);
				ReplaceCommon(replacer, enumInfo.type);

				replacer.Save(targetFileName);
			}
		}
		#endregion

		private void CheckExistenceDestinationFile(string filename, bool overwrite)
		{
			if (! overwrite && File.Exists(filename))
				throw new CodeGeneratorException(ErrorCode.FileAlreadyExists, filename);
			else if (File.Exists(filename))
				File.Delete(filename);
		}

		private void CheckExistenceTemplateFile(string filename)
		{
			if (! File.Exists(filename))
				throw new CodeGeneratorException(ErrorCode.FileNotFound, filename);
		}

		private string Replace(string text, DefinedPlaceholder placeholder, string newText)
		{
			string replaceText = Placeholder.ToString(placeholder);
			return text.Replace(replaceText, newText);
		}

		private void ReplaceCommon(Replacer replacer, Type type)
		{
			replacer.Replace(DefinedPlaceholder.PROJECT_ROOTNAMESPACE, _configuration.ProjectNamespaceRoot);
			replacer.Replace(DefinedPlaceholder.DOMAIN_ROOTNAMESPACE, _configuration.DomainNamespaceRoot);

			replacer.Replace(DefinedPlaceholder.DOMAIN_ASSEMBLYNAME, GetAssemblyName(type));
			replacer.Replace(DefinedPlaceholder.DOMAIN_QUALIFIEDCLASSTYPENAME, GetTypeName(type));

			replacer.Replace(DefinedPlaceholder.ROOT_TEMPLATE_DIR, _configuration.TemplateRoot);
			replacer.Replace(DefinedPlaceholder.ROOT_TARGET_DIR, _configuration.TargetRoot);
		  replacer.Replace (DefinedPlaceholder.WEB_CLIENT_GUID, Guid.NewGuid().ToString());

			replacer.Replace(_configuration.ProjectReplaceInfos);
		}

		private string GetTypeName(Type type)
		{
			string[] assemblyInfo = type.Assembly.FullName.Split(',');
			return string.Format("{0}, {1}", type.FullName, assemblyInfo[0]);
		}

		private string GetAssemblyName(Type type)
		{
			string[] assemblyInfo = type.Assembly.FullName.Split(',');
			return assemblyInfo[0];
		}

		private string GetName(string identifier)
		{
      //MK: Hack requires BindableObject-implementation. Possible since GetClassInfos requires BindableObject-class
		  return TypeUtility.GetType (identifier, true).Name;
		}

		// TODO: remove
		private bool IgnoreClass(IBusinessObjectClass theClass)
		{
			string[] ignoreClasses = _configuration.IgnoreClasses;

			foreach (string className in ignoreClasses)
			{
				if (className == theClass.Identifier)
					return true;
			}

			return false;
		}

    private bool IgnoreClass(ClassDefinition classDefinition)
    {
      string[] ignoreClasses = _configuration.IgnoreClasses;

      foreach (string className in ignoreClasses)
      {
        if (className == classDefinition.ID)
          return true;
      }

      return false;
    }

		private bool IgnoreProperty(IBusinessObjectProperty property)
		{
			string[] ignoreProperties = _configuration.IgnoreProperties;

			foreach (string propertyName in ignoreProperties)
			{
				if (propertyName == property.Identifier)
					return true;
			}

			return false;
		}

		private IBusinessObjectProperty[] GetProperties(IBusinessObjectProperty[] properties)
		{
			ArrayList propertyList = new ArrayList();

			foreach (IBusinessObjectProperty property in properties)
			{
				if (! IgnoreProperty(property))
					propertyList.Add(property);
			}

			properties = new IBusinessObjectProperty[propertyList.Count];

			for (int i=0; i<properties.Length; i++)
				properties[i] = (IBusinessObjectProperty)propertyList[i];

			return properties;
		}

		private Replacer.ReplaceInfo[] GetPropertyInfos(IBusinessObjectClass businessObjectClass, IBusinessObjectProperty[] properties)
		{
			Replacer.ReplaceInfo[] propertyInfos = new Replacer.ReplaceInfo[properties.Length];

			for (int index = 0; index < properties.Length; index++)
			{
				IBusinessObjectProperty property = properties[index];
				bool found = false;

				foreach (Configuration.ControlInfo controlMapping in _configuration.ControlMappings)
				{
					if (HasInterface(property.GetType(), controlMapping.propertyType) && property.IsList == controlMapping.isList)
					{
						ApplicationConfiguration.ReplaceInfo[] additionalReplaceInfos = new ApplicationConfiguration.ReplaceInfo[] {
							new ApplicationConfiguration.ReplaceInfo(Placeholder.ToString(DefinedPlaceholder.DOMAIN_PROPERTYNAME), property.Identifier)
						};

						string additionalAttributes = Replacer.Replace(additionalReplaceInfos, controlMapping.additionalAttributes);
						string additionalElements = Replacer.Replace(additionalReplaceInfos, controlMapping.additionalElements);

						propertyInfos[index].replaceInfos = new string[] {
							Placeholder.ToString(DefinedPlaceholder.DOMAIN_PROPERTYNAME), property.Identifier,
							Placeholder.ToString(DefinedPlaceholder.CONTROLTYPE), controlMapping.controlName,
							Placeholder.ToString(DefinedPlaceholder.ADDITIONALATTRIBUTES), additionalAttributes,
							Placeholder.ToString(DefinedPlaceholder.ADDITIONALELEMENTS), additionalElements
						};

						found = true;
						break;
					}
				}

				if (! found)
				{
					propertyInfos[index].replaceInfos = null;

					_warnings.AddWarning(WarningCode.MissingControlMapping, businessObjectClass.Identifier + "." + property.Identifier +
							" (" + property.GetType().ToString() + ")");
				}
			}

			return propertyInfos;
		}

		private Replacer.ReplaceInfo[] GetReferencedPropertyInfos(IBusinessObjectClass businessObjectClass, IBusinessObjectProperty[] properties, bool isList)
		{
			ArrayList replaceInfosArrayList = new ArrayList();

			foreach (PropertyBase property in properties)
			{
				if (! HasInterface(property.GetType(), "Remotion.ObjectBinding.IBusinessObjectReferenceProperty") || isList != property.IsList)
					continue;

			  Type itemType = property.IsList ? property.ListInfo.ItemType : property.PropertyType;
        string[] referencedClassNameInfo = itemType.FullName.Split ('.');
				string referencedClassName = referencedClassNameInfo[referencedClassNameInfo.Length - 1];

				Replacer.ReplaceInfo replaceInfo = new Replacer.ReplaceInfo();

				replaceInfo.replaceInfos = new string[] {
						Placeholder.ToString(DefinedPlaceholder.DOMAIN_REFERENCEDCLASSNAME), referencedClassName,
						Placeholder.ToString(DefinedPlaceholder.DOMAIN_PROPERTYNAME), property.Identifier
				};

				replaceInfosArrayList.Add(replaceInfo);
			}

			Replacer.ReplaceInfo[] replaceInfos = new Replacer.ReplaceInfo[replaceInfosArrayList.Count];

			for (int i=0; i<replaceInfos.Length; i++)
				replaceInfos[i] = (Replacer.ReplaceInfo)replaceInfosArrayList[i];

			return replaceInfos;
		}

		private Replacer.ReplaceInfo[] GetClassInfos(IBusinessObjectClass[] classes)
		{
			Replacer.ReplaceInfo[] classInfos = new Replacer.ReplaceInfo[classes.Length];

			for (int index = 0; index < classes.Length; index++)
			{
				classInfos[index].replaceInfos = new string[] {
						Placeholder.ToString(DefinedPlaceholder.DOMAIN_CLASSNAME), GetName(classes[index].Identifier) };
			}

			return classInfos;
		}

		private Replacer.ReplaceInfo[] GetEnumInfos(EnumInfo[] enums)
		{
			Replacer.ReplaceInfo[] enumInfos = new Replacer.ReplaceInfo[enums.Length];

			for (int index = 0; index < enums.Length; index++)
			{
				enumInfos[index].replaceInfos = new string[] {
						Placeholder.ToString(DefinedPlaceholder.DOMAIN_ENUMNAME), enums[index].type.Name };
			}

			return enumInfos;
		}
	}
}
