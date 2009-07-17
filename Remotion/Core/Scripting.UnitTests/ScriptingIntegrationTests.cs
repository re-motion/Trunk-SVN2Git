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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingIntegrationTests
  {
    // Create the ScriptContext for the scripts in this re-motion module. The ScriptContext separates scripts from
    // different modules and prevents ambiguitiy exceptions coming from mixed methods with the same signature
    // on the same class.
    // Note: For this example it is more convenient to use a TypeLevelTypeFilter here; in practice, using a 
    // AssemblyLevelTypeFilter will in most cases be the more fitting choice.
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("YourModuleName.ScriptingIntegrationTests",
      new TypeLevelTypeFilter(new[] {typeof(ProxiedChild), typeof(IAmbigous2), typeof(Document)}));

    // Create a ScriptEnvironment for shared use between all the scripts in your re-motion module (to insulate scripts
    // from each other, simply create a seaparate ScriptEnvironment for each of them).
    private readonly ScriptEnvironment _sharedScriptEnvironment = ScriptEnvironment.Create ();


    [SetUp]
    public void SetUp ()
    {
      ScriptContextTestHelper.ReleaseAllScriptContexts ();
    }

    // Shows how to create and use an ExpressionScript. ExpressionScript|s can only contain a single line and automatically return 
    // the result of evaluating them (wihtout the need for a return statement).
    [Test]
    public void CreateAndUseExpressionScript ()
    {
      // TODO: Show iif and lambda
      const string expressionScriptSourceCode = "doc.DocumentName.Contains('Rec') or doc.DocumentNumber == 123456";

      var doc = new Document ("Receipt");

      // Create a separate script environment for the script expression
      var privateScriptEnvironment = ScriptEnvironment.Create ();
      // Import the CLR (e.g. string etc)
      privateScriptEnvironment.ImportClr();
      // Set variable doc to a Document instance
      privateScriptEnvironment.SetVariable ("doc", doc);

      // Create a script expression which checks the Document object stored in the variable "doc".
      var checkDocumentExpressionScript = 
        new ExpressionScript<bool> (_scriptContext, ScriptLanguageType.Python, expressionScriptSourceCode, privateScriptEnvironment);

      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
      doc.DocumentName = "Record";
      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
      doc.DocumentNumber = 123456;
      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
      doc.DocumentNumber = 21;
      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
    }

    [Test]
    public void CreateAndUseScript ()
    {
      const string scriptFunctionSourceCode = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting.UnitTests')
from Remotion.Scripting.UnitTests.TestDomain import Document
def CreateDocument() :
  return Document('Here is your document, sir.')
";

      // Create a script function called "CreateDocument" which returns a Document object.
      var createDocumentScript = new Script<Document> (_scriptContext, ScriptLanguageType.Python, scriptFunctionSourceCode,
        _sharedScriptEnvironment, "CreateDocument");
      
      Document resultDocument = createDocumentScript.Execute ();

      Assert.That (resultDocument.DocumentName, Is.EqualTo ("Here is your document, sir."));
    }


    [Test]
    public void CreateAndUseScriptWithPrivateScriptEnvironment ()
    {
      const string scriptFunctionSourceCode = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting.UnitTests')
from Remotion.Scripting.UnitTests.TestDomain import Document
def CheckDocument(doc) :
  return doc.DocumentName.Contains('Rec') or doc.DocumentNumber == 123456
";

      var privateScriptEnvironment = ScriptEnvironment.Create ();
      // Create a script function called CheckDocument which takes a Document and returns a bool.
      var checkDocumentScript = new Script<Document,bool> (
        ScriptContext.GetScriptContext ("YourModuleName.ScriptingIntegrationTests"), ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "CheckDocument");

      Assert.That (checkDocumentScript.Execute (new Document ("Receipt", 123456)), Is.True);
      Assert.That (checkDocumentScript.Execute (new Document ("XXX", 123456)), Is.True);
      Assert.That (checkDocumentScript.Execute (new Document ("Rec", 0)), Is.True);
      Assert.That (checkDocumentScript.Execute (new Document ("XXX", 0)), Is.False);
    }


    [Test]
    public void CreateAndUseScriptsInSameScriptEnvironment ()
    {
      // Import the Document class into the shared script environment
      _sharedScriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests.TestDomain", "Document");

      const string scriptFunctionSourceCode0 = @"
def CreateDocument() :
  return Document('invoice',1)
";

      const string scriptFunctionSourceCode1 = @"
def ModifyDocument(doc, newNamePrefix, addToNumber) :
  doc.DocumentName += newNamePrefix
  doc.DocumentNumber += addToNumber
  return doc
";

      // Create a script function which creates a new Document.
      var createDocumentScript = new Script<Document> (_scriptContext, ScriptLanguageType.Python, scriptFunctionSourceCode0,
        _sharedScriptEnvironment, "CreateDocument");

      // Create a script function which modifies the passed Document.
      var modifyDocumentScript = new Script<Document, string, int, Document> (_scriptContext, ScriptLanguageType.Python, scriptFunctionSourceCode1,
        _sharedScriptEnvironment, "ModifyDocument");


      Document doc = createDocumentScript.Execute ();
      modifyDocumentScript.Execute (doc," - processed",1000);

      Assert.That (doc.DocumentName, Is.EqualTo ("invoice - processed"));
      Assert.That (doc.DocumentNumber, Is.EqualTo (1001));
    }
  }
}