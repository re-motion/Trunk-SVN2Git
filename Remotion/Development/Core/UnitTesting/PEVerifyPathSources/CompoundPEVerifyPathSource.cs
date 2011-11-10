﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  public class CompoundPEVerifyPathSource : IPEVerifyPathSource
  {
    private readonly IPEVerifyPathSource[] _sources;

    public CompoundPEVerifyPathSource (params IPEVerifyPathSource[] sources)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sources", sources);
      _sources = sources;
    }

    public string GetPEVerifyPath (PEVerifyVersion version)
    {
      return _sources.Select (source => source.GetPEVerifyPath (version)).FirstOrDefault (path => path != null);
    }

    public string GetLookupDiagnostics (PEVerifyVersion version)
    {
      return SeparatedStringBuilder.Build (Environment.NewLine, _sources.Select (source => source.GetLookupDiagnostics (version)));
    }
  }
}