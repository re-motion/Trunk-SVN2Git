// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

namespace Remotion.UnitTests.Utilities.CustomAttributeUtilityTestDomain
{
  public class AttributeWithPropertyParams : Attribute
  {
    public AttributeWithPropertyParams (int i, string s, object o, Type t, int[] iArray, string[] sArray, object[] oArray, Type[] tArray)
    {
    }

    public int INamed
    {
      get { return 0; }
      set { }
    }

    public string SNamed
    {
      get { return null; }
      set { }
    }

    public object ONamed
    {
      get { return null; }
      set { }
    }

    public Type TNamed
    {
      get { return null; }
      set { }
    }

    public int[] INamedArray
    {
      get { return null; }
      set { }
    }

    public string[] SNamedArray
    {
      get { return null; }
      set { }
    }

    public object[] ONamedArray
    {
      get { return null; }
      set { }
    }

    public Type[] TNamedArray
    {
      get { return null; }
      set { }
    }
  }
}
