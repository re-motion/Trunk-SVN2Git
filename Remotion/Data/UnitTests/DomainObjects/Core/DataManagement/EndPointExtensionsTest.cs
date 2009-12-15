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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class EndPointExtensionsTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetDomainObject ()
    {
      var endPointStub = MockRepository.GenerateStub<IEndPoint> ();
      endPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);

      var domainObject = endPointStub.GetDomainObject ();

      Assert.That (domainObject, Is.SameAs (Order.GetObject (DomainObjectIDs.Order1)));
    }

    [Test]
    public void GetDomainObject_Null ()
    {
      var endPointStub = MockRepository.GenerateStub<IEndPoint> ();
      endPointStub.Stub (stub => stub.ObjectID).Return (null);
      endPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);

      var domainObject = endPointStub.GetDomainObject ();

      Assert.That (domainObject, Is.Null);
    }

    [Test]
    public void GetDomainObject_Deleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();

      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      var endPointStub = MockRepository.GenerateStub<IEndPoint> ();
      endPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);

      var domainObject = endPointStub.GetDomainObject ();

      Assert.That (domainObject, Is.SameAs (order1));
    }
  }
}