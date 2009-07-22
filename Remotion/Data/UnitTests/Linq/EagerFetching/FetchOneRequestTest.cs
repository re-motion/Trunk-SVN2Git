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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.UnitTests.Linq.Parsing;
using Remotion.Data.UnitTests.Linq.TestDomain;

namespace Remotion.Data.UnitTests.Linq.EagerFetching
{
  [TestFixture]
  public class FetchOneRequestTest
  {
    private Expression<Func<Student, Student>> _otherStudentFetchExpression;
    private FetchOneRequest _otherStudentFetchRequest;
    private IQueryable<Student> _studentFromStudentDetailQuery;
    private QueryModel _studentFromStudentDetailQueryModel;

    [SetUp]
    public void SetUp ()
    {
      _otherStudentFetchExpression = (s => s.OtherStudent);
      _otherStudentFetchRequest = new FetchOneRequest (_otherStudentFetchExpression);

      _studentFromStudentDetailQuery = (from sd in ExpressionHelper.CreateStudentDetailQueryable ()
                                        select sd.Student);
      _studentFromStudentDetailQueryModel = ExpressionHelper.ParseQuery (_studentFromStudentDetailQuery);
    }

    [Test]
    public void CreateFetchQueryModel ()
    {
      var fetchQueryModel = _otherStudentFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);
      Assert.That (fetchQueryModel, Is.Not.Null);
      Assert.That (fetchQueryModel, Is.Not.SameAs (_studentFromStudentDetailQueryModel));

      // no additional from clauses here
      Assert.That (fetchQueryModel.BodyClauses.Count, Is.EqualTo (_studentFromStudentDetailQueryModel.BodyClauses.Count));
    }

    [Test]
    public void CreateFetchQueryModel_ObjectFetch_SelectClause ()
    {
      var fetchQueryModel = _otherStudentFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateStudentDetailQueryable()
      // select sd.Student.OtherStudent

      var selectClause = fetchQueryModel.SelectClause;
      var expectedExpression = ExpressionHelper.Resolve<Student_Detail, Student> (fetchQueryModel.MainFromClause, sd => sd.Student.OtherStudent);
      ExpressionTreeComparer.CheckAreEqualTrees (selectClause.Selector, expectedExpression);
    }

    [Test]
    public void CreateFetchQueryModel_ObjectFetch_SelectClausee_PreviousClauseIsClauseInNewQueryModel ()
    {
      var fetchQueryModel = _otherStudentFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      // from sd in ExpressionHelper.CreateStudentDetailQueryable()
      // select sd.Student.OtherStudent

      var selectClause = fetchQueryModel.SelectClause;
      var innerMemberExpression = (MemberExpression)((MemberExpression) selectClause.Selector).Expression; // sd.Student
      Assert.That (((QuerySourceReferenceExpression) innerMemberExpression.Expression).ReferencedQuerySource,  Is.SameAs (fetchQueryModel.MainFromClause));
    }

    [Test]
    public void CreateFetchQueryModel_Twice_SelectClause ()
    {
      var fetchQueryModel = _otherStudentFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      var fetchRequest2 = new FetchOneRequest (_otherStudentFetchExpression);
      var fetchQueryModel2 = fetchRequest2.CreateFetchQueryModel (fetchQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateStudentDetailQueryable()
      // select sd.Student.OtherStudent.OtherStudent

      var selectClause = fetchQueryModel2.SelectClause;
      var expectedExpression = ExpressionHelper.Resolve<Student_Detail, Student> (fetchQueryModel2.MainFromClause, sd => sd.Student.OtherStudent.OtherStudent);
      ExpressionTreeComparer.CheckAreEqualTrees (selectClause.Selector, expectedExpression);
    }
  }
}