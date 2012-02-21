// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Text.CommandLine;

namespace Remotion.Mixins.MixerTools
{
  [Serializable]
  public class MixerParameters
  {
    [CommandLineStringArgument ("baseDirectory", true,
        Description = "The base directory to use for looking up the files to be processed (default: current).",
        Placeholder = "directory")]
    public string BaseDirectory = Environment.CurrentDirectory;

    [CommandLineStringArgument ("config", true,
        Description = 
            "The config file holding the application's configuration. "
            + "Unless the path is rooted, the config file is located relative to the baseDirectory.",
        Placeholder = "app.config")]
    public string ConfigFile = string.Empty;

    [CommandLineStringArgument ("assemblyDirectory", true,
        Description = "Create assembly file(s) in this directory (default: current).",
        Placeholder = "directory")]
    public string AssemblyOutputDirectory = Environment.CurrentDirectory;

    [CommandLineStringArgument ("signedAssemblyName", true,
        Description = "The simple name of the signed assembly generated (without extension; default: Remotion.Mixins.Persistent.Signed).",
        Placeholder = "simpleName")]
    public string SignedAssemblyName = "Remotion.Mixins.Persistent.Signed";

    [CommandLineStringArgument ("unsignedAssemblyName", true,
        Description = "The simple name of the unsigned assembly generated (without extension; default: Remotion.Mixins.Persistent.Unsigned).",
        Placeholder = "simpleName")]
    public string UnsignedAssemblyName = "Remotion.Mixins.Persistent.Unsigned";

    [CommandLineFlagArgument ("keepTypeNames", false,
        Description = "Specifies that the mixer should not use GUIDs to name the generated types, but instead keep the type names of the target "
        + "types. To get unique names, the mixer will put the generated types in a dedicated namespace.")]
    public bool KeepTypeNames;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Enables verbose output. Verbose output will include all messages from all loggers in the framework.")]
    public bool Verbose;
  }
}
