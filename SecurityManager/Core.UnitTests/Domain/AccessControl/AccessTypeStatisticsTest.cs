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
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessTypeStatisticsTest 
  {
    private AccessControlTestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope ();
    }


    [Test]
    public void AddAccessTypesSupplyingAceAndIsInAccessTypesSupplyingAcesTest ()
    {
      AccessTypeStatistics accessTypeStatistics = new AccessTypeStatistics();
      var ace = _testHelper.CreateAceWithAbstractRole();
      var ace2 = _testHelper.CreateAceWithoutGroupCondition();

      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace), Is.False);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace2), Is.False);

      accessTypeStatistics.AddAccessTypesContributingAce (ace);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace2), Is.False);

      accessTypeStatistics.AddAccessTypesContributingAce (ace2);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInAccessTypesContributingAces (ace2), Is.True);
    }


    [Test]
    public void AddMatchingAceAndIsInMatchingAcesTest ()
    {
      AccessTypeStatistics accessTypeStatistics = new AccessTypeStatistics ();
      var ace = _testHelper.CreateAceWithAbstractRole ();
      var ace2 = _testHelper.CreateAceWithoutGroupCondition ();

      Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.False);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace2), Is.False);

      accessTypeStatistics.AddMatchingAce (ace);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace2), Is.False);

      accessTypeStatistics.AddMatchingAce (ace2);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace), Is.True);
      Assert.That (accessTypeStatistics.IsInMatchingAces (ace2), Is.True);
    }

  }
}