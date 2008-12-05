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
  public static class EnumValueInfoAssert
  {
    public static void Contains (string expectedName, IList<EnumValueInfo> list, string message, params object[] args)
    {
#pragma warning disable 612,618 // Asserters are obsolete
      Assert.DoAssert (new EnumValueInfoListContentsAsserter(expectedName, list, message, args));
#pragma warning restore 612,618
    }

    public static void Contains (string expectedName, IList<EnumValueInfo> list, string message)
    {
      Contains (expectedName, list, message, null);
    }

    public static void Contains (string expectedName, IList<EnumValueInfo> list)
    {
      Contains (expectedName, list, string.Empty, null);
    }
  }
}
