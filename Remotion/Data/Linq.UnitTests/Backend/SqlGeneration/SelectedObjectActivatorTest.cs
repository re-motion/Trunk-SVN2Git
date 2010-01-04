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
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.SqlGeneration;
using Remotion.Data.Linq.UnitTests.Backend.SqlGeneration.SqlServer;
using Remotion.Data.Linq.UnitTests.Utilities;

namespace Remotion.Data.Linq.UnitTests.Backend.SqlGeneration
{
  [TestFixture]
  public class SelectedObjectActivatorTest
  {
    [Test]
    public void GetValue_ForColumn ()
    {
      Column column = new Column(new Table(),"column");
      IEvaluation evaluation = column;
      object[] values = new object[] { 1 };

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      object result = selectedObjectActivator.CreateSelectedObject (values);

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void GetConstant_ForConstant ()
    {
      Constant constant = new Constant("test");
      IEvaluation evaluation = constant;
      object[] values = new object[] {};

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      object result = selectedObjectActivator.CreateSelectedObject (values);

      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void NewObject ()
    {
      NewObject newObject = new NewObject (typeof (object).GetConstructors()[0]);
      IEvaluation evaluation = newObject;
      object[] values = new object[] { };

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      object result = selectedObjectActivator.CreateSelectedObject (values);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.GetType(), Is.EqualTo (typeof (object)));
    }

    [Test]
    public void NewObject_WithArguments ()
    {
      Constant constant = new Constant("1");
      Column column = new Column (new Table (), "column");
      NewObject newObject = new NewObject (typeof (Tuple<string,string>).GetConstructors ()[0], constant, column);

      IEvaluation evaluation = newObject;
      object[] values = new object[] { "test" };

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      object result = selectedObjectActivator.CreateSelectedObject (values);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.EqualTo (new Tuple<string,string>("1", "test")));  
    }

    [Test]
    public void NewObject_NewObject ()
    {
      Constant constant = new Constant ("1");
      Column column = new Column (new Table(), "column");
      Column innerColumn = new Column (new Table(), "innerColumn");
      NewObject innerNewObject = new NewObject (typeof (DateTime).GetConstructor (new[] { typeof (long) }), innerColumn);
      NewObject newObject = new NewObject (typeof (Tuple<string, string, DateTime>).GetConstructors()[0], constant, column, innerNewObject);

      IEvaluation evaluation = newObject;
      object[] values = new object[] { "test", 1234L };

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      object result = selectedObjectActivator.CreateSelectedObject (values);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.EqualTo (new Tuple<string, string, DateTime> ("1", "test", new DateTime (1234L))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Too few values.\r\nParameter name: values")]
    public void TooFewValues ()
    {
      Column column = new Column ();
      IEvaluation evaluation = column;
      object[] values = new object[] { };

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      selectedObjectActivator.CreateSelectedObject (values);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Too many values.\r\nParameter name: values")]
    public void TooManyValues ()
    {
      Constant constant = new Constant ("test");
      IEvaluation evaluation = constant;
      object[] values = new object[] { 1 };

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      selectedObjectActivator.CreateSelectedObject (values);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Evaluation type DummyEvaluation is not supported.")]
    public void NonSupportedEvaluations ()
    {
      var mockEvaluation = new DummyEvaluation ();
      IEvaluation evaluation = mockEvaluation;
      object[] values = new object[] { };

      SelectedObjectActivator selectedObjectActivator = new SelectedObjectActivator (evaluation);
      selectedObjectActivator.CreateSelectedObject (values);
    }
  }
}
