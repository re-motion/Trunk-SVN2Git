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
// $Workfile: CodeGeneratorWarnings.cs $
// $Revision: 1 $ of $Date: 10.03.06 15:02 $ by $Author: Harald-rene.flasch $
//
// Copyright 2006
// rubicon informationstechnologie gmbh
// --------------------------------------------------------------------------------------

using System;
using System.Collections;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	// OBLXWxxxx (UiGenerator warning)
	public enum WarningCode {
		NoWarning = 0,
		MissingControlMapping = 101,

		// replace info
		ReplaceInvalidFormat = 201,
		ReplaceFromInfoIsEmpty = 202,
		ReplaceToInfoIsEmpty = 203,
		ReplaceInfoFromEqualsTo = 204
	}

	public class CodeGeneratorWarnings
	{
		public delegate void OutputMethod(string text);
		private ArrayList _warnings = new ArrayList();

		public ArrayList Warnings
		{
			get { return _warnings; }
		}

		public static int MaximumWarningCount
		{
			get { return 32; }
		}

		public void AddWarning(WarningCode warningCode, string info)
		{
			// TODO: make number of maximum warnings configureable
			if (_warnings.Count >= MaximumWarningCount)
				throw new CodeGeneratorException(ErrorCode.TooManyWarnings);

			if (info == null || info == string.Empty)
				info = "No Info"; // should not occur

			string warningText = string.Format("OBLXW{0:d4}: {1}: {2}", (int)warningCode, warningCode.ToString(), info);

			if (! _warnings.Contains(warningText))
				_warnings.Add(warningText);
		}

		public void PrintWarnings(OutputMethod output)
		{
			foreach (string warning in _warnings)
				output(warning);
		}
	}
}
