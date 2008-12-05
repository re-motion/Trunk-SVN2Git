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
using Remotion.Diagnostics.ToText;
using Remotion.Text.CommandLine;
using Remotion.Utilities;
using Remotion.Utilities.ConsoleApplication;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplicationSettings : ConsoleApplicationSettings, IToTextConvertible
  {
    [CommandLineStringArgument ("user", true, Placeholder = "accountants/john.doe", Description = "Fully qualified name of user(s) to query access types for.")]
    public string UserName;

    [CommandLineStringArgument ("last", true, Placeholder = "Doe", Description = "Last name of user(s) to query access types for.")]
    public string UserLastName;

    [CommandLineStringArgument ("first", true, Placeholder = "John", Description = "First name of user(s) to query access types for.")]
    public string UserFirstName;

    [CommandLineStringArgument ("dir", true, Placeholder = "c:\\temp", Description = "Directory the ACL-expansion gets written to (e.g. /dir:c:\\temp).")]
    public string Directory = ".";

    [CommandLineStringArgument ("culture", true, Placeholder = "", Description = "Culture to set for output (e.g. /culture:en-US, /culture:de-AT).")]
    public string CultureName = "de-AT";

    [CommandLineFlagArgument ("denied", true, Description = "Output the denied access rights (e.g. /denied+).")]
    public bool OutputDeniedRights;

    [CommandLineFlagArgument ("multifile", false, Description = "Create a single file for all users + permissions or a master file and several detail files.")]
    public bool UseMultipleFileOutput;

    [CommandLineFlagArgument ("verbose", false, Description = "Verbose output (e.g. /verbose+).")]
    public bool Verbose;

    [CommandLineFlagArgument ("rc", false, Description = "Output row count for user, role.... (/multifile- only)")]
    public bool OutputRowCount;

    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.sb ().e ("user", UserName).e ("last", UserLastName).e ("first", UserFirstName).e ("dir", Directory).e ("culture", CultureName).e ("multifile", UseMultipleFileOutput).e ("verbose", Verbose).se ();
    }
  }
}
