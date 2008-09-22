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

namespace Remotion.UnitTests.Utilities.CustomAttributeUtilityTestDomain
{
  public class AttributeWithFieldParams : Attribute
  {
    public AttributeWithFieldParams (int i, string s, object o, Type t, int[] iArray, string[] sArray, object[] oArray, Type[] tArray)
    {
    }

    public int INamedF;
    public string SNamedF;
    public object ONamedF;
    public Type TNamedF;

    public int[] INamedArrayF;
    public string[] SNamedArrayF;
    public object[] ONamedArrayF;
    public Type[] TNamedArrayF;
  }
}