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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.InheritanceRootSample;
using Remotion.Mixins;
using @STI=Remotion.Data.UnitTests.DomainObjects.TestDomain;
using @CTI = Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class DifferentMappingScenariosIntegrationTest : IntegrationTestBase
  {
    private DomainObjectIDs _concreteObjectIDs;

    [SetUp]
    public override void SetUp ()
    {
// ReSharper disable RedundantNameQualifier
      _concreteObjectIDs = new TableInheritance.DomainObjectIDs ();
// ReSharper restore RedundantNameQualifier
      base.SetUp();
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass_SingleTableInheritance ()
    {
      var customer = (from c in QueryFactory.CreateLinqQuery<STI.Customer>()
                   where c.Name == "Kunde 3"
                   select c);

      CheckQueryResult (customer, DomainObjectIDs.Customer3);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass_SingleTableInheritance ()
    {
      var customer = (from c in QueryFactory.CreateLinqQuery<STI.Customer> ()
                      where c.Type == UnitTests.DomainObjects.TestDomain.Customer.CustomerType.Standard
                      select c);

      CheckQueryResult (customer, DomainObjectIDs.Customer1);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInSameClass_SingleTableInheritance ()
    {
      var orders = (from c in QueryFactory.CreateLinqQuery<STI.Customer> ()
                      from o in c.Orders
                      where c.Name=="Kunde 3"
                      select o);
      
      CheckQueryResult (orders, DomainObjectIDs.Order2);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInBaseClass_SingleTableInheritance ()
    {
      var customers = (from c in QueryFactory.CreateLinqQuery<STI.Customer> ()
                    where c.IndustrialSector.ID == DomainObjectIDs.IndustrialSector2
                    select c);

      CheckQueryResult (customers, DomainObjectIDs.Customer3, DomainObjectIDs.Customer2);
    }

    [Test]
    public void BaseObjects_PropertyAccessInSameClass_SingleTableInheritance ()
    {
      var company = (from c in QueryFactory.CreateLinqQuery<STI.Company> ()
                     where c.Name == "Firma 2"
                     select c);

      CheckQueryResult (company, DomainObjectIDs.Company2);
    }

    [Test]
    public void BaseObjects_MemberAccessInSameClass_SingleTableInheritance ()
    {
      var company = (from c in QueryFactory.CreateLinqQuery<STI.Company> ()
                     where c.IndustrialSector.ID == DomainObjectIDs.IndustrialSector2 && c.Name=="Firma 2"
                     select c);

      CheckQueryResult (company, DomainObjectIDs.Company2);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<CTI.File> ()
                     where f.Name == "Datei im Root"
                     select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<CTI.File> ()
                 where f.Size == 512
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.File1);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInBaseClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<CTI.File> ()
                 where f.ParentFolder.ID == _concreteObjectIDs.FolderRoot
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void BaseObjects_PropertyAccessInSameClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<CTI.FileSystemItem> ()
                 where f.Name == "Datei im Root"
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void BaseObjects_MemberAccessInSameClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<CTI.FileSystemItem> ()
                 where f.ParentFolder.ID == _concreteObjectIDs.FolderRoot
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot, _concreteObjectIDs.Folder1);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass_ClassAboveInheritanceHierarchy ()
    {
      var storageClass = (from f in QueryFactory.CreateLinqQuery<StorageGroupClass> ()
                 where f.StorageGroupClassIdentifier == "StorageGroupName1"
                 select f);

      CheckQueryResult (storageClass, DomainObjectIDs.StorageGroupClass1);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass_ClassAboveInheritanceHierarchy ()
    {
      var storageClass = (from f in QueryFactory.CreateLinqQuery<StorageGroupClass> ()
                          where f.AboveInheritanceIdentifier == "AboveInheritanceName1"
                          select f);

      CheckQueryResult (storageClass, DomainObjectIDs.StorageGroupClass1);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToSameObject ()
    {
      var mixins = (from t in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
                          where ((IMixinAddingPeristentProperties) t).PersistentProperty == 99
                          select t);

      CheckQueryResult (mixins, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToBaseObject ()
    {
      var mixins = (from m in QueryFactory.CreateLinqQuery<DerivedTargetClassForPersistentMixin> ()
                    where ((IMixinAddingPeristentProperties) m).PersistentProperty == 199
                    select m);

      CheckQueryResult (mixins, DomainObjectIDs.TargetClassForPersistentMixins2);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToBaseBaseObject ()
    {
      var mixins = (from m in QueryFactory.CreateLinqQuery<DerivedDerivedTargetClassForPersistentMixin> ()
                    where ((IMixinAddingPeristentProperties) m).PersistentProperty == 299
                    select m);

      CheckQueryResult (mixins, DomainObjectIDs.TargetClassForPersistentMixins3);
    }

  }
}