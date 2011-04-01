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
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class CollectionEndPointOppositeTrackingTest : ClientTransactionBaseTest
  {
    private Folder _folder1;
    private CollectionEndPoint _collectionEndPoint;

    private FileSystemItem _fileSystemItem1;
    private FileSystemItem _fileSystemItem2;
    private FileSystemItem _fileSystemItem3;

    private RealObjectEndPoint _fileSystemItem1EndPoint;
    private RealObjectEndPoint _fileSystemItem2EndPoint;
    private RealObjectEndPoint _fileSystemItem3EndPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _folder1 = Folder.NewObject ();
      
      _fileSystemItem1 = FileSystemItem.NewObject ();
      _fileSystemItem2 = FileSystemItem.NewObject ();
      _fileSystemItem3 = FileSystemItem.NewObject ();

      _folder1.FileSystemItems.Add (_fileSystemItem1);
      _folder1.FileSystemItems.Add (_fileSystemItem2);
      
      ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope();

      _folder1.FileSystemItems.EnsureDataComplete();
      _collectionEndPoint = GetEndPoint<CollectionEndPoint> (RelationEndPointID.Create (_folder1, o => o.FileSystemItems));

      _fileSystemItem1EndPoint = GetEndPoint<RealObjectEndPoint> (RelationEndPointID.Create (_fileSystemItem1, oi => oi.ParentFolder));
      _fileSystemItem2EndPoint = GetEndPoint<RealObjectEndPoint> (RelationEndPointID.Create (_fileSystemItem2, oi => oi.ParentFolder));
      _fileSystemItem3EndPoint = GetEndPoint<RealObjectEndPoint> (RelationEndPointID.Create (_fileSystemItem3, oi => oi.ParentFolder));
    }

    [Test]
    public void StateAfterLoading ()
    {
      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);
    }

    [Test]
    public void Insert ()
    {
      _folder1.FileSystemItems.Insert (1, _fileSystemItem3);

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2, _fileSystemItem3);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint, _fileSystemItem3EndPoint);

      ClientTransaction.Current.Rollback();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      _folder1.FileSystemItems.Insert (1, _fileSystemItem3);
      ClientTransaction.Current.Commit();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2, _fileSystemItem3);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint, _fileSystemItem3EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2, _fileSystemItem3);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint, _fileSystemItem3EndPoint);
    }

    [Test]
    public void Insert_ViaFKProperty ()
    {
      _fileSystemItem3.ParentFolder = _folder1;

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2, _fileSystemItem3);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint, _fileSystemItem3EndPoint);
    }

    [Test]
    public void Remove ()
    {
      _folder1.FileSystemItems.Remove (_fileSystemItem1);

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem2EndPoint);

      ClientTransaction.Current.Rollback();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      _folder1.FileSystemItems.Remove (_fileSystemItem1);

      ClientTransaction.Current.Commit();

      CheckOriginalData (_fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem2EndPoint);
    }

    [Test]
    public void Remove_ViaFKProperty ()
    {
      _fileSystemItem1.ParentFolder = null;

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem2EndPoint);
    }

    [Test]
    public void Replace ()
    {
      var index = _folder1.FileSystemItems.IndexOf (_fileSystemItem1);
      _folder1.FileSystemItems[index] = _fileSystemItem3;

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem3, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem3EndPoint, _fileSystemItem2EndPoint);

      ClientTransaction.Current.Rollback ();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      _folder1.FileSystemItems[index] = _fileSystemItem3;
      ClientTransaction.Current.Commit();

      CheckOriginalData (_fileSystemItem3, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem3EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem3, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem3EndPoint, _fileSystemItem2EndPoint);
    }

    [Test]
    public void Clear ()
    {
      _folder1.FileSystemItems.Clear();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData ();
      CheckCurrentOppositeEndPoints ();

      ClientTransaction.Current.Rollback ();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      _folder1.FileSystemItems.Clear ();
      ClientTransaction.Current.Commit();

      CheckOriginalData ();
      CheckOriginalOppositeEndPoints ();

      CheckCurrentData ();
      CheckCurrentOppositeEndPoints ();
    }

    [Test]
    public void ReplaceWholeCollection ()
    {
      _folder1.FileSystemItems = new ObjectList<FileSystemItem> { _fileSystemItem3 };

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem3);
      CheckCurrentOppositeEndPoints (_fileSystemItem3EndPoint);

      ClientTransaction.Current.Rollback ();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      _folder1.FileSystemItems = new ObjectList<FileSystemItem> { _fileSystemItem3 };
      ClientTransaction.Current.Commit ();

      CheckOriginalData (_fileSystemItem3);
      CheckOriginalOppositeEndPoints (_fileSystemItem3EndPoint);

      CheckCurrentData (_fileSystemItem3);
      CheckCurrentOppositeEndPoints (_fileSystemItem3EndPoint);
    }

    [Test]
    public void Delete_FKSide ()
    {
      _fileSystemItem1.Delete();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem2EndPoint);

      ClientTransaction.Current.Rollback ();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      _fileSystemItem1.Delete();

      ClientTransaction.Current.Commit ();

      CheckOriginalData (_fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem2EndPoint);
    }

    [Test]
    public void Delete_CollectionSide ()
    {
      _folder1.Delete ();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData ();
      CheckCurrentOppositeEndPoints ();

      ClientTransaction.Current.Rollback ();

      CheckOriginalData (_fileSystemItem1, _fileSystemItem2);
      CheckOriginalOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      CheckCurrentData (_fileSystemItem1, _fileSystemItem2);
      CheckCurrentOppositeEndPoints (_fileSystemItem1EndPoint, _fileSystemItem2EndPoint);

      _folder1.Delete ();

      ClientTransaction.Current.Commit ();

      CheckOriginalOppositeEndPoints ();
      CheckCurrentOppositeEndPoints ();
    }
    
    private T GetEndPoint<T> (RelationEndPointID endPointID) where T : IRelationEndPoint
    {
      var relationEndPointID = endPointID;
      return (T) ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current).GetRelationEndPointWithLazyLoad (relationEndPointID);
    }

    private void CheckOriginalData (params FileSystemItem[] expected)
    {
      Assert.That (_folder1.Properties[typeof (Folder), "FileSystemItems"].GetOriginalValue<ObjectList<FileSystemItem>> (), Is.EquivalentTo (expected));
    }

    private void CheckCurrentData (params FileSystemItem[] expected)
    {
      Assert.That (_folder1.FileSystemItems, Is.EquivalentTo (expected));
    }

    private void CheckOriginalOppositeEndPoints (params RealObjectEndPoint[] expected)
    {
      var loadState = (CompleteCollectionEndPointLoadState) CollectionEndPointTestHelper.GetLoadState (_collectionEndPoint);
      var dataKeeper = (CollectionEndPointDataKeeper) loadState.DataKeeper;
      Assert.That (dataKeeper.OriginalOppositeEndPoints, Is.EquivalentTo (expected));
    }

    private void CheckCurrentOppositeEndPoints (params RealObjectEndPoint[] expected)
    {
      var loadState = (CompleteCollectionEndPointLoadState) CollectionEndPointTestHelper.GetLoadState (_collectionEndPoint);
      var dataKeeper = (CollectionEndPointDataKeeper) loadState.DataKeeper;
      Assert.That (dataKeeper.CurrentOppositeEndPoints, Is.EquivalentTo (expected));
    }

  }
}