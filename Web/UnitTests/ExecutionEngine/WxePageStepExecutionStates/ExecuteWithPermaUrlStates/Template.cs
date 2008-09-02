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
using NUnit.Framework;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  [TestFixture][Ignore]
  public class Template
  {
    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void RedirectToSubFunction ()
    {
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ExecuteSubFunction ()
    {
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ReturnFromSubFunction ()
    {
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void PostProcessSubFunction ()
    {
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Cleanup ()
    {
    }
  }
}