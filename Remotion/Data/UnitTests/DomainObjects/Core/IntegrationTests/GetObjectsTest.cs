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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using System.Collections.Generic;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class GetObjectsTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetObjects_WithMoreThan2100IDs ()
    {
      var ids = new List<ObjectID> ();
      ids.Add (DomainObjectIDs.Order1);
      ids.Add (DomainObjectIDs.Order2);
      ids.AddRange (Enumerable.Range (0, 4000).Select (i => new ObjectID (DomainObjectIDs.Order1.ClassDefinition, Guid.NewGuid ())));

      var result = ClientTransactionMock.TryGetObjects<Order> (ids.ToArray());
      Assert.That (result.Length, Is.EqualTo (4002));
      Assert.That (result.Distinct().ToArray(), Is.EqualTo (new[] { Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2), null }));
    }
  }
}