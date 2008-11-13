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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class ArgumentEmptyExceptionTest
  {
    [Test]
    public void Initialize ()
    {
      var exception = new ArgumentEmptyException ("TheParameter");

      Assert.That (exception.Message, Is.EqualTo ("Parameter 'TheParameter' cannot be empty.\r\nParameter name: TheParameter"));
      Assert.That (exception.ParamName, Is.EqualTo ("TheParameter"));
    }
  }
}