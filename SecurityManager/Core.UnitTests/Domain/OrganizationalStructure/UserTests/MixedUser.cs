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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class MixedUser : UserTestBase
  {
    public interface ITestInterface
    {
    }

    public class TestMixin : Mixin<User>, ITestInterface
    {
    }

    [Test]
    public void MixedUserTest ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (User)).Clear().AddMixins (typeof (TestMixin)).EnterScope())
      {
        User user = CreateUser();
        Assert.IsNotNull (Mixin.Get<TestMixin> (user));
        Assert.IsTrue (user is ITestInterface);
      }
    }
  }
}
