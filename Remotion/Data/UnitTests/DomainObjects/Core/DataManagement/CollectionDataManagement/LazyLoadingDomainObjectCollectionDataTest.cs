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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Collections;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class LazyLoadingDomainObjectCollectionDataTest : StandardMappingTest
  {
    private ClientTransaction _clientTransactionMock;
    private RelationEndPointID _endPointID;
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;
    
    private LazyLoadingDomainObjectCollectionData _loadedData;
    private LazyLoadingDomainObjectCollectionData _unloadedData;


    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransactionMock = ClientTransactionObjectMother.CreateStrictMock ();
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();


      _loadedData = new LazyLoadingDomainObjectCollectionData (_clientTransactionMock, _endPointID, new[] { _domainObject1 });
      _unloadedData = new LazyLoadingDomainObjectCollectionData (_clientTransactionMock, _endPointID, null);
    }
    
    [Test]
    public void Initialization_Null ()
    {
      var data = new LazyLoadingDomainObjectCollectionData (_clientTransactionMock, _endPointID, null);
      Assert.That (data.IsDataAvailable, Is.False);
    }

    [Test]
    public void Initialization_NotNull ()
    {
      var data = new LazyLoadingDomainObjectCollectionData (_clientTransactionMock, _endPointID, new[] { _domainObject1, _domainObject2 });
      Assert.That (data.IsDataAvailable, Is.True);
    }

    [Test]
    public void ActualData_Loaded ()
    {
      _clientTransactionMock.Replay ();

      Assert.That (_loadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
      _clientTransactionMock.AssertWasNotCalled (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID));
    }

    [Test]
    public void ActualData_Unloaded_LoadsData ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });

      _clientTransactionMock.Replay ();

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      Assert.That (_unloadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);

      _clientTransactionMock.VerifyAllExpectations();
    }

    [Test]
    public void Unload ()
    {
      Assert.That (_loadedData.IsDataAvailable, Is.True);

      _loadedData.Unload ();

      Assert.That (_loadedData.IsDataAvailable, Is.False);
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      Assert.That (((IDomainObjectCollectionData) _loadedData).AssociatedEndPoint, Is.Null);
      Assert.That (((IDomainObjectCollectionData) _unloadedData).AssociatedEndPoint, Is.Null);
      Assert.That (_unloadedData.IsDataAvailable, Is.False);
    }

    [Test]
    public void Count_Loaded ()
    {
      Assert.That (_loadedData.Count, Is.EqualTo (1));
    }

    [Test]
    public void Count_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      Assert.That (_unloadedData.Count, Is.EqualTo (2));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That (((IDomainObjectCollectionData) _loadedData).RequiredItemType, Is.Null);
      Assert.That (((IDomainObjectCollectionData) _unloadedData).RequiredItemType, Is.Null);
      Assert.That (_unloadedData.IsDataAvailable, Is.False);
    }

    [Test]
    public void GetUndecoratedDataStore ()
    {
      Assert.That (((IDomainObjectCollectionData) _loadedData).GetUndecoratedDataStore(), Is.SameAs (_loadedData));
      Assert.That (((IDomainObjectCollectionData) _unloadedData).GetUndecoratedDataStore(), Is.SameAs (_unloadedData));
      Assert.That (_unloadedData.IsDataAvailable, Is.False);
    }

    [Test]
    public void GetEnumerator_Loaded ()
    {
      using (var enumerator = _loadedData.GetEnumerator ())
      {
        Assert.That (enumerator.MoveNext (), Is.True);
        Assert.That (enumerator.Current, Is.SameAs (_domainObject1));
      }
    }

    [Test]
    public void GetEnumerator_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      using (var enumerator = _unloadedData.GetEnumerator ())
      {
        Assert.That (_unloadedData.IsDataAvailable, Is.True);

        Assert.That (enumerator.MoveNext (), Is.True);
        Assert.That (enumerator.Current, Is.SameAs (_domainObject2));
      }
    }

    [Test]
    public void GetEnumerator_NonGeneric_Loaded ()
    {
      var enumerator = ((IEnumerable) _loadedData).GetEnumerator ();

      Assert.That (enumerator.MoveNext (), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetEnumerator_NonGeneric_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);

      var enumerator = ((IEnumerable) _unloadedData).GetEnumerator ();
      
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
      Assert.That (enumerator.MoveNext (), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (_domainObject2));
    }

    [Test]
    public void ContainsObjectID_Loaded ()
    {
      Assert.That (_loadedData.ContainsObjectID (_domainObject1.ID), Is.True);
      Assert.That (_loadedData.ContainsObjectID (_domainObject2.ID), Is.False);
    }

    [Test]
    public void ContainsObjectID_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      Assert.That (_unloadedData.ContainsObjectID (_domainObject2.ID), Is.True);
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
      Assert.That (_unloadedData.ContainsObjectID (_domainObject1.ID), Is.False);
    }

    [Test]
    public void GetObject_Int_Loaded ()
    {
      Assert.That (_loadedData.GetObject (0), Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObject_Int_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      Assert.That (_unloadedData.GetObject (0), Is.SameAs (_domainObject2));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void GetObject_ID_Loaded ()
    {
      Assert.That (_loadedData.GetObject (_domainObject1.ID), Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObject_ID_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      Assert.That (_unloadedData.GetObject (_domainObject2.ID), Is.SameAs (_domainObject2));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void IndexOf_Loaded ()
    {
      Assert.That (_loadedData.IndexOf (_domainObject1.ID), Is.EqualTo (0));
    }

    [Test]
    public void IndexOf_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      Assert.That (_unloadedData.IndexOf (_domainObject3.ID), Is.EqualTo (1));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void Clear_Loaded ()
    {
      _loadedData.Clear ();
      Assert.That (_loadedData.ActualData.ToArray(), Is.Empty);
    }

    [Test]
    public void Clear_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      _unloadedData.Clear ();
      Assert.That (_unloadedData.ActualData.ToArray (), Is.Empty);
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void Insert_Loaded ()
    {
      _loadedData.Insert (0, _domainObject2);
      Assert.That (_loadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject1 }));
    }

    [Test]
    public void Insert_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      _unloadedData.Insert (0, _domainObject1);
      Assert.That (_unloadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void Remove_Loaded ()
    {
      _loadedData.Remove (_domainObject1);
      Assert.That (_loadedData.ActualData.ToArray (), Is.Empty);
    }

    [Test]
    public void Remove_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      _unloadedData.Remove (_domainObject2);
      Assert.That (_unloadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject3 }));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void Remove_ID_Loaded ()
    {
      _loadedData.Remove (_domainObject1.ID);
      Assert.That (_loadedData.ActualData.ToArray (), Is.Empty);
    }

    [Test]
    public void Remove_ID_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      _unloadedData.Remove (_domainObject2.ID);
      Assert.That (_unloadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject3 }));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }

    [Test]
    public void Replace_Loaded ()
    {
      _loadedData.Replace (0, _domainObject2);
      Assert.That (_loadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject2 }));
    }

    [Test]
    public void Replacee_Unloaded ()
    {
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      _unloadedData.Replace (0, _domainObject1);
      Assert.That (_unloadedData.ActualData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject3 }));
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
    }
    
    private void StubLoadRelatedObjects (params DomainObject[] relatedObjects)
    {
      _clientTransactionMock
          .Stub (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (relatedObjects);

      _clientTransactionMock.Replay ();
    }
  }
}