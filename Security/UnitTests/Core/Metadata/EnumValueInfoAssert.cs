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
