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
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests;
using Remotion.Development.UnitTesting;
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
      Assert.That (clientTransaction, Is.InstanceOfType (typeof (RootClientTransaction)));
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
      Assert.That (clientTransaction, Is.InstanceOfType (typeof (RootClientTransaction)));
      Assert.That (clientTransaction.Extensions, Is.Empty);
    }

    [Test]
    public void OnTransactionCreated ()
    {
      ITransactionFactory factory = MockRepository.GenerateMock<SecurityClientTransactionFactory> ();

      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension>();
      factory.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnTransactionCreated", Arg<ClientTransaction>.Is.NotNull)).Do (
          invocation => ((ClientTransaction) invocation.Arguments[0]).Extensions.Add ("extension", extensionStub));

      ITransaction transaction = factory.CreateRootTransaction ();

      var clientTransaction = transaction.To<ClientTransaction> ();
      Assert.That (clientTransaction, Is.InstanceOfType (typeof (RootClientTransaction)));
      Assert.That (clientTransaction.Extensions.Count, Is.EqualTo (1));
      Assert.That (clientTransaction.Extensions[0], Is.SameAs (extensionStub));
    }

    [Test]
    public void CreateChildTransaction_WithEnabledSecurity ()
    {
      ITransactionFactory factory = new SecurityClientTransactionFactory ();

      _testHelper.SetupSecurityConfiguration ();
      SecurityConfiguration.Current.SecurityProvider.BackToRecord ();
      SecurityConfiguration.Current.SecurityProvider.Stub (stub => stub.IsNull).Return (false).Repeat.Any ();
      SecurityConfiguration.Current.SecurityProvider.Replay ();
      ITransaction rootTransaction;
      ITransaction childTransaction;
      try
      {
        Assert.That (SecurityConfiguration.Current.SecurityProvider.IsNull, Is.False);
        rootTransaction = factory.CreateRootTransaction ();
        childTransaction = rootTransaction.CreateChild();
      }
      finally
      {
        _testHelper.TearDownSecurityConfiguration ();
      }

      var clientTransaction = childTransaction.To<ClientTransaction> ();
      Assert.That (rootTransaction.To<ClientTransaction> ().Extensions[0], Is.InstanceOfType (typeof (SecurityClientTransactionExtension)));
      Assert.That (clientTransaction.Extensions, Is.EqualTo (rootTransaction.To<ClientTransaction> ().Extensions));
    }
  }
}