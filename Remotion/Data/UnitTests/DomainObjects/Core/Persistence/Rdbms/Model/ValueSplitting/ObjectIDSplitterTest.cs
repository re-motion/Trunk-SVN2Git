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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.ValueSplitting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.ValueSplitting
{
  [TestFixture]
  public class ObjectIDSplitterTest : StandardMappingTest
  {
    private IValueSplitter _valueSplitterMock;
    private IValueSplitter _classIDSplitterMock;
    private ObjectIDSplitter _objectIDSplitter;

    private ObjectID _objectID;
    private IStorageTypeInformation _storageTypeInfo1;
    private IStorageTypeInformation _storageTypeInfo2;
    private IStorageTypeInformation _storageTypeInfo3;

    public override void SetUp ()
    {
      base.SetUp();

      _valueSplitterMock = MockRepository.GenerateStrictMock<IValueSplitter> ();
      _classIDSplitterMock = MockRepository.GenerateStrictMock<IValueSplitter> ();

      _objectIDSplitter = new ObjectIDSplitter (_valueSplitterMock, _classIDSplitterMock);

      _objectID = DomainObjectIDs.Order1;
      _storageTypeInfo1 = MockRepository.GenerateStub<IStorageTypeInformation> ();
      _storageTypeInfo2 = MockRepository.GenerateStub<IStorageTypeInformation> ();
      _storageTypeInfo3 = MockRepository.GenerateStub<IStorageTypeInformation> ();
    }

    [Test]
    public void Split ()
    {
      _valueSplitterMock
          .Expect (mock => mock.Split(_objectID.Value))
          .Return (new[] { new TypedValue (1, _storageTypeInfo1), new TypedValue(2, _storageTypeInfo2) });
      _valueSplitterMock.Replay();

      _classIDSplitterMock
          .Expect (mock => mock.Split (_objectID.ClassID))
          .Return (new[] { new TypedValue (3, _storageTypeInfo3) });
      _classIDSplitterMock.Replay();

      var result = _objectIDSplitter.Split (_objectID).ToArray();

      _valueSplitterMock.VerifyAllExpectations();
      _classIDSplitterMock.VerifyAllExpectations();

      Assert.That (
          result,
          Is.EqualTo (new[] { new TypedValue (1, _storageTypeInfo1), new TypedValue (2, _storageTypeInfo2), new TypedValue (3, _storageTypeInfo3) }));
    }

    [Test]
    public void Split_Null ()
    {
      _valueSplitterMock
          .Expect (mock => mock.Split (null))
          .Return (new[] { new TypedValue (null, _storageTypeInfo1) });
      _valueSplitterMock.Replay ();

      _classIDSplitterMock
          .Expect (mock => mock.Split (null))
          .Return (new[] { new TypedValue (null, _storageTypeInfo2) });
      _classIDSplitterMock.Replay ();

      var result = _objectIDSplitter.Split (null).ToArray ();

      _valueSplitterMock.VerifyAllExpectations ();
      _classIDSplitterMock.VerifyAllExpectations ();

      Assert.That (
          result,
          Is.EqualTo (new[] { new TypedValue (null, _storageTypeInfo1), new TypedValue (null, _storageTypeInfo2) }));
    }

    [Test]
    public void SplitForComparison ()
    {
      _valueSplitterMock
          .Expect (mock => mock.SplitForComparison (_objectID.Value))
          .Return (new[] { new TypedValue (1, _storageTypeInfo1), new TypedValue (2, _storageTypeInfo2) });
      _valueSplitterMock.Replay ();
      _classIDSplitterMock.Replay ();

      var result = _objectIDSplitter.SplitForComparison (_objectID).ToArray ();

      _valueSplitterMock.VerifyAllExpectations ();
      _classIDSplitterMock.VerifyAllExpectations ();

      Assert.That (
          result,
          Is.EqualTo (new[] { new TypedValue (1, _storageTypeInfo1), new TypedValue (2, _storageTypeInfo2) }));
    }

    [Test]
    public void SplitForComparison_Null ()
    {
      _valueSplitterMock
          .Expect (mock => mock.SplitForComparison (null))
          .Return (new[] { new TypedValue (null, _storageTypeInfo1) });
      _valueSplitterMock.Replay ();
      _classIDSplitterMock.Replay ();

      var result = _objectIDSplitter.SplitForComparison (null).ToArray ();

      _valueSplitterMock.VerifyAllExpectations ();
      _classIDSplitterMock.VerifyAllExpectations ();

      Assert.That (
          result,
          Is.EqualTo (new[] { new TypedValue (null, _storageTypeInfo1) }));
    }

    [Test]
    [Ignore ("TODO 4218")]
    public void Combine ()
    {
    }
  }
}