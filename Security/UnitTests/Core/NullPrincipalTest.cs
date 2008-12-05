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
using System.Security.Principal;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullPrincipalTest
  {
    private IPrincipal _principal;

    [SetUp]
    public void SetUp()
    {
      _principal = new NullPrincipal();
    }

    [Test]
    public void IsInRole()
    {
      Assert.IsFalse (_principal.IsInRole (string.Empty));
    }

    [Test]
    public void GetIdentity()
    {
      Assert.IsInstanceOfType (typeof (NullIdentity), _principal.Identity);
    }
  }
}
