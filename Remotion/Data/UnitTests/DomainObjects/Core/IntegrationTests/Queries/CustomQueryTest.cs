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

    [SetUp]
    public new void SetUp ()
    {
      base.SetUp();

      _query = QueryFactory.CreateCustomQuery (
          "CustomQuery",
          TestDomainStorageProviderDefinition,
          "SELECT String, Int16, Boolean, Enum, ExtensibleEnum  FROM [TableWithAllDataTypes]",
          new QueryParameterCollection(),
          null);
    }


    [Test]
    [Ignore ("TODO 4731")]
    public void WithRawValues ()
    {
      var result = ClientTransaction.Current.QueryManager.GetCustom (_query, ExtractCustomObject_RawValues).ToList();

      var exptectedResult = new object[]
                            {
                                new object[] { "üäöfedcba", -32767, 1, 0, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ColorExtensions.Blue" },
                                new object[] { "abcdeföäü", -32767, 0, 1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ColorExtensions.Red" }
                            };

      Assert.That (result.Count, Is.EqualTo (2));
      CollectionAssert.AreEquivalent (exptectedResult, result);
    }

    [Test]
    [Ignore ("TODO 4731")]
    public void WithConvertedValues ()
    {
      var result = ClientTransaction.Current.QueryManager.GetCustom (_query, ExtractCustomObject_ConvertedValues).ToList();

      var exptectedResult = new object[]
                            {
                                new object[] { "üäöfedcba", -32767, true, ClassWithAllDataTypes.EnumType.Value0, Color.Values.Blue() },
                                new object[] { "abcdeföäü", -32767, false, ClassWithAllDataTypes.EnumType.Value1, Color.Values.Red() }
                            };

      Assert.That (result.Count, Is.EqualTo (2));
      CollectionAssert.AreEquivalent (exptectedResult, result);
    }

    [Test]
    [Ignore ("TODO 4731")]
    public void InvokesFilterQueryResultEvent ()
    {
      var transactionExtensionMock = MockRepository.GenerateMock<IClientTransactionExtension>();
      
      var fakeResult = new[] { new object () };
      transactionExtensionMock.Stub (stub => stub.Key).Return ("CustomQuery");
      transactionExtensionMock
        .Expect (mock => mock.FilterCustomQueryResult (Arg.Is(TestableClientTransaction), Arg.Is(_query), Arg<IEnumerable<object>>.Matches (arg => arg.Count () == 2)))
        .Return (fakeResult);
      transactionExtensionMock.Replay ();

      TestableClientTransaction.Extensions.Add (transactionExtensionMock);


      var result = ClientTransaction.Current.QueryManager.GetCustom (_query, ExtractCustomObject_RawValues);

      Assert.That (result, Is.SameAs (fakeResult));
      transactionExtensionMock.VerifyAllExpectations();
    }

    [Test]
    [Ignore ("TODO 4731")]
    public void FromXmlFile ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("CustomQuery");

      var result = QueryManager.GetCustom (query, ExtractCustomObject_RawValues);

      Assert.That (result.Count(), Is.GreaterThan (0));
    }

    private object ExtractCustomObject_ConvertedValues (IQueryResultRow queryResultRow)
    {
      return new object[]
             {
                 queryResultRow.GetConvertedValue<string> (0), queryResultRow.GetConvertedValue<Int16> (1), queryResultRow.GetConvertedValue<bool> (2), 
                 queryResultRow.GetConvertedValue<ClassWithAllDataTypes.EnumType> (3), queryResultRow.GetConvertedValue<Color> (4)
             };
    }

    private object ExtractCustomObject_RawValues (IQueryResultRow queryResultRow)
    {
      return new[]
             {
                 queryResultRow.GetRawValue (0), queryResultRow.GetRawValue (1), queryResultRow.GetRawValue (2), queryResultRow.GetRawValue (3),
                 queryResultRow.GetRawValue (4)
             };
    }
  }
}