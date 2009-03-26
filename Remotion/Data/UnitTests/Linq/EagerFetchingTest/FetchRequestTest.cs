// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.UnitTests.Linq.ParsingTest;

namespace Remotion.Data.UnitTests.Linq.EagerFetchingTest
{
  [TestFixture]
  public class CollectionFetchRequestTest
  {
    private Expression<Func<Student, IEnumerable<int>>> _scoresFetchExpression;
    private Expression<Func<Student, IEnumerable<Student>>> _friendsFetchExpression;
    private CollectionFetchRequest _friendsFetchRequest;
    private Expression<Func<Student, Student>> _otherStudentFetchExpression;
    private CollectionFetchRequest _otherStudentFetchRequest;
    private IQueryable<Student> _studentFromStudentDetailQuery;
    private QueryModel _studentFromStudentDetailQueryModel;

    [SetUp]
    public void SetUp ()
    {
      _scoresFetchExpression = (s => s.Scores);
      _friendsFetchExpression = (s => s.Friends);
      _friendsFetchRequest = new CollectionFetchRequest (_friendsFetchExpression);
      _otherStudentFetchExpression = (s => s.OtherStudent);
      _otherStudentFetchRequest = new CollectionFetchRequest (_otherStudentFetchExpression);

      _studentFromStudentDetailQuery = (from sd in ExpressionHelper.CreateQuerySource_Detail ()
                                        select sd.Student);
      _studentFromStudentDetailQueryModel = ExpressionHelper.ParseQuery (_studentFromStudentDetailQuery);
    }

    [Test]
    [Ignore ("TODO 1115")]
    public void Create_InvalidExpression_NoEnumerableOfT ()
    {
      new CollectionFetchRequest ((Expression<Func<Student, int>>) (s => s.ID));
      Assert.Fail ("Expected exception");
    }

    [Test]
    public void CreateFetchFromClause ()
    {
      // simulate a fetch request for the following: var query = from ... select sd.Student; query.Fetch (s => s.Friends);

      var previousClause = ExpressionHelper.CreateClause();
      Expression<Func<Student_Detail, Student>> selectProjection = sd => sd.Student;
      var selectClause = new SelectClause (previousClause, selectProjection);

      var clause = _friendsFetchRequest.CreateFetchFromClause (selectClause, "studi");
      Assert.That (clause, Is.Not.Null);
      Assert.That (clause.PreviousClause, Is.SameAs (previousClause));
    }

    [Test]
    public void CreateFetchFromClause_FromExpression ()
    {
      // simulate a fetch request for the following: var query = from ... select sd.Student; query.Fetch (s => s.Friends);

      var previousClause = ExpressionHelper.CreateClause ();
      Expression<Func<Student_Detail, Student>> selectProjection = sd => sd.Student;
      var selectClause = new SelectClause (previousClause, selectProjection);

      var clause = _friendsFetchRequest.CreateFetchFromClause (selectClause, "studi");

      // expecting: from studi in sd.Student.Friends
      //            fromExpression: sd => sd.Student.Friends

      Assert.That (clause.FromExpression.Parameters.Count, Is.EqualTo (1));
      Assert.That (clause.FromExpression.Parameters[0], Is.SameAs (selectProjection.Parameters[0]));

      var memberExpression = (MemberExpression) clause.FromExpression.Body;
      Assert.That (memberExpression.Member, Is.EqualTo (typeof (Student).GetProperty ("Friends")));

      var innerMemberExpression = (MemberExpression) memberExpression.Expression;
      Assert.That (innerMemberExpression.Member, Is.EqualTo (typeof (Student_Detail).GetProperty ("Student")));

      var innermostParameterExpression = innerMemberExpression.Expression as ParameterExpression;
      Assert.That (innermostParameterExpression, Is.Not.Null);
      Assert.That (innermostParameterExpression, Is.SameAs (clause.FromExpression.Parameters[0]));
    }

