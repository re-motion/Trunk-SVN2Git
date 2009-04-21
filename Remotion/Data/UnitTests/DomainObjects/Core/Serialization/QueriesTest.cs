// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.IO;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class QueriesTest : SerializationBaseTest
  {
    [Test]
    public void QueryParameter ()
    {
      QueryParameter queryParameter = new QueryParameter ("name", "value", QueryParameterType.Text);

      QueryParameter deserializedQueryParameter = (QueryParameter) SerializeAndDeserialize (queryParameter);

      AreEqual (queryParameter, deserializedQueryParameter);
    }

    [Test]
    public void QueryParameterCollection ()
    {
      QueryParameterCollection queryParameters = new QueryParameterCollection ();
      queryParameters.Add ("Text Parameter", "Value 1", QueryParameterType.Text);
      queryParameters.Add ("Value Parameter", "Value 2", QueryParameterType.Value);

      QueryParameterCollection deserializedQueryParameters = (QueryParameterCollection) SerializeAndDeserialize (queryParameters);

      AreEqual (queryParameters, deserializedQueryParameters);
    }

    [Test]
    public void QueryDefinition ()
    {
      QueryDefinition queryDefinition = new QueryDefinition ("queryID", "TestDomain", "statement", QueryType.Collection, typeof (DomainObjectCollection));

      QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

      Assert.IsFalse (ReferenceEquals (queryDefinition, deserializedQueryDefinition));
      AreEqual (queryDefinition, deserializedQueryDefinition);
    }

    [Test]
    public void QueryDefinitionInQueryConfiguration ()
    {
      QueryDefinition queryDefinition = DomainObjectsConfiguration.Current.Query.QueryDefinitions["OrderQuery"];

      QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

      Assert.AreSame (queryDefinition, deserializedQueryDefinition);
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException), ExpectedMessage = "QueryDefinition 'UnknownQuery' does not exist.")]
    public void UnknownQueryDefinitionInQueryConfiguration ()
    {
      QueryDefinition unknownQueryDefinition = new QueryDefinition ("UnknownQuery", "TestDomain", "select 42", QueryType.Scalar);
      DomainObjectsConfiguration.Current.Query.QueryDefinitions.Add (unknownQueryDefinition);

      using (MemoryStream stream = new MemoryStream ())
      {
        Serialize (stream, unknownQueryDefinition);
        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (
                DomainObjectsConfiguration.Current.MappingLoader, DomainObjectsConfiguration.Current.Storage, new QueryConfiguration()));

        Deserialize (stream);
      }
    }

    [Test]
    public void QueryDefinitionCollection ()
    {
      QueryDefinitionCollection queryDefinitions = new QueryDefinitionCollection ();
      queryDefinitions.Add (DomainObjectsConfiguration.Current.Query.QueryDefinitions[0]);
      queryDefinitions.Add (DomainObjectsConfiguration.Current.Query.QueryDefinitions[1]);

      QueryDefinitionCollection deserializedQueryDefinitions = (QueryDefinitionCollection) SerializeAndDeserialize (queryDefinitions);
      AreEqual (queryDefinitions, deserializedQueryDefinitions);
      Assert.AreSame (deserializedQueryDefinitions[0], DomainObjectsConfiguration.Current.Query.QueryDefinitions[0]);
      Assert.AreSame (deserializedQueryDefinitions[1], DomainObjectsConfiguration.Current.Query.QueryDefinitions[1]);
    }

    [Test]
    public void Query ()
    {
      var query = (Query) QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      var deserializedQuery = (Query) SerializeAndDeserialize (query);
      AreEqual (query, deserializedQuery);
      Assert.AreSame (DomainObjectsConfiguration.Current.Query.QueryDefinitions["OrderQuery"], deserializedQuery.Definition);
    }

    private void AreEqual (Query expected, Query actual)
    {
      Assert.IsFalse (ReferenceEquals (expected, actual));
      Assert.IsNotNull (actual);

      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreSame (expected.Definition, actual.Definition);
      AreEqual (expected.Parameters, actual.Parameters);
    }

    private void AreEqual (QueryDefinitionCollection expected, QueryDefinitionCollection actual)
    {
      Assert.IsFalse (ReferenceEquals (expected, actual));
      Assert.IsNotNull (actual);
      Assert.AreEqual (expected.Count, actual.Count);

      for (int i = 0; i < expected.Count; i++)
        AreEqual (expected[i], actual[i]);
    }

    private void AreEqual (QueryParameter expected, QueryParameter actual)
    {
      Assert.IsFalse (ReferenceEquals (expected, actual));
      Assert.AreEqual (expected.Name, actual.Name);
      Assert.AreEqual (expected.ParameterType, actual.ParameterType);
      Assert.AreEqual (expected.Value, actual.Value);
    }

    private void AreEqual (QueryParameterCollection expected, QueryParameterCollection actual)
    {
      Assert.IsFalse (ReferenceEquals (expected, actual));
      Assert.AreEqual (expected.Count, actual.Count);
      Assert.AreEqual (expected.IsReadOnly, actual.IsReadOnly);

      for (int i = 0; i < expected.Count; i++)
      {
        AreEqual (expected[i], actual[i]);

        // Check if Hashtable of CommonCollection is deserialized correctly
        QueryParameter actualQueryParameter = actual[i];
        Assert.AreSame (actualQueryParameter, actual[actualQueryParameter.Name]);
      }
    }

    private void AreEqual (QueryDefinition expected, QueryDefinition actual)
    {
      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreEqual (expected.QueryType, actual.QueryType);
      Assert.AreEqual (expected.Statement, actual.Statement);
      Assert.AreEqual (expected.StorageProviderID, actual.StorageProviderID);
      Assert.AreEqual (expected.CollectionType, actual.CollectionType);
    }

  }
}
