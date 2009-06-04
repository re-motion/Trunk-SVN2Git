// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Linq.Expressions;
//using Extension1;
using Microsoft;
//using Microsoft.Linq.Expressions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remtion.Scripting;


namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class IronPythonAsFormulaLanguageSpikeTest
  {
    [Test]
    public void ExpressionBaseTest ()
    {
      const string scriptText1 = "2+7+4";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);
      var result = scriptSource1.Execute (scope1);

      Assert.That (result, Is.EqualTo (13));
    }


    [Test]
    [ExpectedException (ExceptionType = typeof (Microsoft.Scripting.SyntaxErrorException), ExpectedMessage = "invalid syntax")]
    public void ExpressionIsSingleLineTest ()
    {
      const string scriptText1 =
          @"
17";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);
      scriptSource1.Execute (scope1);
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (Microsoft.Scripting.SyntaxErrorException), ExpectedMessage = "unexpected token '17'")]
    public void ExpressionIsSingleLineTest2 ()
    {
      const string scriptText1 =
          @"33
17";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);
      scriptSource1.Execute (scope1);
    }


    [Test]
    [ExpectedException (ExceptionType = typeof (Microsoft.Scripting.SyntaxErrorException), ExpectedMessage = "unexpected token '='")]
    public void ExpressionAllowsNoVariableAssignment ()
    {
      const string scriptText1 =
          @"x=3
x+=17";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope0 = pythonEngine.CreateScope ();
      ScriptSource scriptSource0 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Statements);
      scriptSource0.Execute (scope0);
      int x = -1;
      scope0.TryGetVariable ("x", out x);
      Assert.That (x, Is.EqualTo (20));

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);
      scriptSource1.Execute (scope1);
    }


    [Test]
    [ExpectedException (ExceptionType = typeof (Microsoft.Scripting.SyntaxErrorException), ExpectedMessage = "unexpected token 'import'")]
    public void ExpressionAllowsNoImportTest ()
    {
      const string scriptText1 =
          @"import clr";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);
      scriptSource1.Execute (scope1);
    }



    [Test]
    public void ExpressionTest ()
    {
      const string scriptText1 =
          @"(val < 100.0 and val > 0) + val + doc.Value";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);

      var doc = new Document { Name = "Einzug", Value = 7 };

      scope1.SetVariable ("val", 11);
      scope1.SetVariable ("doc", doc);
      var result = scriptSource1.Execute (scope1);

      Assert.That (result, Is.EqualTo (19));
    }



    [Test]
    [ExpectedException (ExceptionType = typeof (Microsoft.Scripting.SyntaxErrorException), ExpectedMessage = "invalid syntax")]
    public void ExpressionUsingIfFailsTest ()
    {
      const string scriptText1 =
          @"if val < 100.0: val 
else: 100.0";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope0 = pythonEngine.CreateScope ();
      scope0.SetVariable ("val", 11);
      ScriptSource scriptSource0 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Statements);
      scriptSource0.Execute (scope0);
      Assert.That (true);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);
      scriptSource1.Execute (scope1);
    }


//    // Conclusion: Cannot use SourceCodeKind.Statements together with simply moving the user code into a Py function to disallow import etc.
//    [Test]
//    public void IronyPythonFunctionDoesAllowImportStatementTest ()
//    {
//      const string scriptText1 =
//          @"
//def remotionScript() :
//  import clr
//  clr.AddReferenceByPartialName('Extension1')
//  from Extension1 import *
//  return Class1('xyz')
//";

//      var dlrHost = DlrHost.New ();
//      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHost.EngineType.Python);

