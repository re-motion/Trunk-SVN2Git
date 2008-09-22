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
  public class ComplexAttributeTarget
  {
    [Complex ()]
    public void DefaultCtor ()
    {
    }

    [Complex (S = "foo")]
    public void DefaultCtorWithProperty ()
    {
    }

    [Complex (T = typeof (object))]
    public void DefaultCtorWithField ()
    {
    }

    [Complex (typeof (void), S = "string")]
    public void CtorWithTypeAndProperty ()
    {
    }

    [Complex ("s", 1, 2, 3, "4")]
    public void CtorWithStringAndParamsArray ()
    {
    }

    [Complex (typeof (double), typeof (int), typeof (string))]
    public void CtorWithStringAndTypeParamsArray ()
    {
    }

    [Complex (new int[] {1, 2, 3})]
    public void CtorWithIntArray ()
    {
    }

    [Complex (new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday })]
    public void CtorWithEnumArray ()
    {
    }
  }
}