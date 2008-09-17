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

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class CultureTest : DomainTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
    
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitSecurableClassDefinitionWithLocalizedNames (ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Find_Existing ()
    {
      Culture foundCulture = Culture.Find ("de");

      Assert.IsNotNull (foundCulture);
      Assert.AreNotEqual (StateType.New, foundCulture.State);
      Assert.AreEqual ("de", foundCulture.CultureName);
    }

    [Test]
    public void Find_NotExisting ()
    {
      Culture foundCulture = Culture.Find ("hu");

      Assert.IsNull (foundCulture);
    }
  }
}
