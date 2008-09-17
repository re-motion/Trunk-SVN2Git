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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class TestWithEmptyDomain : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.FindAll();

      Assert.AreEqual (0, result.Count);
    }
  }
}