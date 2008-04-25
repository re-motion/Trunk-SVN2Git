using System;
using System.IO;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.Web.UnitTests.ExecutionEngine.UrlMapping
{

  [TestFixture]
  public class UrlMappingConfigurationTest
  {
    [SetUp]
    public virtual void SetUp ()
    {
      UrlMappingConfiguration.SetCurrent (null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
    }

    [Test]
    public void LoadMappingFromFile ()
    {
      UrlMappingConfiguration mapping = UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMapping.xml");

      Assert.IsNotNull (mapping, "Mapping is null.");

      Assert.IsNotNull (mapping.Mappings, "Rules are null.");
      Assert.AreEqual (3, mapping.Mappings.Count);

      Assert.IsNotNull (mapping.Mappings[0], "First rule is null.");
      Assert.IsNotNull (mapping.Mappings[1], "Second rule is null.");
      Assert.IsNotNull (mapping.Mappings[2], "Thrid rule is null.");

      Assert.AreEqual ("First", mapping.Mappings[0].ID);
      Assert.AreEqual (typeof (FirstMappedFunction), mapping.Mappings[0].FunctionType);
      Assert.AreEqual ("~/First.wxe", mapping.Mappings[0].Resource);

      Assert.AreEqual ("Second", mapping.Mappings[1].ID);
      Assert.AreEqual (typeof (SecondMappedFunction), mapping.Mappings[1].FunctionType);
      Assert.AreEqual ("~/Second.wxe", mapping.Mappings[1].Resource);

      Assert.IsNull (mapping.Mappings[2].ID);
      Assert.AreEqual (typeof (FirstMappedFunction), mapping.Mappings[2].FunctionType);
      Assert.AreEqual ("~/Primary.wxe", mapping.Mappings[2].Resource);
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException))]
    public void LoadMappingFromFileWithInvalidFilename ()
    {
      UrlMappingConfiguration mapping = UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\InvalidFilename.xml");
    }

    [Test]
    public void GetCurrentMapping ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineUrlMapping ();
      UrlMappingConfiguration mapping = UrlMappingConfiguration.Current;
      Assert.IsNotNull (mapping);
    }

    [Test]
    public void GetCurrentMappingFromConfiguration ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineUrlMapping ();
      UrlMappingConfiguration mapping = UrlMappingConfiguration.Current;

      Assert.IsNotNull (mapping, "Mapping is null.");

      Assert.IsNotNull (mapping.Mappings, "Rules are null.");
      Assert.AreEqual (3, mapping.Mappings.Count);

      Assert.IsNotNull (mapping.Mappings[0], "First rule is null.");
      Assert.IsNotNull (mapping.Mappings[1], "Second rule is null.");
      Assert.IsNotNull (mapping.Mappings[2], "Thrid rule is null.");

      Assert.AreEqual ("First", mapping.Mappings[0].ID);
      Assert.AreEqual (typeof (FirstMappedFunction), mapping.Mappings[0].FunctionType);
      Assert.AreEqual ("~/First.wxe", mapping.Mappings[0].Resource);

      Assert.AreEqual ("Second", mapping.Mappings[1].ID);
      Assert.AreEqual (typeof (SecondMappedFunction), mapping.Mappings[1].FunctionType);
      Assert.AreEqual ("~/Second.wxe", mapping.Mappings[1].Resource);

      Assert.IsNull (mapping.Mappings[2].ID);
      Assert.AreEqual (typeof (FirstMappedFunction), mapping.Mappings[2].FunctionType);
      Assert.AreEqual ("~/Primary.wxe", mapping.Mappings[2].Resource);
    }

    [Test]
    public void GetCurrentMappingFromConfigurationWithNoFilemane ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineMappingWithNoFilename ();
      UrlMappingConfiguration mapping = UrlMappingConfiguration.Current;

      Assert.IsNotNull (mapping, "Mapping is null.");

      Assert.IsNotNull (mapping.Mappings, "Rules are null.");
      Assert.AreEqual (0, mapping.Mappings.Count);
    }

    [Test]
    public void FindByFunctionType ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineUrlMapping ();
      UrlMappingCollection mappings = UrlMappingConfiguration.Current.Mappings;

      UrlMappingEntry entry = mappings[0];
      Assert.AreSame (mappings[0], mappings.Find (entry.FunctionType), "Could not find {0}.", entry.FunctionType.FullName);

      entry = mappings[1];
      Assert.AreSame (mappings[1], mappings.Find (entry.FunctionType), "Could not find {0}.", entry.FunctionType.FullName);

      entry = mappings[2];
      Assert.AreSame (mappings[0], mappings.Find (entry.FunctionType), "Could not find {0}.", entry.FunctionType.FullName);

      Assert.IsNull (mappings.Find (typeof (UnmappedFunction)), "Found mapping for unmapped function.");
    }

    [Test]
    public void FindByResource ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineUrlMapping ();
      UrlMappingCollection mappings = UrlMappingConfiguration.Current.Mappings;

      UrlMappingEntry entry = mappings[0];
      Assert.AreSame (mappings[0], mappings.Find (entry.Resource), "Could not find {0}.", entry.Resource);

      entry = mappings[1];
      Assert.AreSame (mappings[1], mappings.Find (entry.Resource), "Could not find {0}.", entry.Resource);

      entry = mappings[2];
      Assert.AreSame (mappings[2], mappings.Find (entry.Resource), "Could not find {0}.", entry.Resource);

      Assert.IsNull (mappings.Find ("~/unmapped.wxe"), "Found mapping for unmapped resource.");
    }

    [Test]
    public void FindByID ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineUrlMapping ();
      UrlMappingCollection mappings = UrlMappingConfiguration.Current.Mappings;

      UrlMappingEntry entry = mappings[0];
      Assert.AreSame (mappings[0], mappings.FindByID (entry.ID), "Could not find {0}.", entry.ID);

      entry = mappings[1];
      Assert.AreSame (mappings[1], mappings.FindByID (entry.ID), "Could not find {0}.", entry.Resource);

      Assert.IsNull (mappings.FindByID ("unknown"), "Found mapping for unknown id.");
    }
  }

}
