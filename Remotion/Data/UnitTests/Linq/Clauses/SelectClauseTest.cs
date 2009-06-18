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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Data.Linq.Clauses.ResultModifications;
using Rhino.Mocks;
using Remotion.Data.Linq.Clauses;

namespace Remotion.Data.UnitTests.Linq.Clauses
{
  [TestFixture]
  public class SelectClauseTest
  {
    private LambdaExpression _legacySelector;
    private Expression _selector;
    private IClause _previousClause;
    private SelectClause _selectClause;
    private ClonedClauseMapping _clonedClauseMapping;

    [SetUp]
    public void SetUp ()
    {
      _legacySelector = ExpressionHelper.CreateLambdaExpression ();
      _selector = ExpressionHelper.CreateExpression();
      _previousClause = ExpressionHelper.CreateClause ();
      _selectClause = new SelectClause (_previousClause, _legacySelector, _selector);
      _clonedClauseMapping = new ClonedClauseMapping ();
    }

    [Test]
    public void InitializeWithExpression ()
    {
      Assert.AreSame (_previousClause, _selectClause.PreviousClause);
      Assert.AreEqual (_legacySelector, _selectClause.LegacySelector);
      Assert.AreEqual (_selector, _selectClause.Selector);
    }

    [Test]
    public void SelectWithMethodCall_ResultModifications ()
    {
      var resultModifierClause = new DistinctResultModification (_selectClause);
      _selectClause.AddResultModification (resultModifierClause);

      Assert.IsNotEmpty (_selectClause.ResultModifications);
      Assert.That (_selectClause.ResultModifications, Is.EqualTo (new[] { resultModifierClause }));
    }

    [Test]
    public void SelectClause_ImplementISelectGroupClause()
    {
      Assert.IsInstanceOfType (typeof(ISelectGroupClause),_selectClause);
    }
        
    [Test]
    public void Accept()
    {
      var repository = new MockRepository();
      var visitorMock = repository.StrictMock<IQueryVisitor>();
      visitorMock.VisitSelectClause (_selectClause);
      repository.ReplayAll();
      _selectClause.Accept (visitorMock);
      repository.VerifyAll();
    }

    [Test]
    public void Clone ()
    {
      var newPreviousClause = ExpressionHelper.CreateMainFromClause ();
      var clone = _selectClause.Clone (newPreviousClause, _clonedClauseMapping);

      Assert.That (clone, Is.Not.Null);
      Assert.That (clone, Is.Not.SameAs (_selectClause));
      Assert.That (clone.LegacySelector, Is.SameAs (_selectClause.LegacySelector));
      Assert.That (clone.Selector, Is.SameAs (_selectClause.Selector));
      Assert.That (clone.PreviousClause, Is.SameAs (newPreviousClause));
    }

    [Test]
    public void Clone_AddsClauseToMapping ()
    {
      var clone = _selectClause.Clone (ExpressionHelper.CreateClause(), _clonedClauseMapping);
      Assert.That (_clonedClauseMapping.GetClause (_selectClause), Is.SameAs (clone));
    }

    [Test]
    public void Clone_ResultModifiers ()
    {
      var newPreviousClause = ExpressionHelper.CreateMainFromClause ();

      var resultModifierClause1 = ExpressionHelper.CreateResultModification (_selectClause);
      _selectClause.AddResultModification (resultModifierClause1);
      var resultModifierClause2 = ExpressionHelper.CreateResultModification (_selectClause);
      _selectClause.AddResultModification (resultModifierClause2);

      var clone = _selectClause.Clone (newPreviousClause, _clonedClauseMapping);

      Assert.That (clone.ResultModifications.Count, Is.EqualTo (2));
      Assert.That (clone.ResultModifications[0], Is.Not.SameAs (resultModifierClause1));
      Assert.That (clone.ResultModifications[0].SelectClause, Is.SameAs (clone));

      Assert.That (clone.ResultModifications[1], Is.Not.SameAs (resultModifierClause2));
    }

    [Test]
    public void Clone_ResultModifiers_PassesMapping ()
    {
      var newPreviousClause = ExpressionHelper.CreateMainFromClause ();

      var resultModifierClauseMock = MockRepository.GenerateMock<ResultModificationBase> (_selectClause, CollectionExecutionStrategy.Instance);
      _selectClause.AddResultModification (resultModifierClauseMock);

      resultModifierClauseMock
          .Expect (mock => mock.Clone (Arg.Is (_clonedClauseMapping)))
          .Return (ExpressionHelper.CreateResultModification());
      resultModifierClauseMock.Replay();

      _selectClause.Clone (newPreviousClause, _clonedClauseMapping);

      resultModifierClauseMock.VerifyAllExpectations();
    }

    [Test]
    public void GetExecutionStrategy_WithoutResultModifiers ()
    {
      Assert.That (_selectClause.GetExecutionStrategy (), Is.SameAs (CollectionExecutionStrategy.Instance));
    }

    [Test]
    public void GetExecutionStrategy_WithResultModifiers ()
    {
      var firstModifier = new FirstResultModification (_selectClause, true);
      _selectClause.AddResultModification (firstModifier);

      Assert.That (_selectClause.GetExecutionStrategy (), Is.SameAs (firstModifier.ExecutionStrategy));
    }

    [Test]
    public void GetExecutionStrategy_WithManyResultModifiers ()
    {
      var takeModifier = new TakeResultModification (_selectClause, 7);
      var distinctModifier = new DistinctResultModification (_selectClause);
      var countModifier = new CountResultModification (_selectClause);
      _selectClause.AddResultModification (takeModifier);
      _selectClause.AddResultModification (distinctModifier);
      _selectClause.AddResultModification (countModifier);

      Assert.That (_selectClause.GetExecutionStrategy (), Is.SameAs (countModifier.ExecutionStrategy));
    }
  }
}
