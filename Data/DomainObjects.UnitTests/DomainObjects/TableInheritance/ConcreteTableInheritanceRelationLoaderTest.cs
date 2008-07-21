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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance
{
  [TestFixture]
  public class ConcreteTableInheritanceRelationLoaderTest : SqlProviderBaseTest
  {
    private ClassDefinition _domainBaseClass;
    private ConcreteTableInheritanceRelationLoader _loader;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      _loader = new ConcreteTableInheritanceRelationLoader (
          Provider, _domainBaseClass, _domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain.DomainBase.Client"), DomainObjectIDs.Client);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (Provider, _loader.Provider);
    }

    [Test]
    public void LoadDataContainers ()
    {
      DataContainerCollection dataContainers = _loader.LoadDataContainers ();

      Assert.IsNotNull (dataContainers);
      Assert.AreEqual (4, dataContainers.Count);
      Assert.IsTrue (dataContainers.Contains (DomainObjectIDs.Customer));
      Assert.IsTrue (dataContainers.Contains (DomainObjectIDs.Person));
      Assert.IsTrue (dataContainers.Contains (DomainObjectIDs.OrganizationalUnit));
      Assert.IsTrue (dataContainers.Contains (DomainObjectIDs.PersonForUnidirectionalRelationTest));
    }

    [Test]
    public void LoadOrderedDataContainers ()
    {
      DataContainerCollection dataContainers = _loader.LoadDataContainers ();

      Assert.IsNotNull (dataContainers);
      Assert.AreEqual (4, dataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, dataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, dataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, dataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, dataContainers[3].ID);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "Invalid ClassID 'InvalidClassID' for ID '1b5ba13a-f6ad-4390-87bb-d85a1c098d1c' encountered.")]
    public void LoadDataContainerWithInvalidClassID ()
    {
      ConcreteTableInheritanceRelationLoader loader = new ConcreteTableInheritanceRelationLoader (
          Provider, _domainBaseClass, _domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain.DomainBase.Client"), 
          new ObjectID (typeof (Client), new Guid ("{58535280-84EC-41d9-9F8F-BCAC64BB3709}")));

      loader.LoadDataContainers ();
    }

    [Test]
    public void LoadDataContainersWithNoConcreteEntity ()
    {
      ClassDefinition abstractClassWithoutDerivationsClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (
          typeof (AbstractClassWithoutDerivations));

      ConcreteTableInheritanceRelationLoader loader = new ConcreteTableInheritanceRelationLoader (
          Provider, abstractClassWithoutDerivationsClass,
          abstractClassWithoutDerivationsClass.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain.AbstractClassWithoutDerivations.DomainBase"), 
          DomainObjectIDs.Person);

      DataContainerCollection loadedDataContainers = loader.LoadDataContainers ();
      Assert.IsNotNull (loadedDataContainers);
      Assert.IsEmpty (loadedDataContainers);
    }

  }
}
