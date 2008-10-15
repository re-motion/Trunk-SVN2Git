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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.Data.UnitTesting.DomainObjects.Linq;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class FindMetadataObjectQueryBuilderTest : DomainTest
  {
    private FindMetadataObjectQueryBuilder _queryBuilder;
    private ExpressionTreeComparer _expressionTreeComparer;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _expressionTreeComparer = new ExpressionTreeComparer ((actual, exptected) => Assert.That (actual, Is.EqualTo (exptected)));
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _queryBuilder = new FindMetadataObjectQueryBuilder();
    }

    [Test]
    public void CreateQuery_ForMetadataObject ()
    {
      string metadataObjectID = "b8621bc9-9ab3-4524-b1e4-582657d6b420";

      var expected = from m in QueryFactory.CreateLinqQuery<MetadataObject>()
                     where m.MetadataItemID == new Guid (metadataObjectID)
                     select m;

      var actual = _queryBuilder.CreateQuery (metadataObjectID);

      _expressionTreeComparer.Compare (expected, actual);
    }

    [Test]
    public void CreateQuery_ForStateDefinition ()
    {
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|2";

      var expected = from state in QueryFactory.CreateLinqQuery<StateDefinition>()
                     where state.StateProperty.MetadataItemID == new Guid ("9e689c4c-3758-436e-ac86-23171289fa5e") && state.Value == 2
                     select state;

      var actual = _queryBuilder.CreateQuery (metadataObjectID);

      _expressionTreeComparer.Compare (expected, actual.Cast<StateDefinition>());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The metadata ID 'Hello|42' is invalid.\r\nParameter name: metadataID")]
    public void Find_InvalidMetadataItemID ()
    {
      string metadataObjectID = "Hello|42";

      _queryBuilder.CreateQuery (metadataObjectID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The metadata ID '9e689c4c-3758-436e-ac86-23171289fa5e|Hello' is invalid.\r\nParameter name: metadataID")]
    public void Find_InvalidStateValue ()
    {
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|Hello";

      _queryBuilder.CreateQuery (metadataObjectID);
    }
  }
}