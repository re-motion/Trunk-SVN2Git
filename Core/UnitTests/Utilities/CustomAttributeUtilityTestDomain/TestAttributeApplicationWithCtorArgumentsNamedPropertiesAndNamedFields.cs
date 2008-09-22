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
using Remotion.UnitTests.Utilities.CustomAttributeUtilityTestDomain;

namespace Remotion.UnitTests.Utilities.CustomAttributeUtilityTestDomain
{
  [AttributeWithPropertyAndFieldParams (
      1,
      "1",
      null,
      typeof (object),
      new int[] { 2, 3 },
      new string[] { "2", "3" },
      new object[] { null, "foo", typeof (object) }, new Type[] { typeof (string), typeof (int), typeof (double) },
      INamed = 5,
      SNamed = "P5",
      ONamed = "Pbla",
      TNamed = typeof (float),
      INamedArray = new int[] { 1, 2, 3 },
      SNamedArray = new string[] { "P1", null, "P2" },
      ONamedArray = new object[] { 1, 2, null },
      TNamedArray = new Type[] { typeof (Random), null },
      INamedF = 5,
      SNamedF = "5",
      ONamedF = "bla",
      TNamedF = typeof (float),
      INamedArrayF = new int[] { 1, 2, 3 },
      SNamedArrayF = new string[] { "1", null, "2" },
      ONamedArrayF = new object[] { 1, 2, null },
      TNamedArrayF = new Type[] { typeof (Random), null }
      )]
  public class TestAttributeApplicationWithCtorArgumentsNamedPropertiesAndNamedFields
  {
  }
}