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

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	// OBLXExxxx (UiGenerator error)
	public enum ErrorCode {
		NoError = 0,

		TooManyWarnings = 99,

		// assembly
		AssemblyNotFound = 101,
		AssemblyFormatInvalid = 102,
		AssemblyFormatPropertyNotSupported = 103,

		// files
		FileNotFound = 201,
		FileAlreadyExists = 202,
		IncludeFileNotFound = 203,

		// include
		InvalidIncludeTemplateFormat = 304,
		InvalidIncludeTemplateFormatOpeningBracket = 314,
		InvalidIncludeTemplateFormatClosingBracket = 315,

		// repeater
		RepeaterBeginWithoutEnd = 401,
		RepeaterEndWithoutStart = 402,

		// configuration
		ConfigFormatInvalid = 501,
		TypeNotMapped = 502,
		InvalidCopyMode = 503
	}

	public class CodeGeneratorException : ApplicationException
	{
		private static string FormatMessage(ErrorCode errorCode, string info)
		{
			switch (errorCode)
			{
				case ErrorCode.TooManyWarnings:
					info = string.Format("There are more than {0} warnings", CodeGeneratorWarnings.MaximumWarningCount);
					break;

				case ErrorCode.InvalidIncludeTemplateFormat:
					info = "Include placeholders must have the following format: $NAME$(filename)";
					break;

				case ErrorCode.InvalidIncludeTemplateFormatOpeningBracket:
					info = string.Format("{0} is missing \"(\"", info);
					break;

				case ErrorCode.InvalidIncludeTemplateFormatClosingBracket:
					info = string.Format("{0} is missing \")\"", info);
					break;
			}

			if (info == null || info == string.Empty)
				info = "No Info"; // should not occur

			return string.Format("OBLXE{0:d4}: {1}: {2}", (int)errorCode, errorCode.ToString(), info);
		}

		public CodeGeneratorException(ErrorCode errorCode, string info, Exception inner)
			: base(FormatMessage(errorCode, info), inner)
		{
		}

		public CodeGeneratorException(ErrorCode errorCode, string info)
			: base(FormatMessage(errorCode, info))
		{
		}

		public CodeGeneratorException(ErrorCode errorCode)
			: base(FormatMessage(errorCode, null))
		{
		}
	}
}
