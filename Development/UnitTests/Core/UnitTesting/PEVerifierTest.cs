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
using Remotion.Development.UnitTesting;
using System.IO;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class PEVerifierTest
  {
    [SetUp]
    public void SetUp()
    {
      PEVerifier.PEVerifyPath = PEVerifier.DefaultPEVerifyPath;
    }

    [TearDown]
    public void TearDown ()
    {
      PEVerifier.PEVerifyPath = PEVerifier.DefaultPEVerifyPath;
    }

    [Test]
    public void PEVerifyPath ()
    {
      Assert.That (File.Exists (PEVerifier.PEVerifyPath));
    }

    [Test]
    public void VerifyMSCorlib ()
    {
      PEVerifier.VerifyPEFile (typeof (object).Assembly);
    }

    [Test]
    [ExpectedException (typeof (PEVerifyException), ExpectedMessage = "PEVerify returned 1", MatchType = MessageMatch.Contains)]
    public void VerifyInvalidPath ()
    {
      PEVerifier.VerifyPEFile ("Foobar whatever");
    }

    [Test]
    [ExpectedException (typeof (PEVerifyException), ExpectedMessage = "PEVerify could not be found at path 'Foobar whatever'.")]
    public void VerifyWithPEVerifyNotFound ()
    {
      PEVerifier.PEVerifyPath = "Foobar whatever";
      PEVerifier.VerifyPEFile (typeof (object).Assembly);
    }
  }
}
