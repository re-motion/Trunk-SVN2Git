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

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  public class EnumerationTestBase : TestBase
  {
    protected void CheckEnumerationValueInfos (EnumerationValueInfo[] expected, IEnumerationValueInfo[] actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < expected.Length; i++)
        CheckEnumerationValueInfo (expected[i], actual[i]);
    }

    protected void CheckEnumerationValueInfo (EnumerationValueInfo expected, IEnumerationValueInfo actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.InstanceOfType (expected.GetType()), expected.DisplayName);
      Assert.That (actual.Value, Is.EqualTo (expected.Value), expected.DisplayName);
      Assert.That (actual.Identifier, Is.EqualTo (expected.Identifier), expected.DisplayName);
      Assert.That (actual.IsEnabled, Is.EqualTo (expected.IsEnabled), expected.DisplayName);
      Assert.That (actual.DisplayName, Is.EqualTo (expected.DisplayName), expected.DisplayName);
    }
  }
}