//      var scope0 = pythonEngine.CreateScope ();
//      ScriptSource scriptSource0 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Statements);
//      scriptSource0.Execute (scope0);
//      var remotionScript = scope0.GetVariable<Func<Class1>> ("remotionScript");
//      var result = remotionScript ();
//      Assert.That (result.Name, Is.EqualTo ("xyz"));
//    }


    [Test]
    public void ExpressionWithImmediateIfTest ()
    {
      const string scriptText1 = "IIf(val < 100.0,val,100.0)";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);

      scope1.SetVariable ("val", 111);
      scope1.SetVariable ("IIf", new System.Func<bool, object, object, object> ((cond, trueVal, falseVal) => cond ? trueVal : falseVal));
      var result = scriptSource1.Execute (scope1);

      Assert.That (result, Is.EqualTo (100));
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (System.ArgumentNullException))]
    public void ExpressionWithImmediateIfLazyEvaluationTest ()
    {
      const string scriptText1 = "IIf(True,val,dummy.NonExistingAttribute)";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);

      scope1.SetVariable ("val", 33);
      scope1.SetVariable ("dummy", null);
      scope1.SetVariable ("IIf", new System.Func<bool, object, object, object> ((cond, trueVal, falseVal) => cond ? trueVal : falseVal));
      scriptSource1.Execute (scope1);
    }


    [Test]
    [ExpectedException (ExceptionType = typeof (System.MissingMemberException), ExpectedMessage = "'int' object has no attribute 'NonExistingAttribute'")]
    public void ExpressionWithImmediateIfUsingSystemLinqExpressionsExpressionTest ()
    {
      const string scriptText1 = "IIf(True,val,val.NonExistingAttribute)";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);

      scope1.SetVariable ("val", 111);
      scope1.SetVariable ("IIf", new System.Func<bool, Expression, Expression, object> ((cond, trueVal, falseVal) => cond ? trueVal : falseVal));
      scriptSource1.Execute (scope1);
    }

    [Test]
    public void ExpressionWithImmediateIfUsingLambdaTest ()
    {
      const string scriptText1 = "LazyIIf(True,lambda:val,lambda:val.NonExistingAttribute)";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();
      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);

      scope1.SetVariable ("val", 33);
      // Define LazyIIf in C#
      scope1.SetVariable ("LazyIIf", new System.Func<bool, System.Func<object>, System.Func<object>, object> ((cond, trueVal, falseVal) => cond ? trueVal () : falseVal ()));
      var result = scriptSource1.Execute (scope1);

      Assert.That (result, Is.EqualTo (33));
    }


    [Test]
    public void ExpressionWithImmediateIfUsingLambdaTest2 ()
    {
      // Define LazyIIf in IPy
      const string scriptTextPythonLazyIIf = @"
def LazyIIf(cond, trueVal, falseVal):
  if cond:
    return trueVal()
  else:
    return falseVal()
";
      const string scriptText1 = "LazyIIf(val<100,lambda:val,lambda:val.NonExistingAttribute)";

      var dlrHost = DlrHostSpike.New ();
      ScriptEngine pythonEngine = dlrHost.GetEngine (DlrHostSpike.EngineType.Python);

      var scope1 = pythonEngine.CreateScope ();

      ScriptSource scriptPythonLazyIIf = pythonEngine.CreateScriptSourceFromString (scriptTextPythonLazyIIf, SourceCodeKind.Statements);
      scriptPythonLazyIIf.Execute (scope1);

      ScriptSource scriptSource1 = pythonEngine.CreateScriptSourceFromString (scriptText1, SourceCodeKind.Expression);

      scope1.SetVariable ("val", 33);
      var result = scriptSource1.Execute (scope1);

      Assert.That (result, Is.EqualTo (33));
    }


    [Test]
    [Explicit]
    public void ParamsDelegateTest ()
    {
      ParamsDelegate fn = ParamsDelegateTestHelper;
      fn (1, 2, 3);
      fn (new object[] { 1, 2, 3 });
    }


    public delegate void ParamsDelegate (params object[] arguments);

    public void ParamsDelegateTestHelper (params object[] arguments)
    {
      //To.ConsoleLine.e (() => arguments.Length).e (() => arguments);
    }

    public class Document
    {
      public string Name { get; set; }
      public double Value { get; set; }
    }
  }
}