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

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  public class ComplexAttribute : Attribute
  {
    public Type T;
    public object[] Os;
    public Type[] Ts;

    private string s;

    public ComplexAttribute ()
    {
    }

    public ComplexAttribute (Type t)
    {
      T = t;
    }

    public ComplexAttribute (string s)
    {
      this.s = s;
    }

    public ComplexAttribute (string s, params object[] os)
    {
      S = s;
      Os = os;
    }

    public ComplexAttribute (Type t, params Type[] ts)
    {
      T = t;
      Ts = ts;
    }

    public string S
    {
      get { return s; }
      set { s = value; }
    }
  }
}
