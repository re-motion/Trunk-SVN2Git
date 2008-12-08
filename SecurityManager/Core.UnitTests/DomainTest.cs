// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;

namespace Remotion.SecurityManager.UnitTests
{
  public abstract class DomainTest
  {
    protected DomainTest ()
    {
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
    }

    [SetUp]
    public virtual void SetUp ()
    {
    }

    [TearDown]
    public virtual void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope();
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown ()
    {
    }
  }
}
