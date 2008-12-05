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
// --------------------------------------------------------------------------------------
// $Workfile: UiGeneratorSmartCopy.cs $
// $Revision: 1 $ of $Date: 10.03.06 15:02 $ by $Author: Harald-rene.flasch $
//
// Copyright 2006
// rubicon informationstechnologie gmbh
// --------------------------------------------------------------------------------------

using System;
using System.IO;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	public partial class UiGenerator
	{
		private string GetSourceDirectory(Configuration.CopyInfo copyInfo, bool combine)
		{
			string source = (combine) ? Path.Combine(_configuration.TemplateRoot, copyInfo.source) : copyInfo.source;
			int index = source.LastIndexOf('\\');

			if (index == -1)
				return @".\";
			else
				return source.Substring(0, index);
		}

		private string GetSourcePattern(Configuration.CopyInfo copyInfo, bool combine)
		{
			string source = (combine) ? Path.Combine(_configuration.TemplateRoot, copyInfo.source) : copyInfo.source;
			int index = source.LastIndexOf('\\');

			if (index == -1)
				return source;
			else
				return source.Substring(index + 1, source.Length - index - 1);
		}

		private void CopyGlobalFiles()
		{
			foreach (Configuration.CopyInfo copyInfo in _configuration.GlobalCopyFileInfos)
			{
				Copy(copyInfo);
			}
		}

		private void Copy(Configuration.CopyInfo copyInfo)
		{
			// source
			string sourceDirectory = GetSourceDirectory(copyInfo, true);
			string sourcePattern = GetSourcePattern(copyInfo, true);

			// target
			string targetDirectory = Path.Combine(_configuration.TargetRoot, copyInfo.target);
			targetDirectory = Replacer.Replace(_configuration.ProjectReplaceInfos, targetDirectory);

			// ensure target directory
			if (! Directory.Exists(targetDirectory))
				Directory.CreateDirectory(targetDirectory);

			// copy
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectory);
			FileSystemInfo[] fileSysInfos = directoryInfo.GetFileSystemInfos(sourcePattern);

			for (int i = 0; i < fileSysInfos.Length; i++)
			{
				FileInfo fileInfo = fileSysInfos[i] as FileInfo;

				if (fileInfo != null)
					Copy(copyInfo, fileInfo, targetDirectory);
				else
				{
					if (copyInfo.recursive)
					{
						// source
						string sourceDirectoryRecursive = GetSourceDirectory(copyInfo, false);
						string sourcePatternRecursive = GetSourcePattern(copyInfo, false);

						// target
						string targetRecursive = Replacer.Replace(_configuration.ProjectReplaceInfos, copyInfo.target);

						DirectoryInfo directoryInfoRecursive = fileSysInfos[i] as DirectoryInfo;

						// prepate for recursion
						Configuration.CopyInfo copyInfoRecursive = new Configuration.CopyInfo();

						copyInfoRecursive.mode = copyInfo.mode;
						copyInfoRecursive.recursive = copyInfo.recursive;
						copyInfoRecursive.source = Path.Combine(sourceDirectoryRecursive, directoryInfoRecursive.Name) + "\\" + sourcePatternRecursive;
						copyInfoRecursive.target = Path.Combine(targetRecursive, directoryInfoRecursive.Name);

						// recursion
						Copy(copyInfoRecursive);
					}
				}
			}
		}

		private void Copy(Configuration.CopyInfo copyInfo, FileInfo sourceFileInfo, string destinationDirectory)
		{
			string sourceFile = sourceFileInfo.FullName;
			string destinationFile = Path.Combine(destinationDirectory, sourceFileInfo.Name);

			FileInfo destinationFileInfo = new FileInfo(destinationFile);

			// determine copying mode
			if (copyInfo.mode == Configuration.CopyInfoMode.CopyAlways)
			{
				destinationFileInfo.Attributes = FileAttributes.Archive;
			}
			else if (copyInfo.mode == Configuration.CopyInfoMode.CopyIfNewer)
			{
				if (File.Exists(destinationFile))
				{
					if (IsEqual(sourceFileInfo, destinationFileInfo))
						return;

					destinationFileInfo.Attributes = FileAttributes.Archive;
				}
			}
			else
			{
				throw new CodeGeneratorException(ErrorCode.InvalidCopyMode);
			}

			// copy and synchronize file attributes
			File.Copy(sourceFile, destinationFile, true);
			destinationFileInfo.Attributes = sourceFileInfo.Attributes;
		}

		private bool IsEqual(FileInfo a, FileInfo b)
		{
			if (a.Length != b.Length)
				return false;

			if (a.LastWriteTime != b.LastWriteTime)
				return false;

			if (a.Attributes != b.Attributes)
				return false;

			return true;
		}
	}
}
