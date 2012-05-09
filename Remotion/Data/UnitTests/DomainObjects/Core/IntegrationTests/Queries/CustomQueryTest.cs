// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Queries
{
  [TestFixture]
  public class CustomQueryTest : QueryTestBase
  {
    private IQuery _query;

    public override void SetUp ()
    {
      base.SetUp();

      _query = QueryFactory.CreateCustomQuery (
          "CustomQuery",
          TestDomainStorageProviderDefinition,
          "SELECT String, Int16, Boolean, Enum, ExtensibleEnum FROM [TableWithAllDataTypes]",
          new QueryParameterCollection());
    }

    [Test]
    public void WithRawValues ()
    {
      var result = QueryManager.GetCustom (_query, QueryResultRowTestHelper.ExtractRawValues).ToList();

      var expected = new object[]
                            {
                                new object[] { "���fedcba", -32767, true, 0, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ColorExtensions.Blue" },
                                new object[] { "abcdef���", 32767, false, 1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ColorExtensions.Red" }
                            };

      Assert.That (result, Is.EquivalentTo (expected));
    }

    [Test]
    public void WithConvertedValues ()
    {
      var result = QueryManager.GetCustom (
          _query,
          queryResultRow => new object[]
                            {
                                queryResultRow.GetConvertedValue<string> (0),
                                queryResultRow.GetConvertedValue<Int16> (1),
                                queryResultRow.GetConvertedValue<bool> (2),
                                queryResultRow.GetConvertedValue<ClassWithAllDataTypes.EnumType> (3),
                                queryResultRow.GetConvertedValue<Color> (4)
                            }).ToList();

      var expected =
          new[]
          {
              new object[]
              {
                  "���fedcba",
                  -32767,
                  true,
                  ClassWithAllDataTypes.EnumType.Value0,
                  Color.Values.Blue()
              },
              new object[]
              {
                  "abcdef���",
                  32767,
                  false,
                  ClassWithAllDataTypes.EnumType.Value1,
                  Color.Values.Red()
              },
          };

      Assert.That (result, Is.EquivalentTo (expected));
    }

    [Test]
    public void InvokesFilterQueryResultEvent ()
    {
      var transactionExtensionMock = MockRepository.GenerateMock<IClientTransactionExtension>();
      
      var fakeResult = new[] { new object[0] };
      transactionExtensionMock.Stub (stub => stub.Key).Return ("CustomQueryExtension");
      transactionExtensionMock
          .Expect (
              mock => mock.FilterCustomQueryResult (
                  Arg.Is (TestableClientTransaction), Arg.Is(_query), Arg<IEnumerable<object[]>>.Matches(e=>e.Count()==2)))
          .Return (fakeResult);
      transactionExtensionMock.Replay ();

      TestableClientTransaction.Extensions.Add (transactionExtensionMock);
      
      var result = QueryManager.GetCustom (_query, QueryResultRowTestHelper.ExtractRawValues);

      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void FromXmlFile ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomQuery");

      var result = QueryManager.GetCustom (query, QueryResultRowTestHelper.ExtractRawValues);

      Assert.That (result.Count(), Is.EqualTo (2));
    }
  }
}