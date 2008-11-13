/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Text.CommandLine;

namespace Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication
{
  /// <summary>
  /// Supplies command line arguments for <see cref="ConsoleApplication{TApplication, TApplicationSettings}"/>|s:
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