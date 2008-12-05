// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
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
          Provider, _domainBaseClass, _domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client"), DomainObjectIDs.Client);
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
          Provider, _domainBaseClass, _domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client"), 
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
          abstractClassWithoutDerivationsClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.AbstractClassWithoutDerivations.DomainBase"), 
          DomainObjectIDs.Person);

      DataContainerCollection loadedDataContainers = loader.LoadDataContainers ();
      Assert.IsNotNull (loadedDataContainers);
      Assert.IsEmpty (loadedDataContainers);
    }

  }
}