    [Test]
    public void CreateFetchFromClause_ProjectionExpression ()
    {
      // simulate a fetch request for the following: var query = from ... select sd.Student; query.Fetch (s => s.Friends);

      var previousClause = ExpressionHelper.CreateClause ();
      Expression<Func<Student_Detail, Student>> selectProjection = sd => sd.Student;
      var selectClause = new SelectClause (previousClause, selectProjection);

      var clause = _friendsFetchRequest.CreateFetchFromClause (selectClause, "studi");

      // expecting: from studi in sd.Student.Friends
      //            projectionExpression: (sd, studi) => studi

      Assert.That (clause.ProjectionExpression.Parameters.Count, Is.EqualTo (2));
      Assert.That (clause.ProjectionExpression.Parameters[0], Is.SameAs (selectProjection.Parameters[0]));
      Assert.That (clause.ProjectionExpression.Parameters[1].Name, Is.EqualTo ("studi"));
      Assert.That (clause.ProjectionExpression.Parameters[1].Type, Is.EqualTo (typeof (Student)));

      Assert.That (clause.ProjectionExpression.Body, Is.SameAs (clause.ProjectionExpression.Parameters[1]));
    }

    [Test]
    public void CreateFetchQueryModel ()
    {
      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);
      Assert.That (fetchQueryModel, Is.Not.Null);
      Assert.That (fetchQueryModel, Is.Not.SameAs (_studentFromStudentDetailQueryModel));
    }

    [Test]
    [Ignore ("TODO 1115")]
    public void CreateFetchQueryModel_ObjectFetch ()
    {
      var fetchQueryModel = _otherStudentFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);
      Assert.That (fetchQueryModel, Is.Not.Null);
      Assert.That (fetchQueryModel, Is.Not.SameAs (_studentFromStudentDetailQueryModel));

