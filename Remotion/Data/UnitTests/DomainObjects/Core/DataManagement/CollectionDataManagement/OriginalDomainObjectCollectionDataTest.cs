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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class OriginalDomainObjectCollectionDataTest
  {
    private Order _domainObject1;
    private Order _domainObject2;
    private Order _domainObject3;

    private IDomainObjectCollectionData _actualData;
    private OriginalDomainObjectCollectionData _originalData;

    [SetUp]
    public void SetUp ()
    {
      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      _actualData = new DomainObjectCollectionData (new[] { _domainObject1, _domainObject2 });
      _originalData = new OriginalDomainObjectCollectionData (_actualData);
    }

    [Test]
    public void Initialization_SetsUpDelegationToActualValues_ByDefault ()
    {
      Assert.That (_originalData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      
      _actualData.Add (_domainObject3);
      
      Assert.That (_originalData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
    }

    [Test]
    public void Initialization_SetsUpReadOnly ()
    {
      Assert.That (_originalData.IsReadOnly, Is.True);
    }

    [Test]
    public void CopyOnWrite_SetsUpCopy ()
    {
      _originalData.CopyOnWrite ();

      Assert.That (_originalData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));

      _actualData.Add (_domainObject3);

      Assert.That (_originalData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void CopyOnWrite_SetsUpCopy_ReadOnly ()
    {
      _originalData.CopyOnWrite ();
      Assert.That (_originalData.IsReadOnly);
    }

    [Test]
    public void CopyOnWrite_Twice ()
    {
      _originalData.CopyOnWrite ();
      var data1 = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (_originalData);

      _originalData.CopyOnWrite ();
      var data2 = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (_originalData);
      
      Assert.That (data1, Is.SameAs (data2));
    }

    [Test]
    public void RevertToActualData ()
    {
      _originalData.CopyOnWrite ();
      _actualData.Remove (_domainObject1);
      
      _originalData.RevertToActualData ();

      Assert.That (_originalData.ToArray (), Is.EqualTo (new[] { _domainObject2 }));

      _actualData.Add (_domainObject3);
      Assert.That (_originalData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
    }

    [Test]
    public void RevertToActualData_ReadOnly ()
    {
      _originalData.CopyOnWrite ();
      _originalData.RevertToActualData ();
      
      Assert.That (_originalData.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This collection is read-only and does not support accessing its underlying data store.")]
    public void GetDataStore ()
    {
      _originalData.GetDataStore ();
    }

    [Test]
    public void Serializable ()
    {
     Assert.That (_originalData.Count, Is.EqualTo (2));

      var deserialized = Serializer.SerializeAndDeserialize (_originalData);
      Assert.That (deserialized.Count, Is.EqualTo (2));
    }
  }
}