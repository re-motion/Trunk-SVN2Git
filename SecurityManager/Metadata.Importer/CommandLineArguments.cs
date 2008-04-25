using System;
using Remotion.Text.CommandLine;

namespace Remotion.SecurityManager.Metadata.Importer
{
  public class CommandLineArguments
  {
    [CommandLineModeArgument (false)]
    public OperationMode Mode;

    [CommandLineStringArgument (false,
        Description = "The name of the XML metadata file.",
        Placeholder = "metadata")]
    public string MetadataFile = string.Empty;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;

    public bool ImportMetadata
    {
      get { return (Mode & OperationMode.Metadata) != 0; }
    }

    public bool ImportLocalization
    {
      get { return (Mode & OperationMode.Localization) != 0; }
    }
  }
}
