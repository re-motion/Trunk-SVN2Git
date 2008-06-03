/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
