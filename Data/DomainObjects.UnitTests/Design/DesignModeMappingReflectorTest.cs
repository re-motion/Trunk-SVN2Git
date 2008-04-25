using System;
using System.ComponentModel.Design;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Design
{
  [TestFixture]
  public class DesignModeMappingReflectorTest
  {
    [TearDown]
    public void TearDown ()
    {
      // reset mixin configuration to defaults
      MixinConfiguration.SetActiveConfiguration (null);
      MixinConfiguration.ResetMasterConfiguration();
    }

    [Test]
    public void Initialize ()
    {
      MockRepository mockRepository = new MockRepository();
      IDesignerHost mockDesignerHost = mockRepository.CreateMock<IDesignerHost>();
      ITypeDiscoveryService stubTypeDiscoveryService = mockRepository.CreateMock<ITypeDiscoveryService>();
      Expect.Call (mockDesignerHost.GetService (typeof (ITypeDiscoveryService))).Return (stubTypeDiscoveryService);

      mockRepository.ReplayAll();

      new DesignModeMappingReflector (mockDesignerHost);

      mockRepository.VerifyAll();
    }

    [Test]
    public void GetClassDefinitions ()
    {
      MockRepository mockRepository = new MockRepository();
      IDesignerHost stubDesignerHost = mockRepository.CreateMock<IDesignerHost>();
      ITypeDiscoveryService mockTypeDiscoveryService = mockRepository.CreateMock<ITypeDiscoveryService>();
      SetupResult.For (stubDesignerHost.GetService (typeof (ITypeDiscoveryService))).Return (mockTypeDiscoveryService);
      Expect.Call (mockTypeDiscoveryService.GetTypes (typeof (DomainObject), false)).Return (new Type[] {typeof (Company)});
      mockRepository.ReplayAll();

      DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (stubDesignerHost);
      ClassDefinitionCollection classDefinitionCollection = mappingReflector.GetClassDefinitions();

      mockRepository.VerifyAll();

      Assert.That (classDefinitionCollection.Count, Is.EqualTo (1));
      Assert.That (classDefinitionCollection.Contains (typeof (Company)));
    }

    [Test]
    public void DesignModeMappingReflector_SetsEmptyMixinConfigurationIfNoneExists ()
    {
      IDesignerHost mockDesignerHost = GetMockDesignerHost();

      MixinConfiguration.SetActiveConfiguration (null);
      DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (mockDesignerHost);
      mappingReflector.GetClassDefinitions();

      Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
      Assert.AreEqual (0, MixinConfiguration.ActiveConfiguration.ClassContexts.Count);
    }

    [Test]
    public void DesignModeMappingReflector_KeepsExistingMixinConfiguration ()
    {
      IDesignerHost mockDesignerHost = GetMockDesignerHost();

      MixinConfiguration context = new MixinConfiguration();
      MixinConfiguration.SetActiveConfiguration (context);

      DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (mockDesignerHost);
      mappingReflector.GetClassDefinitions();

      Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
      Assert.AreSame (context, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void DesignModeMappingReflectorWorksFine_WithDelaySignAssemblyInAppBase ()
    {
      AppDomainRunner.Run (
          delegate
          {
            try
            {
              Compile (@"Design\DelaySignAssembly", @"Design.Dlls\Remotion.Data.DomainObjects.UnitTests.Design.DelaySignAssembly.dll");
              Assert.Fail ("Expected FileLoadException");
            }
            catch (FileLoadException)
            {
              // expected
            }
            catch (AssemblyCompilationException)
            {
              // file gets locked on multiple executions
              Assert.IsTrue (System.IO.File.Exists (@"Design.Dlls\Remotion.Data.DomainObjects.UnitTests.Design.DelaySignAssembly.dll"));
            }
          });

      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = @"Design.Dlls";
      setup.DynamicBase = Path.GetTempPath();
      new AppDomainRunner (
          setup,
          delegate
          {
            ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create();

            PersistenceConfiguration persistenceConfiguration =
                new PersistenceConfiguration (
                    storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);
            persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new TestDomainAttribute(), DatabaseTest.c_testDomainProviderID));

            MappingLoaderConfiguration mappingLoaderConfiguration = new MappingLoaderConfiguration();
            QueryConfiguration queryConfiguration = new QueryConfiguration();
            DomainObjectsConfiguration.SetCurrent (
                new FakeDomainObjectsConfiguration (mappingLoaderConfiguration, persistenceConfiguration, queryConfiguration));

            Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);

            IDesignerHost mockDesignerHost = GetMockDesignerHost();
            DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (mockDesignerHost);
            mappingReflector.GetClassDefinitions();
          },
          new object[0]).Run();
    }

    private static void Compile (string sourceDirectory, string outputAssembly)
    {
      string outputAssemblyDirectory = Path.GetDirectoryName (Path.GetFullPath (outputAssembly));
      if (!Directory.Exists (outputAssemblyDirectory))
        Directory.CreateDirectory (outputAssemblyDirectory);

      AssemblyCompiler compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssembly,
          new string[] {"Remotion.dll", "Remotion.Data.Interfaces.dll", "Remotion.Data.DomainObjects.dll"});

      compiler.Compile();
    }

    private static IDesignerHost GetMockDesignerHost ()
    {
      MockRepository mockRepository = new MockRepository();
      IDesignerHost mockDesignerHost = mockRepository.CreateMock<IDesignerHost>();

      ITypeDiscoveryService stubTypeDiscoveryService = mockRepository.CreateMock<ITypeDiscoveryService>();
      Expect.Call (mockDesignerHost.GetService (typeof (ITypeDiscoveryService))).Return (stubTypeDiscoveryService);
      Expect.Call (stubTypeDiscoveryService.GetTypes (typeof (DomainObject), false)).Return (new Type[] {typeof (Order)});

      mockRepository.ReplayAll();
      return mockDesignerHost;
    }
  }
}