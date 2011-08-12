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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.ValueSplitting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.ValueSplitting
{
  [TestFixture]
  public class SimpleValueSplitterTest
  {
    private IStorageTypeInformation _storageTypeInformationStub;
    private SimpleValueSplitter _valueSplitter;
    private IEnumerator<object> _enumeratorStrictMock;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationStub = MockRepository.GenerateStrictMock<IStorageTypeInformation>();
      _enumeratorStrictMock = MockRepository.GenerateStrictMock<IEnumerator<object>>();
      _valueSplitter = new SimpleValueSplitter (_storageTypeInformationStub);
    }

    [Test]
    public void Split ()
    {
      var result = _valueSplitter.Split (17);

      Assert.That (result, Is.EqualTo (new[] { new TypedValue (17, _storageTypeInformationStub) }));
    }

    [Test]
    public void Split_Null ()
    {
      var result = _valueSplitter.Split (null);

      Assert.That (result, Is.EqualTo (new[] { new TypedValue (null, _storageTypeInformationStub) }));
    }

    [Test]
    public void Combine ()
    {
      using (_enumeratorStrictMock.GetMockRepository ().Ordered ())
      {
        _enumeratorStrictMock.Expect (mock => mock.MoveNext()).Return (true);
        _enumeratorStrictMock.Expect (mock => mock.Current).Return (12);
      }

      _enumeratorStrictMock.Replay();

      var result = _valueSplitter.Combine (_enumeratorStrictMock);

      _enumeratorStrictMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (12));
    }

    [Test]
    public void Combine_NoMoreData ()
    {
      _enumeratorStrictMock.Stub (mock => mock.MoveNext ()).Return (false);
      Assert.That (
          () => _valueSplitter.Combine (_enumeratorStrictMock),
          Throws.Exception.TypeOf<InvalidOperationException>().With.Message.EqualTo ("Expected more data."));
    }
  }
}