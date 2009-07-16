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

    [Test]
    public void CreateAndUseScript ()
    {
      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting.UnitTests')
from Remotion.Scripting.UnitTests.TestDomain import Document
def CreateDocumentPythonFunction() :
  return Document('Here is your document, sir.')
";

      // Create a script function called "CreateDocumentPythonFunction" which returns a Document object from scriptText.
      // Note: "CreateDocumentPythonFunction" was only chosen for this tutorial; in practice calling the Python function just "CreateDocument"
      // makes more sense.
      var createDocumentScript = new Script<Document> (_scriptContext, ScriptLanguageType.Python, scriptText,
        _sharedScriptEnvironment, "CreateDocumentPythonFunction");
      
      Document resultDocument = createDocumentScript.Execute ();
      Assert.That (resultDocument.DocumentName, Is.EqualTo ("Here is your document, sir."));

      // ScriptContext.GetScriptContext ("YourModuleName.ScriptingIntegrationTests")
    }


    [Test]
    public void CreateAndUseScriptWithPrivateScriptEnvironment ()
    {
      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting.UnitTests')
from Remotion.Scripting.UnitTests.TestDomain import Document
def CheckDocumentPythonFunction(doc) :
  return doc.DocumentName.Contains('Rec') or doc.DocumentNumber == 123456
";

      var privateScriptEnvironment = ScriptEnvironment.Create ();
      // Create a script function called CheckDocumentPythonFunction which takes a Document and returns a bool.
      var script = new Script<Document,bool> (
        ScriptContext.GetScriptContext ("YourModuleName.ScriptingIntegrationTests"), ScriptLanguageType.Python,
        scriptText, privateScriptEnvironment, "CheckDocumentPythonFunction");

      Assert.That (script.Execute (new Document ("Receipt", 123456)), Is.True);
      Assert.That (script.Execute (new Document ("XXX", 123456)), Is.True);
      Assert.That (script.Execute (new Document ("Rec", 0)), Is.True);
      Assert.That (script.Execute (new Document ("XXX", 0)), Is.False);

    } 
  }
}