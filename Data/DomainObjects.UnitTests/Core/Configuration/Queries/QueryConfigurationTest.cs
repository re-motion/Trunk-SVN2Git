using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Queries
{
  [TestFixture]
  public class QueryConfigurationTest : StandardMappingTest
  {
    [Test]
    public void Loading ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"Core\\QueriesForLoaderTest.xml");
      QueryDefinitionCollection actualQueries = loader.GetQueryDefinitions ();
      QueryDefinitionCollection expectedQueries = CreateExpectedQueryDefinitions ();

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, actualQueries);
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException),
        ExpectedMessage = "A scalar query 'OrderSumQuery' must not specify a collectionType.")]
    public void ScalarQueryWithCollectionType ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"Core\\ScalarQueryWithCollectionType.xml");
      loader.GetQueryDefinitions ();
    }

    [Test]
    public void QueryConfigurationWithInvalidNamespace ()
    {
      string configurationFile = "Core\\QueriesWithInvalidNamespace.xml";
      try
      {
        QueryConfigurationLoader loader = new QueryConfigurationLoader (configurationFile);

        Assert.Fail ("QueryConfigurationException was expected");
      }
      catch (QueryConfigurationException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading query configuration: The namespace 'http://www.re-motion.org/Data/DomainObjects/InvalidNamespace' of"
            + " the root element is invalid. Expected namespace: 'http://www.re-motion.org/Data/DomainObjects/Queries/1.0'. File: '{0}'.",
            Path.GetFullPath (configurationFile));

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void Deserialize_WithQueryFiles ()
    {
      string xmlFragment =
          @"<query>
              <queryFiles>
                <add filename=""..\..\myqueries1.xml""/>
                <add filename=""..\..\myqueries2.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (2));
      Assert.That (configuration.QueryFiles[0].FileName, Is.EqualTo (@"..\..\myqueries1.xml"));
      Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries1.xml")));
      Assert.That (configuration.QueryFiles[1].FileName, Is.EqualTo (@"..\..\myqueries2.xml"));
      Assert.That (configuration.QueryFiles[1].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries2.xml")));
    }

    [Test]
    public void DefaultQueryFile ()
    {
      QueryConfiguration configuration = new QueryConfiguration ();

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (0));
      Assert.That (configuration.QueryDefinitions.Count, Is.GreaterThan (0));

      Assert.That (QueryConfiguration.DefaultConfigurationFile, Is.EqualTo (Path.Combine (Environment.CurrentDirectory, "queries.xml")));

      QueryConfigurationLoader loader = new QueryConfigurationLoader (QueryConfiguration.DefaultConfigurationFile);
      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (loader.GetQueryDefinitions (), configuration.QueryDefinitions);
    }

    [Test]
    public void Deserialize_WithNonUniqueNames ()
    {
      string xmlFragment =
          @"<query>
              <queryFiles>
                <add filename=""..\..\myqueries1.xml""/>
                <add filename=""..\..\myqueries1.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      // unfortunately, this silently works because identical elements are not considered duplicates
    }

    [Test]
    public void QueryConfiguration_WithFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");

      Assert.AreEqual (1, configuration.QueryFiles.Count);
      Assert.AreEqual ("QueriesForLoaderTest.xml", configuration.QueryFiles[0].FileName);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "QueriesForLoaderTest.xml"), configuration.QueryFiles[0].RootedFileName);
    }

    [Test]
    public void QueryConfiguration_WithRootedFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration (@"c:\QueriesForLoaderTest.xml");

      Assert.AreEqual (1, configuration.QueryFiles.Count);
      Assert.AreEqual (@"c:\QueriesForLoaderTest.xml", configuration.QueryFiles[0].FileName);
    }

    [Test]
    public void QueryConfiguration_WithMultipleFileNames ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("Q1.xml", "Q2.xml");

      Assert.AreEqual (2, configuration.QueryFiles.Count);
      Assert.AreEqual ("Q1.xml", configuration.QueryFiles[0].FileName);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "Q1.xml"), configuration.QueryFiles[0].RootedFileName);
      Assert.AreEqual ("Q2.xml", configuration.QueryFiles[1].FileName);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "Q2.xml"), configuration.QueryFiles[1].RootedFileName);
    }

    [Test]
    public void GetDefinitions ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("Core\\QueriesForLoaderTest.xml");

      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"Core\\QueriesForLoaderTest.xml");
      QueryDefinitionCollection expectedQueries = loader.GetQueryDefinitions ();

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefinitions_WithMultipleFiles ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("Core\\QueriesForLoaderTest.xml", "Core\\QueriesForLoaderTest2.xml");

      QueryConfigurationLoader loader1 = new QueryConfigurationLoader (@"Core\\QueriesForLoaderTest.xml");
      QueryConfigurationLoader loader2 = new QueryConfigurationLoader (@"Core\\QueriesForLoaderTest2.xml");
      QueryDefinitionCollection expectedQueries = loader1.GetQueryDefinitions ();
      expectedQueries.Merge (loader2.GetQueryDefinitions());

      Assert.IsTrue (expectedQueries.Count > loader1.GetQueryDefinitions ().Count);

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefinitions_UsesRootedPath ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("Core\\QueriesForLoaderTest.xml");
      string oldDirectory = Environment.CurrentDirectory;
      try
      {
        Environment.CurrentDirectory = @"c:\";
        Assert.IsNotEmpty (configuration.QueryDefinitions);
      }
      finally
      {
        Environment.CurrentDirectory = oldDirectory;
      }
    }

    [Test]
    public void CollectionType_SupportsTypeUtilityNotation ()
    {
      QueryDefinitionCollection queries = DomainObjectsConfiguration.Current.Query.QueryDefinitions;
      Assert.AreSame (typeof (SpecificOrderCollection), queries["QueryWithSpecificCollectionType"].CollectionType);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = @"File '.*QueriesForLoaderTestDuplicate.xml' defines a duplicate "
        + @"for query definition 'OrderQueryWithCustomCollectionType'.", MatchType = MessageMatch.Regex)]
    public void DifferentQueryFiles_SpecifyingDuplicates ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("Core\\QueriesForLoaderTest.xml", "Core\\QueriesForLoaderTestDuplicate.xml");

      Dev.Null = configuration.QueryDefinitions;
    }

    private QueryDefinitionCollection CreateExpectedQueryDefinitions ()
    {
      QueryDefinitionCollection queries = new QueryDefinitionCollection ();

      queries.Add (QueryFactory.CreateOrderQueryWithCustomCollectionType ());
      queries.Add (QueryFactory.CreateOrderQueryDefinitionWithObjectListOfOrder ());
      queries.Add (QueryFactory.CreateCustomerTypeQueryDefinition ());
      queries.Add (QueryFactory.CreateOrderSumQueryDefinition ());

      return queries;
    }
  }
}
