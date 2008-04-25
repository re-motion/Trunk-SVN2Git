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
