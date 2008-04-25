using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Text.CommandLine;

namespace Remotion.Mixins.MixerTool
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
  }
}
