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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class CopyOnWriteDomainObjectCollectionDataTest : StandardMappingTest
  {
    private Order _domainObject1;
    private Order _domainObject2;
    private Order _domainObject3;

    private ObservableCollectionDataDecorator _copiedData;
    private CopyOnWriteDomainObjectCollectionData _copyOnWriteData;
    private DomainObjectCollectionData _underlyingCopiedData;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      _underlyingCopiedData = new DomainObjectCollectionData (new[] { _domainObject1, _domainObject2 });
      _copiedData = new ObservableCollectionDataDecorator (_underlyingCopiedData);
      _copyOnWriteData = new CopyOnWriteDomainObjectCollectionData (_copiedData);
    }

    [Test]
    public void Initialization_SetsUpDelegationToCopiedValues_ByDefault ()
    {
      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));

      _underlyingCopiedData.Add (_domainObject3);
      
      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
    }

    [Test]
    public void Initialization_SetsUpCopyOnWrite_ForCopiedCollection ()
    {
      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));

      _copiedData.Add (_domainObject3);

      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void CopyOnWrite_SetsUpCopy ()
    {
      _copyOnWriteData.CopyOnWrite ();

      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));

      _underlyingCopiedData.Add (_domainObject3);

      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void CopyOnWrite_Twice ()
    {
      _copyOnWriteData.CopyOnWrite ();
      var data1 = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (_copyOnWriteData);

      _copyOnWriteData.CopyOnWrite ();
      var data2 = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (_copyOnWriteData);
      
      Assert.That (data1, Is.SameAs (data2));
    }

    [Test]
    public void RevertToCopiedData ()
    {
      _copyOnWriteData.CopyOnWrite ();
      _underlyingCopiedData.Remove (_domainObject1);
      
      _copyOnWriteData.RevertToCopiedData ();

      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject2 }));

      _underlyingCopiedData.Add (_domainObject3);
      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
    }

    [Test]
    public void OnDataChange_PerformsCopyOperation ()
    {
      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));

      _copyOnWriteData.Add (_domainObject3);

      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (_copiedData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetDataStore_PerformsCopyOperation ()
    {
      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));

      var underlyingData = _copyOnWriteData.GetDataStore ();
      underlyingData.Add (_domainObject3);

      Assert.That (_copyOnWriteData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (_copiedData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void Serializable ()
    {
     Assert.That (_copyOnWriteData.Count, Is.EqualTo (2));

      var deserialized = Serializer.SerializeAndDeserialize (_copyOnWriteData);
      Assert.That (deserialized.Count, Is.EqualTo (2));
    }
  }
}