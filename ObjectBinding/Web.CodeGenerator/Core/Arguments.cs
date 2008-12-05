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
