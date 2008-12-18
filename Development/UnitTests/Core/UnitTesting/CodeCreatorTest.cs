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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class CodeCreatorTest
  {
    [Test]
    public void CreateResultExpectedCodeTest ()
    {
      var result = CodeCreator.CreateResultExpectedCode ("heinz");
      To.ConsoleLine.e (result);

      const string resultExpected =
      #region
 @"
const string resultExpected =
#region
@""heinz"";
#endregion
";
      #endregion

      Assert.That (result, Is.EqualTo (resultExpected));
    }
  }
}