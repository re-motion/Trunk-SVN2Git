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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class DomainObjectTransactionContextIndexerTest : ClientTransactionBaseTest
  {
    [Test]
    public void Item()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var tx = ClientTransaction.CreateRootTransaction ();
      var indexer = new DomainObjectTransactionContextIndexer (order);

      var item = indexer[tx];
      Assert.That (item, Is.InstanceOfType (typeof (DomainObjectTransactionContext)));
      Assert.That (((DomainObjectTransactionContext) item).DomainObject, Is.SameAs (order));
      Assert.That (((DomainObjectTransactionContext) item).AssociatedTransaction, Is.SameAs (tx));
    }
  }
}