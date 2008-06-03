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

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.MetadataObjectTests
{
  [TestFixture]
  public class FindTest : DomainTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitSecurableClassDefinitionWithStates (ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Find_ValidSimpleMetadataObjectID ()
    {
      string metadataObjectID = "b8621bc9-9ab3-4524-b1e4-582657d6b420";

      MetadataObject metadataObject = MetadataObject.Find (metadataObjectID);

      Assert.IsInstanceOfType (typeof (SecurableClassDefinition), metadataObject);
    }

    [Test]
    public void Find_NotExistingSimpleMetadataObjectID ()
    {
      string metadataObjectID = "38777218-cd4d-45ca-952d-c10b1104996a";

      MetadataObject metadataObject = MetadataObject.Find (metadataObjectID);

      Assert.IsNull (metadataObject);
    }

    [Test]
    public void Find_ValidStateByMetadataObjectID ()
    {
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|2";

      MetadataObject metadataObject = MetadataObject.Find (metadataObjectID);

      Assert.IsInstanceOfType (typeof (StateDefinition), metadataObject);
      StateDefinition state = (StateDefinition) metadataObject;
      Assert.AreEqual ("Reaccounted", state.Name);
      Assert.AreEqual (2, state.Value);
    }

    [Test]
    public void Find_NotExistingStateDefinition ()
    {
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|42";

      MetadataObject metadataObject = MetadataObject.Find (metadataObjectID);

      Assert.IsNull (metadataObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The metadata ID 'Hello|42' is invalid.\r\nParameter name: metadataID")]
    public void Find_InvalidMetadataItemID ()
    {
      string metadataObjectID = "Hello|42";

      MetadataObject metadataObject = MetadataObject.Find (metadataObjectID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The metadata ID '9e689c4c-3758-436e-ac86-23171289fa5e|Hello' is invalid.\r\nParameter name: metadataID")]
    public void Find_InvalidStateValue ()
    {
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|Hello";

      MetadataObject metadataObject = MetadataObject.Find (metadataObjectID);
    }
  }
}
