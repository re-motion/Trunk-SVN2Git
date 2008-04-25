// --------------------------------------------------------------------------------------
// $Workfile: Arguments.cs $
// $Revision: 2 $ of $Date: 10.03.06 15:19 $ by $Author: Harald-rene.flasch $
//
// Copyright 2006
// rubicon informationstechnologie gmbh
// --------------------------------------------------------------------------------------

using System;
using Remotion.Text.CommandLine;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	internal class Arguments
	{
		[CommandLineFlagArgument("appinfo", false,
				Description = "Displays application and copyright information")]
		public bool ApplicationInfo = false;

		[CommandLineStringArgument("uigen", true,
				Description = "Generates application based on templates",
				Placeholder = "configuration")]
		public string UIGenConfiguration = null;

		[CommandLineStringArgument("placeholders", true,
				Description = "Displays all placeholders and their values",
				Placeholder = "configuration")]
		public string Placeholders = null;

		[CommandLineStringArgument("asmdir", true)]
		public string AssemblyDirectory = null;
	}
}
