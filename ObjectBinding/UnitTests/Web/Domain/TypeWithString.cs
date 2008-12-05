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
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithString
  {
    public static TypeWithString Create ()
    {
      return ObjectFactory.Create<TypeWithString> (true).With ();
    }

    public static TypeWithString Create (string firstValue, string secondValue)
    {
      return ObjectFactory.Create<TypeWithString> (true).With (firstValue, secondValue);
    }

    private string _stringValue;
    private string[] _stringArray;
    private string _firstValue;
    private string _secondValue;

    protected TypeWithString ()
    {
    }

    protected TypeWithString (string firstValue, string secondValue)
    {
      _firstValue = firstValue;
      _secondValue = secondValue;
    }

    public string StringValue
    {
      get { return _stringValue; }
      set { _stringValue = value; }
    }

    public string[] StringArray
    {
      get { return _stringArray; }
      set { _stringArray = value; }
    }

    public string FirstValue
    {
      get { return _firstValue; }
      set { _firstValue = value; }
    }

    public string SecondValue
    {
      get { return _secondValue; }
      set { _secondValue = value; }
    }
  }
}
