// --------------------------------------------------------------------------------------
// $Workfile: ApplicationConfiguration.cs $
// $Revision: 1 $ of $Date: 10.03.06 15:02 $ by $Author: Harald-rene.flasch $
//
// Copyright 2006
// rubicon informationstechnologie gmbh
// --------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;
using System.Xml;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	// TODO: Refactoring: Use Serialization later
	public class ApplicationConfiguration : Configuration
	{
		private CodeGeneratorWarnings _warnings;

		public struct ReplaceInfo {
			public string from;
			public string to;

			public ReplaceInfo(string from, string to) {
				this.from = from;
				this.to = to;
			}
		}

		#region private "settings" fields
		private string _templateRoot;
		private string _targetRoot;
		private string _projectNamespaceRoot;
		private string _domainNamespaceRoot;
		private ReplaceInfo[] _projectReplaceInfos;
		private FileInfo[] _allGlobalFileInfos;
		#endregion

		#region public read-only "settings" properties

    public string TemplateRoot
		{
			get { return _templateRoot; }
		}

		public string TargetRoot
		{
			get { return _targetRoot; }
		}

		public string ProjectNamespaceRoot
		{
			get { return _projectNamespaceRoot; }
		}

		public string DomainNamespaceRoot
		{
			get { return _domainNamespaceRoot; }
		}

		public ReplaceInfo[] ProjectReplaceInfos
		{
			get { return _projectReplaceInfos; }
		}

		public FileInfo[] AllGlobalFileInfos
		{
			get { return _allGlobalFileInfos; }
		}
		#endregion

		public ApplicationConfiguration(string filename, CodeGeneratorWarnings warnings)
		{
			_warnings = warnings;
			this.Init(filename);

			// additional things...
			CompineFileInfos(GlobalFileInfos, GlobalMultipleFileInfos);
		}

		protected new void Init(string filename)
		{
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(filename);

			XmlElement applicationGeneratorElement = xmldoc.DocumentElement;

			// TODO: use XSD file for validation of XML configuration file
			if (applicationGeneratorElement.Name != "applicationGenerator")
				throw new CodeGeneratorException(ErrorCode.ConfigFormatInvalid, "Root element applicationGenerator is missing.");

			// <applicationGenerator/>
			string templateName = GetValue(applicationGeneratorElement, "template", string.Empty);
			base.Init(templateName);

			// <settings/>
			XmlNode settingNode = applicationGeneratorElement.SelectSingleNode("settings");
			_templateRoot = GetValue(settingNode, "templateRoot", ".").TrimEnd('\\');
			_targetRoot = GetValue(settingNode, "targetRoot", ".").TrimEnd('\\');
			_projectNamespaceRoot = GetValue(settingNode, "projectNamespaceRoot", "ProjectNamespace");
			_domainNamespaceRoot = GetValue(settingNode, "domainNamespaceRoot", "DomainNamespaceRoot");

			// <projectReplacements/>
			XmlNode replacementsNode = applicationGeneratorElement.SelectSingleNode("projectReplacements");

			XmlNodeList replaceNodeList = applicationGeneratorElement.SelectNodes("projectReplacements/replace");
			_projectReplaceInfos = new ReplaceInfo[replaceNodeList.Count];
			_projectReplaceInfos = GetReplaceInfos(replaceNodeList);
		}

		private ReplaceInfo[] GetReplaceInfos(XmlNodeList nodes)
		{
			ArrayList tempReplaceInfos = new ArrayList();

			for (int index = 0; index < nodes.Count; index++)
			{
				string from = GetValue(nodes[index], "from");
				string to = GetValue(nodes[index], "to");

				int currentWarningCount = _warnings.Warnings.Count;

				if (! from.StartsWith(PlaceholderPrefix))
					_warnings.AddWarning(WarningCode.ReplaceInvalidFormat, string.Format("Invalid prefix for {0}", from));

				if (! from.EndsWith(PlaceholderPostfix))
					_warnings.AddWarning(WarningCode.ReplaceInvalidFormat, string.Format("Invalid postfix for {0}", from));

				if (from == string.Empty)
					_warnings.AddWarning(WarningCode.ReplaceFromInfoIsEmpty, to);

				if (to == string.Empty) // maybe legal
					_warnings.AddWarning(WarningCode.ReplaceToInfoIsEmpty, from);

				if (from == to)
					_warnings.AddWarning(WarningCode.ReplaceInfoFromEqualsTo, from);

				if (currentWarningCount == _warnings.Warnings.Count)
				{
					ReplaceInfo replaceInfo = new ReplaceInfo();

					replaceInfo.from = from;
					replaceInfo.to = to;

					tempReplaceInfos.Add(replaceInfo);
				}
			}

			ReplaceInfo[] replaceInfos = new ReplaceInfo[tempReplaceInfos.Count];
			int replaceInfoIndex = 0;

			foreach (ReplaceInfo replaceInfo in tempReplaceInfos)
			{
				if (replaceInfo.from == null || replaceInfo.to == null)
					continue;

				replaceInfos[replaceInfoIndex++] = replaceInfo;
			}

			return replaceInfos;
		}

		// fill _allGlobalFileInfos
		private void CompineFileInfos(FileInfo[] fileInfos, MultipleFileInfo[] multipleFileInfos)
		{
			ArrayList allFiles = new ArrayList();

			foreach (FileInfo fileInfo in fileInfos)
				allFiles.Add(fileInfo);

			foreach (MultipleFileInfo multipleFileInfo in multipleFileInfos)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(_templateRoot);
				FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos(multipleFileInfo.pattern);

				int lastBackslashIndex = multipleFileInfo.pattern.LastIndexOf('\\');
				string subDirectory;

				if (lastBackslashIndex == -1)
					subDirectory = string.Empty;
				else
					subDirectory = multipleFileInfo.pattern.Substring(0, lastBackslashIndex);

				foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
				{
					if ( (fileSystemInfo.Attributes & FileAttributes.Directory) != 0)
						continue;

					string targetName = fileSystemInfo.Name;

					if (multipleFileInfo.remove != string.Empty)
					{
						int index = targetName.LastIndexOf(multipleFileInfo.remove);
						targetName = targetName.Remove(index);
					}

					FileInfo fileInfo = new FileInfo();

					fileInfo.template = (subDirectory != string.Empty) ? subDirectory + "\\" + fileSystemInfo.Name : fileSystemInfo.Name;
					fileInfo.target = (multipleFileInfo.target != string.Empty) ? multipleFileInfo.target + "\\" + targetName : targetName;
					fileInfo.overwrite = multipleFileInfo.overwrite;

					allFiles.Add(fileInfo);
				}
			}

			_allGlobalFileInfos = new FileInfo[allFiles.Count];

			for (int index=0; index<allFiles.Count; index++)
				_allGlobalFileInfos[index] = (FileInfo)allFiles[index];
		}
	}
}
