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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests;
using Remotion.Security.Configuration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Security
{
  [TestFixture]
  public class SecurityClientTransactionFactoryTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
    }

    [Test]
    public void CreateRootTransaction_WithEnabledSecurity ()
    {
      ITransactionFactory factory = new SecurityClientTransactionFactory();

      _testHelper.SetupSecurityConfiguration();
      SecurityConfiguration.Current.SecurityProvider.BackToRecord();
      SecurityConfiguration.Current.SecurityProvider.Stub (stub => stub.IsNull).Return (false).Repeat.Any();
      SecurityConfiguration.Current.SecurityProvider.Replay();
      ITransaction transaction;
      try
      {
        Assert.That (SecurityConfiguration.Current.SecurityProvider.IsNull, Is.False);
        transaction = factory.CreateRootTransaction ();
      }
      finally
      {
        _testHelper.TearDownSecurityConfiguration();
      }

      var clientTransaction = transaction.To<ClientTransaction>();
      var dataSource = ClientTransactionTestHelper.GetDataSourceStrategy (clientTransaction);
      Assert.That (dataSource, Is.InstanceOfType (typeof (RootClientTransaction)));
      Assert.That (clientTransaction.Extensions.Count, Is.EqualTo (1));
      Assert.That (clientTransaction.Extensions[0], Is.InstanceOfType (typeof (SecurityClientTransactionExtension)));
      Assert.That (clientTransaction.Extensions[typeof (SecurityClientTransactionExtension).FullName], Is.SameAs (clientTransaction.Extensions[0]));
    }

    [Test]
    public void CreateRootTransaction_WithDisabledSecurity ()
    {
      ITransactionFactory factory = new SecurityClientTransactionFactory ();

      Assert.That (SecurityConfiguration.Current.SecurityProvider.IsNull, Is.True);
      ITransaction transaction = factory.CreateRootTransaction ();

      var clientTransaction = transaction.To<ClientTransaction> ();
      var dataSource = ClientTransactionTestHelper.GetDataSourceStrategy (clientTransaction);
      Assert.That (dataSource, Is.InstanceOfType (typeof (RootClientTransaction)));
      Assert.That (clientTransaction.Extensions, Is.Empty);
    }
  }
}