      // no additional from clauses here
      Assert.That (fetchQueryModel.BodyClauses.Count, Is.EqualTo (_studentFromStudentDetailQueryModel.BodyClauses.Count));
    }

    [Test]
    public void CreateFetchQueryModel_MainFromClause ()
    {
      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateQuerySource_Detail()
      // from <x> in sd.Student.Friends
      // select <x>

      ExpressionTreeComparer.CheckAreEqualTrees (fetchQueryModel.MainFromClause.QuerySource, _studentFromStudentDetailQueryModel.MainFromClause.QuerySource);
      Assert.That (fetchQueryModel.MainFromClause.Identifier, Is.EqualTo (_studentFromStudentDetailQueryModel.MainFromClause.Identifier));
    }

    [Test]
    public void CreateFetchQueryModel_MemberFromClause ()
    {
      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateQuerySource_Detail()
      // from <x> in sd.Student.Friends
      // select <x>

      Assert.That (fetchQueryModel.BodyClauses.Count, Is.EqualTo (1));
      var memberFromClause = (MemberFromClause) fetchQueryModel.BodyClauses.Single ();
      Expression<Func<Student_Detail, List<Student>>> expectedFromExpression = sd => sd.Student.Friends;
      ExpressionTreeComparer.CheckAreEqualTrees (memberFromClause.FromExpression, expectedFromExpression);
    }

    [Test]
    public void CreateFetchQueryModel_SelectClause ()
    {
      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateQuerySource_Detail()
      // from <x> in sd.Student.Friends
      // select <x>

      var selectClause = (SelectClause) fetchQueryModel.SelectOrGroupClause;
      Assert.That (selectClause.ProjectionExpression.Body, Is.SameAs (selectClause.ProjectionExpression.Parameters[0]));
    }

    [Test]
    [Ignore ("TODO 1115")]
    public void CreateFetchQueryModel_ObjectFetch_SelectClause ()
    {
      var fetchQueryModel = _otherStudentFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateQuerySource_Detail()
      // select sd.Student.OtherStudent

      var selectClause = (SelectClause) fetchQueryModel.SelectOrGroupClause;
      var expectedExpression = ExpressionHelper.CreateLambdaExpression<Student_Detail, Student> (sd => sd.Student.OtherStudent);

      ExpressionTreeComparer.CheckAreEqualTrees (selectClause.ProjectionExpression, expectedExpression);
    }

    [Test]
    public void CreateFetchQueryModel_Twice_MemberFromClause ()
    {
      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);

      var fetchRequest2 = new CollectionFetchRequest (_scoresFetchExpression);
      var fetchQueryModel2 = fetchRequest2.CreateFetchQueryModel (fetchQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateQuerySource_Detail()
      // from <x> in sd.Student.Friends
      // from <y> in <x>.Scores
      // select <y>

      Assert.That (fetchQueryModel2.BodyClauses.Count, Is.EqualTo (2));
      var memberFromClause1 = (MemberFromClause) fetchQueryModel2.BodyClauses.First ();
      var memberFromClause2 = (MemberFromClause) fetchQueryModel2.BodyClauses.Last ();

      Assert.That (memberFromClause1.Identifier.Name, Is.Not.EqualTo (memberFromClause2.Identifier.Name));

      Assert.That (memberFromClause2.MemberExpression.Expression, Is.SameAs (memberFromClause2.FromExpression.Parameters[0]));
      Assert.That (memberFromClause2.MemberExpression.Member, Is.EqualTo (typeof (Student).GetProperty ("Scores")));
    }

    [Test]
    public void CreateFetchQueryModel_Twice_SelectClause ()
    {
      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);
      
      var fetchRequest2 = new CollectionFetchRequest (_scoresFetchExpression);
      var fetchQueryModel2 = fetchRequest2.CreateFetchQueryModel (fetchQueryModel);

      // expecting:
      // from sd in ExpressionHelper.CreateQuerySource_Detail()
      // from <x> in sd.Student.Friends
      // from <y> in <x>.Scores
      // select <y>

      var selectClause = (SelectClause) fetchQueryModel2.SelectOrGroupClause;
      Assert.That (selectClause.ProjectionExpression.Body, Is.SameAs (selectClause.ProjectionExpression.Parameters[0]));
    }

    [Test]
    public void CreateFetchQueryModel_ResultModifierClausesAreCloned ()
    {
      var selectClause = (SelectClause) _studentFromStudentDetailQueryModel.SelectOrGroupClause;
      var modifier = ExpressionHelper.CreateResultModifierClause (selectClause, selectClause);
      selectClause.AddResultModifierData (modifier);

      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);
      var fetchSelectClause = (SelectClause)fetchQueryModel.SelectOrGroupClause;

      Assert.That (fetchSelectClause.ResultModifierClauses.Count, Is.EqualTo (1));
      Assert.That (fetchSelectClause.ResultModifierClauses[0].ResultModifier, Is.SameAs (modifier.ResultModifier));
    }

    [Test]
    public void CreateFetchQueryModel_ResultModifierClausesAreClonedWithCorrectPreviousClauses ()
    {
      var selectClause = (SelectClause) _studentFromStudentDetailQueryModel.SelectOrGroupClause;
      var modifier1 = ExpressionHelper.CreateResultModifierClause (selectClause, selectClause);
      var modifier2 = ExpressionHelper.CreateResultModifierClause (modifier1, selectClause);
      selectClause.AddResultModifierData (modifier1);
      selectClause.AddResultModifierData (modifier2);

      var fetchQueryModel = _friendsFetchRequest.CreateFetchQueryModel (_studentFromStudentDetailQueryModel);
      var fetchSelectClause = (SelectClause) fetchQueryModel.SelectOrGroupClause;

      Assert.That (fetchSelectClause.ResultModifierClauses.Count, Is.EqualTo (2));
      Assert.That (fetchSelectClause.ResultModifierClauses[0].SelectClause, Is.SameAs (fetchSelectClause));
      Assert.That (fetchSelectClause.ResultModifierClauses[0].PreviousClause, Is.SameAs (fetchSelectClause));
      Assert.That (fetchSelectClause.ResultModifierClauses[1].SelectClause, Is.SameAs (fetchSelectClause));
      Assert.That (fetchSelectClause.ResultModifierClauses[1].PreviousClause, Is.SameAs (fetchSelectClause.ResultModifierClauses[0]));
    }
  }
}