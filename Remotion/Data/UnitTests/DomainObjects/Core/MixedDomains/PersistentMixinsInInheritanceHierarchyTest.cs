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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.ConcreteInheritance;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.SingleInheritance;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains
{
  [TestFixture]
  public class PersistentMixinsInInheritanceHierarchyTest : StandardMappingTest
  {

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      SetDatabaseModifyable();
    }

    //[TearDown]
    //public override void TearDown ()
    //{
    //  base.TearDown();
    //}

    [Test]
    public void SingleInheritance_QueryOverView ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var firstDerivedClass = SingleInheritanceFirstDerivedClass.NewObject ();
        var secondDerivedClass = SingleInheritanceSecondDerivedClass.NewObject ();

        firstDerivedClass.BaseProperty = "BasePropertyValue 1";
        firstDerivedClass.FirstDerivedProperty = "FirstDerivedPropertyValue 1";
        ((ISingleInheritancePersistentMixin) firstDerivedClass).PersistentProperty = "PersistentPropertyValue 1";

        secondDerivedClass.BaseProperty = "BasePropertyValue 2";
        secondDerivedClass.SecondDerivedProperty = "SecondDerivedPropertyValue 2";
        ((ISingleInheritancePersistentMixin) secondDerivedClass).PersistentProperty = "PersistentPropertyValue 2";

        ClientTransaction.Current.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var query = new Query (new QueryDefinition ("QueryOverUnionView", TestDomainStorageProviderDefinition,
                                                    "SELECT * FROM [SingleInheritanceBaseClassView]", QueryType.Collection), new QueryParameterCollection ());
        var actualObjects = ClientTransaction.Current.QueryManager.GetCollection<SingleInheritanceBaseClass> (query);

        Assert.AreEqual (2, actualObjects.Count);
        var actualFirstDerivedClass = actualObjects.AsEnumerable ().OfType<SingleInheritanceFirstDerivedClass> ().Single ();
        var actualSecondDerivedClass = actualObjects.AsEnumerable ().OfType<SingleInheritanceSecondDerivedClass> ().Single ();

        Assert.AreEqual ("BasePropertyValue 1", actualFirstDerivedClass.BaseProperty);
        Assert.AreEqual ("FirstDerivedPropertyValue 1", actualFirstDerivedClass.FirstDerivedProperty);
        Assert.AreEqual ("PersistentPropertyValue 1", ((ISingleInheritancePersistentMixin) actualFirstDerivedClass).PersistentProperty);

        Assert.AreEqual ("BasePropertyValue 2", actualSecondDerivedClass.BaseProperty);
        Assert.AreEqual ("SecondDerivedPropertyValue 2", actualSecondDerivedClass.SecondDerivedProperty);
        Assert.AreEqual ("PersistentPropertyValue 2", ((ISingleInheritancePersistentMixin) actualSecondDerivedClass).PersistentProperty);
      }
    }

    [Test]
    public void SingleInheritance_GetObject ()
    {
      ObjectID firstDerivedClassObjectID;
      ObjectID secondDerivedClassObjectID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var firstDerivedClass = SingleInheritanceFirstDerivedClass.NewObject();
        firstDerivedClassObjectID = firstDerivedClass.ID;
        var secondDerivedClass = SingleInheritanceSecondDerivedClass.NewObject();
        secondDerivedClassObjectID = secondDerivedClass.ID;

        ClientTransaction.Current.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Assert.IsInstanceOf (typeof (SingleInheritanceFirstDerivedClass), LifetimeService.GetObject (ClientTransaction.Current, firstDerivedClassObjectID, false));
        Assert.IsInstanceOf (typeof (SingleInheritanceSecondDerivedClass), LifetimeService.GetObject (ClientTransaction.Current, secondDerivedClassObjectID, false));
      }
    }

    [Test]
    public void SingleInheritance_RelationsWorkCorrectly ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var objectWithRelations = SingleInheritanceObjectWithRelations.NewObject();
        objectWithRelations.ScalarProperty = SingleInheritanceFirstDerivedClass.NewObject ();
        objectWithRelations.VectorProperty.Add (SingleInheritanceFirstDerivedClass.NewObject());
        objectWithRelations.VectorProperty.Add (SingleInheritanceSecondDerivedClass.NewObject ());

        ClientTransaction.Current.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var query = new Query (new QueryDefinition ("QueryOverUnionView", TestDomainStorageProviderDefinition,
                                               "SELECT * FROM [SingleInheritanceObjectWithRelationsView]", QueryType.Collection), new QueryParameterCollection ());
        var actualObjectWithRelations = ClientTransaction.Current.QueryManager.GetCollection<SingleInheritanceObjectWithRelations> (query)
          .AsEnumerable().Single();

        Assert.IsInstanceOf (typeof (SingleInheritanceFirstDerivedClass), actualObjectWithRelations.ScalarProperty);
        Assert.AreEqual (2, actualObjectWithRelations.VectorProperty.Count);
        Assert.IsNotNull (actualObjectWithRelations.VectorProperty.OfType<SingleInheritanceFirstDerivedClass> ().Single ());
        Assert.IsNotNull (actualObjectWithRelations.VectorProperty.OfType<SingleInheritanceSecondDerivedClass> ().Single ());
      }
    }

    [Test]
    public void ConcreteInheritance_QueryOverUnionView ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var firstDerivedClass = ConcreteInheritanceFirstDerivedClass.NewObject ();
        var secondDerivedClass = ConcreteInheritanceSecondDerivedClass.NewObject ();

        firstDerivedClass.BaseProperty = "BasePropertyValue 1";
        firstDerivedClass.FirstDerivedProperty = "FirstDerivedPropertyValue 1";
        ((IConcreteInheritancePersistentMixin) firstDerivedClass).PersistentProperty = "PersistentPropertyValue 1";

        secondDerivedClass.BaseProperty = "BasePropertyValue 2";
        secondDerivedClass.SecondDerivedProperty = "SecondDerivedPropertyValue 2";
        ((IConcreteInheritancePersistentMixin) secondDerivedClass).PersistentProperty = "PersistentPropertyValue 2";

        ClientTransaction.Current.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var query = new Query (new QueryDefinition ("QueryOverUnionView", TestDomainStorageProviderDefinition,
                                                    "SELECT * FROM [ConcreteInheritanceBaseClassView]", QueryType.Collection), new QueryParameterCollection ());
        var actualObjects = ClientTransaction.Current.QueryManager.GetCollection<ConcreteInheritanceBaseClass> (query);

        Assert.AreEqual (2, actualObjects.Count);
        var actualFirstDerivedClass = actualObjects.AsEnumerable ().OfType<ConcreteInheritanceFirstDerivedClass> ().Single ();
        var actualSecondDerivedClass = actualObjects.AsEnumerable ().OfType<ConcreteInheritanceSecondDerivedClass> ().Single ();

        Assert.AreEqual ("BasePropertyValue 1", actualFirstDerivedClass.BaseProperty);
        Assert.AreEqual ("FirstDerivedPropertyValue 1", actualFirstDerivedClass.FirstDerivedProperty);
        Assert.AreEqual ("PersistentPropertyValue 1", ((IConcreteInheritancePersistentMixin) actualFirstDerivedClass).PersistentProperty);

        Assert.AreEqual ("BasePropertyValue 2", actualSecondDerivedClass.BaseProperty);
        Assert.AreEqual ("SecondDerivedPropertyValue 2", actualSecondDerivedClass.SecondDerivedProperty);
        Assert.AreEqual ("PersistentPropertyValue 2", ((IConcreteInheritancePersistentMixin) actualSecondDerivedClass).PersistentProperty);
      }
    }

    [Test]
    public void ConcreteInheritance_GetObject ()
    {
      ObjectID firstDerivedClassObjectID;
      ObjectID secondDerivedClassObjectID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var firstDerivedClass = ConcreteInheritanceFirstDerivedClass.NewObject ();
        firstDerivedClassObjectID = firstDerivedClass.ID;
        var secondDerivedClass = ConcreteInheritanceSecondDerivedClass.NewObject ();
        secondDerivedClassObjectID = secondDerivedClass.ID;

        ClientTransaction.Current.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Assert.IsInstanceOf (typeof (ConcreteInheritanceFirstDerivedClass), LifetimeService.GetObject (ClientTransaction.Current, firstDerivedClassObjectID, false));
        Assert.IsInstanceOf (typeof (ConcreteInheritanceSecondDerivedClass), LifetimeService.GetObject (ClientTransaction.Current, secondDerivedClassObjectID, false));
      }
    }

    [Test]
    public void ConcreteInheritance_RelationsWorkCorrectly ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var objectWithRelations = ConcreteInheritanceObjectWithRelations.NewObject ();
        objectWithRelations.ScalarProperty = ConcreteInheritanceFirstDerivedClass.NewObject ();
        objectWithRelations.VectorProperty.Add (ConcreteInheritanceFirstDerivedClass.NewObject ());
        objectWithRelations.VectorProperty.Add (ConcreteInheritanceSecondDerivedClass.NewObject ());

        ClientTransaction.Current.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var query = new Query (new QueryDefinition ("QueryOverUnionView", TestDomainStorageProviderDefinition,
                                               "SELECT * FROM [ConcreteInheritanceObjectWithRelationsView]", QueryType.Collection), new QueryParameterCollection ());
        var actualObjectWithRelations = ClientTransaction.Current.QueryManager.GetCollection<ConcreteInheritanceObjectWithRelations> (query)
          .AsEnumerable ().Single ();

        Assert.IsInstanceOf (typeof (ConcreteInheritanceFirstDerivedClass), actualObjectWithRelations.ScalarProperty);
        Assert.AreEqual (2, actualObjectWithRelations.VectorProperty.Count);
        Assert.IsNotNull (actualObjectWithRelations.VectorProperty.OfType<ConcreteInheritanceFirstDerivedClass> ().Single ());
        Assert.IsNotNull (actualObjectWithRelations.VectorProperty.OfType<ConcreteInheritanceSecondDerivedClass> ().Single ());
      }
    }

  }
}