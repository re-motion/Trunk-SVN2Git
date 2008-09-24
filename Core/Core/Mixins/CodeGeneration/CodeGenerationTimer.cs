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