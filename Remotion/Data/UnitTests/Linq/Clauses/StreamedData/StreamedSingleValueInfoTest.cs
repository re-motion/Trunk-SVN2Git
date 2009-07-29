// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.UnitTests.Linq.TestDomain;
using Rhino.Mocks;
using Remotion.Collections;

namespace Remotion.Data.UnitTests.Linq.Clauses.StreamedData
{
  [TestFixture]
  public class StreamedSingleValueInfoTest
  {
    private StreamedSingleValueInfo _streamedSingleValueInfoWithDefault;
    private StreamedSingleValueInfo _streamedSingleValueInfoNoDefault;

    [SetUp]
    public void SetUp ()
    {
      _streamedSingleValueInfoWithDefault = new StreamedSingleValueInfo (typeof (Student), true);
      _streamedSingleValueInfoNoDefault = new StreamedSingleValueInfo (typeof (Student), false);
    }

    [Test]
    public void ExecuteQueryModel ()
    {
      var queryModel = ExpressionHelper.CreateQueryModel ();
      var fetchRequests = new FetchRequestBase[0];
      
      Student student1 = new Student();
      var expectedStudents = new[] { student1 };
      
      var executorMock = MockRepository.GenerateMock<IQueryExecutor> ();
      executorMock.Expect (mock => mock.ExecuteCollection<Student> (queryModel, fetchRequests)).Return (expectedStudents);
      
      var streamedData = _streamedSingleValueInfoWithDefault.ExecuteQueryModel (queryModel, fetchRequests, executorMock);

      executorMock.VerifyAllExpectations ();

      Assert.That (streamedData, Is.InstanceOfType (typeof (StreamedValue)));
      Assert.That (streamedData.DataInfo, Is.SameAs (_streamedSingleValueInfoWithDefault));
      Assert.That (streamedData.Value, Is.EqualTo (student1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Test")]
    public void ExecuteQueryModel_WithException ()
    {
      var queryModel = ExpressionHelper.CreateQueryModel ();
      var fetchRequests = new FetchRequestBase[0];

      var executorMock = MockRepository.GenerateMock<IQueryExecutor> ();
      executorMock.Expect (mock => mock.ExecuteCollection<Student> (queryModel, fetchRequests)).Throw (new InvalidOperationException ("Test"));

      _streamedSingleValueInfoWithDefault.ExecuteQueryModel (queryModel, fetchRequests, executorMock);
    }

    [Test]
    public void ExecuteQueryModel_WithDefaultWhenEmpty ()
    {
      var queryModel = ExpressionHelper.CreateQueryModel ();
      var fetchRequests = new FetchRequestBase[0];

      var executorMock = MockRepository.GenerateMock<IQueryExecutor> ();
      executorMock.Expect (mock => mock.ExecuteCollection<Student> (queryModel, fetchRequests)).Return (new Student[0]);

      var streamedData = _streamedSingleValueInfoWithDefault.ExecuteQueryModel (queryModel, fetchRequests, executorMock);

      executorMock.VerifyAllExpectations ();

      Assert.That (streamedData, Is.InstanceOfType (typeof (StreamedValue)));
      Assert.That (streamedData.Value, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void ExecuteQueryModel_NoDefaultWhenEmpty ()
    {
      var queryModel = ExpressionHelper.CreateQueryModel ();
      var fetchRequests = new FetchRequestBase[0];

      var executorMock = MockRepository.GenerateMock<IQueryExecutor> ();
      executorMock.Expect (mock => mock.ExecuteCollection<Student> (queryModel, fetchRequests)).Return (new Student[0]);

      _streamedSingleValueInfoNoDefault.ExecuteQueryModel (queryModel, fetchRequests, executorMock);
    }
  }
}