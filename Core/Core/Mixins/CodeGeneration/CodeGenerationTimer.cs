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
using System.Diagnostics;
using System.Threading;
using Remotion.Logging;

namespace Remotion.Mixins.CodeGeneration
{
  public class CodeGenerationTimer : IDisposable
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ConcreteTypeBuilder));

    private static long s_codeGenerationTime = 0;

    public static TimeSpan CodeGenerationTime
    {
      get
      {
        long codeGenerationTime = Interlocked.Read (ref s_codeGenerationTime);
        return TimeSpan.FromMilliseconds (codeGenerationTime);
      }
    }

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public void Dispose ()
    {
      _stopwatch.Stop();
      long elapsed = _stopwatch.ElapsedMilliseconds;
      Interlocked.Add (ref s_codeGenerationTime, elapsed);

      s_log.InfoFormat ("Code generation: {0} ms.", elapsed);
    }
  }
}
