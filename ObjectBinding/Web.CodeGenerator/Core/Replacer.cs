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
using System.IO;
using System.Text;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	class Replacer
	{
		public struct ReplaceInfo {
			public string[] replaceInfos;
		}

		private StringBuilder _text;
		private Encoding _encoding;
		private string _templateRoot;

		// ctor
		public Replacer(string filename)
		{
			_templateRoot = Path.GetDirectoryName(filename);

			StreamReader reader = new StreamReader(filename);

			_text = new StringBuilder( reader.ReadToEnd() );
			_encoding = reader.CurrentEncoding;
			reader.Close();
		}

		#region General Replace implementation
		public static string Replace(ApplicationConfiguration.ReplaceInfo[] replaceInfos, string text)
		{
			foreach (ApplicationConfiguration.ReplaceInfo replaceInfo in replaceInfos)
				text = text.Replace(replaceInfo.from, replaceInfo.to);

			return text;
		}

		public void Replace(ApplicationConfiguration.ReplaceInfo[] replaceInfos)
		{
			foreach (ApplicationConfiguration.ReplaceInfo replaceInfo in replaceInfos)
				_text.Replace(replaceInfo.from, replaceInfo.to);
		}

		public void Replace(DefinedPlaceholder placeholder, string newText)
		{
			_text.Replace(Placeholder.ToString(placeholder), newText);
		}
		#endregion

		#region "Include" implementation
		public void Include(DefinedPlaceholder placeholder, ReplaceInfo[] replaceInfos)
		{
			while (__include(placeholder, replaceInfos));
		}

		private bool __include(DefinedPlaceholder placeholder, ReplaceInfo[] replaceInfos)
		{
			string placeholderString = Placeholder.ToString(placeholder);
			string text = _text.ToString();
			int startIndex = text.IndexOf(placeholderString);

			// something to do?
			if (startIndex == -1)
				return false;

			// yes!
			int length = placeholderString.Length;
			int endIndex = startIndex + length;

			// get include placeholder + filename $NAME$(filename)
			if (text[endIndex] != '(')
				throw new CodeGeneratorException(ErrorCode.InvalidIncludeTemplateFormatOpeningBracket, placeholderString);

			do
			{
				if (endIndex+1 == text.Length)
					throw new CodeGeneratorException(ErrorCode.InvalidIncludeTemplateFormatClosingBracket, placeholderString);

				endIndex++;
				length++;

				if (text[endIndex] == ')')
				{
					endIndex++;
					length++;

					break;
				}
			}
			while (true);

			// get template from include
			string includePlaceholderPlusFilename = text.Substring(startIndex, length);
			string includeTemplate = GetIncludeTemplate(placeholderString, includePlaceholderPlusFilename);

			// replace
			string includeResults = string.Empty;

			foreach (ReplaceInfo replaceInfo in replaceInfos)
			{
				if (replaceInfo.replaceInfos == null)
					continue;

				string includeResult = includeTemplate;

				for (int i=0; i<replaceInfo.replaceInfos.Length-1; i+=2)
					includeResult = includeResult.Replace(replaceInfo.replaceInfos[i], replaceInfo.replaceInfos[i+1]);

				includeResults += includeResult;
			}

			text = text.Replace(includePlaceholderPlusFilename, includeResults);
			_text = new StringBuilder(text);

			return true;
		}

		private string GetIncludeTemplate(string placeholderString, string includePlaceholder)
		{
			string filename = includePlaceholder.Replace(placeholderString, string.Empty);
			filename = filename.TrimStart('(').TrimEnd(')');

			filename = Path.Combine(_templateRoot, filename);

			if (! File.Exists(filename))
				throw new CodeGeneratorException(ErrorCode.IncludeFileNotFound, filename);

			using (StreamReader reader = new StreamReader(filename))
			{
				return reader.ReadToEnd();
			}
		}
		#endregion

		#region "Repeater" implementation
		public void Repeat(DefinedPlaceholder beginPlaceholder, DefinedPlaceholder endPlaceholder, ReplaceInfo[] replaceInfos,
		                   string parameters)
		{
			while (__repeat(beginPlaceholder, endPlaceholder, replaceInfos, parameters)) ;
		}

		public void Repeat(DefinedPlaceholder beginPlaceholder, DefinedPlaceholder endPlaceholder, ReplaceInfo[] replaceInfos)
		{
			while (__repeat(beginPlaceholder, endPlaceholder, replaceInfos, null));
		}

		private bool __repeat(DefinedPlaceholder beginPlaceholder, DefinedPlaceholder endPlaceholder, ReplaceInfo[] replaceInfos,
		                      string parameters)
		{
			string beginPlaceholderString = Placeholder.ToString(beginPlaceholder);
			string endPlaceholderString = Placeholder.ToString(endPlaceholder);
			string text = _text.ToString();
			int beginIndex = text.IndexOf(beginPlaceholderString);
			int endIndex = text.IndexOf(endPlaceholderString);

			if (parameters != null)
				beginPlaceholderString += "(" + parameters + ")";

			// something to do?
			if (beginIndex == -1 && endIndex == -1)
				return false;

			if (beginIndex != -1 && endIndex == -1)
				throw new CodeGeneratorException(ErrorCode.RepeaterBeginWithoutEnd, beginPlaceholderString);

			if (endIndex != -1 && beginIndex == -1)
				throw new CodeGeneratorException(ErrorCode.RepeaterEndWithoutStart, endPlaceholderString);

			// yes!
			string repeatTemplate = GetRepeatTemplate(text, beginPlaceholderString, endPlaceholderString);
			string repeatResults = string.Empty;

			foreach (ReplaceInfo replaceInfo in replaceInfos)
			{
				if (replaceInfo.replaceInfos == null)
					continue;

				string includeResult = repeatTemplate;

				for (int i = 0; i < replaceInfo.replaceInfos.Length - 1; i += 2)
					includeResult = includeResult.Replace(replaceInfo.replaceInfos[i], replaceInfo.replaceInfos[i + 1]);

				repeatResults += includeResult;
			}

			// replace
			text = text.Remove(beginIndex, endIndex-beginIndex+endPlaceholderString.Length);
			text = text.Insert(beginIndex, repeatResults);

			_text = new StringBuilder(text);

			return true;
		}

		private string GetRepeatTemplate(string text, string beginToken, string endToken)
		{
			int beginIndex = text.IndexOf(beginToken);
			int endIndex = text.IndexOf(endToken);

			string template = text.Substring(beginIndex, endIndex-beginIndex);
			template = template.Replace(beginToken, string.Empty);

			return template;
		}
		#endregion

		public void Save(string filename)
		{
			string directoryName = Path.GetDirectoryName(filename);

			if (! File.Exists(directoryName))
				Directory.CreateDirectory(directoryName);

			// we use the same encoding used in the template file (e.g. important for .resx files using umlauts)
			StreamWriter writer = new StreamWriter(filename, false, _encoding);

			writer.Write(_text.ToString());
			writer.Close();
		}
	}
}
