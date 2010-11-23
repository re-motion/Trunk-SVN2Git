// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Configuration
{
  [TestFixture]
  public class StorageProviderDefinitionFinderTest
  {
    private ReflectionBasedClassDefinition _classWithoutStorageGroupType;
    private ReflectionBasedClassDefinition _classWithStorageGroupType;
    private StorageProviderDefinitionFinder _finder;
    private IDomainObjectsConfiguration _currentConfiguration;

    [SetUp]
    public void SetUp ()
    {
      _finder = new StorageProviderDefinitionFinder();
      _classWithoutStorageGroupType = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Test", "Test", "DefaultStorageProviderID", typeof (Customer), false, null, null, new PersistentMixinFinder (typeof (Customer)));
      _classWithStorageGroupType = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Test",
          "Test",
          "DefaultStorageProviderID",
          typeof (Customer),
          false,
          null,
          typeof (StubStorageGroup1Attribute),
          new PersistentMixinFinder (typeof (Customer)));

      _currentConfiguration = DomainObjectsConfiguration.Current;
    }

    [TearDown]
    public void TearDown ()
    {
      DomainObjectsConfiguration.SetCurrent (_currentConfiguration);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Missing default storage provider.", MatchType = MessageMatch.Contains)]
    public void GetStorageProviderDefinition_ClassWithoutStorageGroupType_NoDefaultStorageProviderDefinitionDefined ()
    {
      DomainObjectsConfiguration.SetCurrent (null);

      _finder.GetStorageProviderDefinition (_classWithoutStorageGroupType);
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithoutStorageGroupType_DefaultStorageProviderDefinitionDefined ()
    {
      var result = _finder.GetStorageProviderDefinition (_classWithoutStorageGroupType);

      Assert.That (result.Name, Is.EqualTo ("DefaultStorageProvider"));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Missing default storage provider.", MatchType = MessageMatch.Contains)]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupNotDefined_NoDefaultStorageProviderDefinitionDefined ()
    {
      DomainObjectsConfiguration.SetCurrent (null);

      _finder.GetStorageProviderDefinition (_classWithStorageGroupType);
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupNotDefined_DefaultStorageProviderDefinitionDefined ()
    {
      var result = _finder.GetStorageProviderDefinition (_classWithStorageGroupType);

      Assert.That (result.Name, Is.EqualTo ("DefaultStorageProvider"));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupDefined ()
    {
      var spID = "Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Configuration.StubStorageGroup1Attribute, Remotion.Data.UnitTests";
      var storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (spID, typeof (UnitTestStorageProviderStub), typeof (UnitTestStorageObjectFactoryStub)));
      var storageConfiguration = new StorageConfiguration (
          storageProviderDefinitionCollection, DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition);
      storageConfiguration.StorageGroups.Add (new StorageGroupElement (new StubStorageGroup1Attribute(), spID));
      DomainObjectsConfiguration.SetCurrent (
          new FakeDomainObjectsConfiguration (
              new MappingLoaderConfiguration(), storageConfiguration, new QueryConfiguration ("DomainObjects\\QueriesForStandardMapping.xml")));
      DomainObjectsConfiguration.Current.Storage.StorageGroups.Add (
          new StorageGroupElement (
              new StubStorageGroup1Attribute(),
              "Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Configuration.StubStorageGroup1Attribute, Remotion.Data.UnitTests"));
      
      var result = _finder.GetStorageProviderDefinition (_classWithStorageGroupType);

      Assert.That (result.Name, Is.EqualTo (spID));
    }
  }
}