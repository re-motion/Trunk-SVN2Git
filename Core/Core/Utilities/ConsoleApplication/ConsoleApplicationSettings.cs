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
using Remotion.Text.CommandLine;

namespace Remotion.Utilities.ConsoleApplication
{
  /// <summary>
  /// Supplies command line arguments for <see cref="ConsoleApplication{TApplication,TApplicationSettings}"/>|s:
  /// <para>/? ... output usage information</para>
  /// <para>/wait+ ... wait for a keypress at the end of program execution</para>
  /// </summary>
  public class ConsoleApplicationSettings
  {
    // "/?"-command-line-switch outputs usage information.
    public enum ShowUsageMode
    {
      [CommandLineMode ("?", Description = "Show usage")]
      ShowUsage
    };

    // Assign non-enum value != to ShowUsageMode.ShowUsage here, to default to not show usage.
    // Introducing an extra enum value for this state, would lead to it being listed in the "usage" output, which is not what we want.
    [CommandLineModeArgument (true)]
    public ShowUsageMode Mode = (ShowUsageMode) 987654321;

    [CommandLineFlagArgument ("wait", false, Description = "Wait for keypress at end of program execution.")]
    public bool WaitForKeypress;
  }
}
