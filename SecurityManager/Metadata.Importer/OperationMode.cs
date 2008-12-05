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
