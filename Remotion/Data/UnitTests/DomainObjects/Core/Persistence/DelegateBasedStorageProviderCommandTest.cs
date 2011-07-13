// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
{
  [TestFixture]
  public class DelegateBasedStorageProviderCommandTest
  {
    [Test]
    public void Execute ()
    {
      var executionContext = new object();
      var commandStub = MockRepository.GenerateStub<IStorageProviderCommand<IEnumerable<string>, object>>();
      var providerCommand = new DelegateBasedStorageProviderCommand<IEnumerable<string>, IEnumerable<int>, object> (
          commandStub, s => s.Select (r => r.Count()));

      commandStub.Stub (stub => stub.Execute (executionContext)).Return (new[] { "Test1", "TestTest2" });

      var result = providerCommand.Execute (executionContext).ToArray();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.EqualTo (5));
      Assert.That (result[1], Is.EqualTo (9));
    }
  }
}