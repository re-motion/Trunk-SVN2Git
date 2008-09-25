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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class CodeGenerationTimerTest
  {
    [Test]
    public void Time ()
    {
      TimeSpan timeBefore = CodeGenerationTimer.CodeGenerationTime;
      using (new CodeGenerationTimer ())
      {
        Stopwatch sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < 1) // wait for at least one millisecond
          ;
      }
      Assert.That (CodeGenerationTimer.CodeGenerationTime > timeBefore);
    }
  }
}