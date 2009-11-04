// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
