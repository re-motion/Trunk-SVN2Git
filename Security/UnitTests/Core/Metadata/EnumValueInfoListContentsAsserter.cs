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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata
{
#pragma warning disable 612,618 // Asserters are obsolete
  public class EnumValueInfoListContentsAsserter : AbstractAsserter
#pragma warning restore 612,618
  {
    // types

    // static members

    // member fields

    private string _expectedName;
    private IList<EnumValueInfo> _list;

    // construction and disposing

    public EnumValueInfoListContentsAsserter (string expectedName, IList<EnumValueInfo> list, string message, params object[] args)
      : base (message, args)
    {
      _expectedName = expectedName;
      _list = list;
    }

    // methods and properties

    public override bool Test ()
    {
      if (_list != null)
      {
        foreach (EnumValueInfo value in _list)
        {
          if (string.Equals (value.Name, _expectedName, StringComparison.Ordinal))
            return true;
        }
      }

      return false;
    }

    public override string Message
    {
      get
      {
        FailureMessage.DisplayExpectedValue (_expectedName);
        FailureMessage.DisplayListElements ("\t but was: ", ExtractNames (_list), 0, 10);

        return base.FailureMessage.ToString ();
      }
    }

    private List<string> ExtractNames (IList<EnumValueInfo> list)
    {
      if (_list == null)
        return null;

      List<string> actualNames = new List<string> ();
      foreach (EnumValueInfo value in list)
        actualNames.Add (value.Name);

      return actualNames;
    }
  }
}
