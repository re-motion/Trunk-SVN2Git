using System;
using Remotion.Text.CommandLine;

namespace Remotion.SecurityManager.Metadata.Importer
{
  [Flags]
  public enum OperationMode
  {
    [CommandLineMode ("metadata", Description = "Import security metadata.")]
    Metadata = 1,

    [CommandLineMode ("localization", Description = "Import metadata localization (Metadata must have been imported).")]
    Localization = 2,

    [CommandLineMode ("all", Description = "Import security metadata and localization.")]
    All = Metadata | Localization
  }
}
