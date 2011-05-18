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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UberProfIntegration;

namespace Remotion.Data.UnitTests.DomainObjects.UberProfIntegration
{
  [TestFixture]
  public class LinqToSqlListenerFactoryTest : UberProfIntegrationTestBase
  {
    private LinqToSqlListenerFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new LinqToSqlListenerFactory();
    }

    [Test]
    public void CreateClientTransactionListener_RootTransaction ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();

      var result = _factory.CreateClientTransactionListener (clientTransaction);

      Assert.That (result, Is.TypeOf<LinqToSqlListener> ());
      Assert.That (((LinqToSqlListener) result).ClientTransactionID, Is.EqualTo (clientTransaction.ID));
      Assert.That (((LinqToSqlListener) result).AppenderProxy, Is.SameAs (AppenderProxy));
    }

    [Test]
    public void CreateClientTransactionListener_SubTransaction ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ().CreateSubTransaction();

      var result = _factory.CreateClientTransactionListener (clientTransaction);

      Assert.That (result, Is.TypeOf<NullClientTransactionListener> ());
    }
  }
}