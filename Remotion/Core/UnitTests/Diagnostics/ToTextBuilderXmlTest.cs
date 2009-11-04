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
using System.IO;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.UnitTests.Diagnostics.TestDomain;
using Rhino.Mocks;


namespace Remotion.UnitTests.Diagnostics
{

  
  [TestFixture]
  public class ToTextBuilderXmlTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (false);

    [Test]
    [Ignore]
    public void XmlTest ()
    {
      var stringWriter = new StringWriter ();
      XmlTextWriter xmlWriter = new XmlTextWriter (stringWriter);
      xmlWriter.Formatting = Formatting.Indented;
      xmlWriter.WriteStartElement ("test");
      xmlWriter.WriteAttributeString ("name", "A test tag");
      xmlWriter.WriteValue ("Some text");
      xmlWriter.WriteValue (", and some entities: <>&");
      xmlWriter.WriteEndElement ();
      xmlWriter.Flush();
      log.It ("xml=" + stringWriter);
    }

    private const string toXmlInputString = "test string <>& after entities";
    private const string toXmlresultString = "test string &lt;&gt;&amp; after entities";

    [Test]
    public void RawStringWithEntitiesXmlTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Open();
      toTextBuilderXml.s (toXmlInputString);
      toTextBuilderXml.Close ();
      var result = stringWriter.ToString();
      log.It ("xml=" + result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains (toXmlresultString));
    }

    [Test]
    public void ToTextXmlTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml =  CreateTextBuilderXml(stringWriter, false);
      toTextBuilderXml.Open ();
      toTextBuilderXml.e (toXmlInputString);
      toTextBuilderXml.Close ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains (toXmlresultString));
    }

    [Test]
    public void ToTextXmlSequenceTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Open ();
      toTextBuilderXml.sb ().e (toXmlInputString).se ();
      toTextBuilderXml.Close ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, Is.EqualTo ("<remotion><seq><e>" + toXmlresultString  + "</e></seq></remotion>"));
    }


    [Test]
    public void ToTextXmlVarTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Open ();

      var x = 123.456;
      toTextBuilderXml.sb ().e(() => x).se ();
      toTextBuilderXml.Close ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<var name=\"x\">123.456</var>"));
    }


    [Test]
    [Ignore]
    public void ToTextXmlInstanceTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, true);
      toTextBuilderXml.Open ();
      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb().e ("This is a sample specifc type handler - important members listed first:").e (() => t.Int).e (() => t.Name).se ());

      //var simpleTest = new ToTextProviderTest.TestSimple ();
      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      var aNumber = 123.456;
      toTextBuilderXml.sb ().e (() => aNumber).e("ABC").e(simpleTest).se ();
      toTextBuilderXml.Close ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      //Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }


    [Test]
    public void ToTextXmlBeginEndTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);

      toTextBuilderXml.Open ();
      toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<remotion />"));
    }


    [Test]
    [Ignore]
    public void ToTextXmlMultiTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);

      toTextBuilderXml.Open();

      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb ().e ("This is a sample specifc type handler - important members listed first:").e (() => t.Int).e (() => t.Name).se ());

      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      int counter = 1;
      toTextBuilderXml.sb ().e (() => counter).e (simpleTest).se ();
      toTextBuilderXml.sb ().e (() => counter).e (simpleTest).se ();
      toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      log.It (result);
      //Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }

    [Test]
    [Ignore]
    public void ToTextXmlLoopTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, true);

      toTextBuilderXml.Open ();

      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb ().e ("This is a sample specifc type handler - important members listed first:").e (() => t.Int).e (() => t.Name).se ());

      //var simpleTest = new ToTextProviderTest.TestSimple ();
      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      for (int counter = 1; counter < 20; ++counter)
      {
        toTextBuilderXml.sb ().e (() => counter).e (simpleTest).se ();
        simpleTest.Int += 13;
        simpleTest.Name += ".";
      }

      toTextBuilderXml.Close ();

      string result = stringWriter.ToString ();
      log.It (result);
      //Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }


    [Test]
    [Ignore]
    public void ToTextXmlLoopTest2 ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, true);

      toTextBuilderXml.Open ();

      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb ().e ("This is a sample specifc type handler - important members listed first:").e (() => t.Int).e (() => t.Name).e(() => t.Talk).se ());

      //var simpleTest = new ToTextProviderTest.TestSimple ();
      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      simpleTest.Talk = new TestSimpleToTextBuilderXmlTestOwned("Silverlight");
      simpleTest.Talk.Short = "Interesting stuff about silver lines and  moonlighting.";
      simpleTest.Talk.Participants = ListMother.New("Markus","Fabian","Heinz","Peter");
      for (int counter = 1; counter < 5; ++counter)
      {
        toTextBuilderXml.sb ().e (() => counter).e (simpleTest).se ();
        simpleTest.Int += 13;
        simpleTest.Name += ".";
      }

      toTextBuilderXml.Close ();

      string result = stringWriter.ToString ();
      log.It (result);
      //Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }


    [Test]
    public void ToTextXmlCharTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);

      toTextBuilderXml.Open ();
      toTextBuilderXml.e('x');
      toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<remotion>x</remotion>"));
    }


    [Test]
    public void ToTextXmlMultipleNewlinesTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.AllowNewline = true;

      toTextBuilderXml.Open ();
      toTextBuilderXml.nl (3);
      toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<br /><br /><br />"));
    }

    [Test]
    public void ToTextXmlAllowNewlinesTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);

      toTextBuilderXml.Open ();
      toTextBuilderXml.AllowNewline = false;
      toTextBuilderXml.e ("abc").nl ().e ("defg");
      toTextBuilderXml.AllowNewline = true;
      toTextBuilderXml.e ("hij").nl ().e ("klm").nl ().e ("op");
      toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("abcdefghij<br />klm<br />op"));
    }


    public void SequenceAllFilterLevelsFilteredOutput (ToTextBuilderXml toTextBuilderXml)
    {
      toTextBuilderXml.Open ();
      toTextBuilderXml.sb ().e ("before");
      toTextBuilderXml.sb ().e ("start");
      toTextBuilderXml.writeIfMediumOrHigher.e ("m").writeIfSkeletonOrHigher.e ("s");
      toTextBuilderXml.writeIfFull.sb ().e ("1").e ("2").e ("3").se ();
      toTextBuilderXml.writeIfBasicOrHigher.e ("b").writeIfComplexOrHigher.e ("c").writeIfFull.e ("f");
      toTextBuilderXml.e ("end").se ();
      toTextBuilderXml.e ("after").se ();
      toTextBuilderXml.Close ();
    }


    [Test]
    public void SequenceComplexityFilteringTest ()
    {
      {
        var stringWriter = new StringWriter ();
        var toTextBuilderXml = CreateTextBuilderXml (stringWriter,false);
        toTextBuilderXml.SetOutputComplexityToMedium ();
        SequenceAllFilterLevelsFilteredOutput (toTextBuilderXml);
        string result = stringWriter.ToString ();
        log.It (result);

        Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<remotion><seq><e>before</e><seq><e>start</e><e>m</e><e>s</e><e>b</e></seq></seq></remotion>"));
      }
    }


    [Test]
    [Ignore]
    public void PartialXmlTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);

      //toTextBuilderXml.Begin ();
      toTextBuilderXml.AllowNewline = false;
      //toTextBuilderXml.e ("first element");
      toTextBuilderXml.sb ().e ("seq_begin");
      toTextBuilderXml.sb ().e ("subseq_begin");
      toTextBuilderXml.writeIfBasicOrHigher.e ("subseq_element");
      toTextBuilderXml.e ("subseq_end").se ();
      toTextBuilderXml.e ("seq_end").se ();
      toTextBuilderXml.Flush();
      //toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<seq><e>before</e><seq><e>start</e><e>m</e><e>s</e><e>b</e></seq></seq>"));
    }


    [Test]
    public void WriteDictionaryTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      var dictionary = DictionaryMother.New ("a", 11, "b", 22, "C", 33);
      toTextBuilderXml.WriteDictionary (dictionary);
      toTextBuilderXml.Flush ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<dictionary name=\"Dictionary`2\" type=\"dictionary\"><de><key>a</key><val>11</val></de><de><key>b</key><val>22</val></de><de><key>C</key><val>33</val></de></dictionary>"));
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void CloseWithoutOpenExceptionTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Close();
    }

    [Test]
    public void OpenCloseTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Open ();
      toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<remotion />"));
    }

    [Test]
    public void DisposeTest ()
    {
      var stringWriter = new StringWriter ();
      using (var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false))
      {
        toTextBuilderXml.Open();
        toTextBuilderXml.s ("DisposeTest");
      }
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("<remotion>DisposeTest</remotion>"));
    }


    [Test]
    public void AppendRectangularArrayTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      string[,] array = { { "The", "Shadow" }, { "of", "the" } };
      toTextBuilderXml.WriteArray (array).Flush();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, Is.EqualTo ("<array><array><e>The</e><e>Shadow</e></array><array><e>of</e><e>the</e></array></array>"));
    }


    [Test]
    public void AppendRectangularArrayTest2 ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      int[, ,] array = { { { 1, 3 }, { 5, 7 } }, { { 11, 13 }, { 17, 19 } }, { { 23, 29 }, { 31, 37 } } };
      toTextBuilderXml.WriteArray (array).Flush();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, Is.EqualTo ("<array><array><array><e>1</e><e>3</e></array><array><e>5</e><e>7</e></array></array><array><array><e>11</e><e>13</e></array><array><e>17</e><e>19</e></array></array><array><array><e>23</e><e>29</e></array><array><e>31</e><e>37</e></array></array></array>"));
    }


    [Test]
    [ExpectedException (typeof (System.InvalidOperationException))]
    public void AlreadyClosedTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Open ();
      toTextBuilderXml.Close ();
      // ToTextBuilder is disposed => Write attempt throws
      toTextBuilderXml.s ("Sorry, we're alredy closed !");
    }


    [Test]
    [ExpectedException (typeof (System.InvalidOperationException))]
    public void DisposableTest ()
    {
      XmlWriterSettings settings = new XmlWriterSettings ();
      settings.OmitXmlDeclaration = true;
      settings.Indent = false;
      settings.NewLineOnAttributes = false;
      settings.ConformanceLevel = ConformanceLevel.Fragment;

      var stringWriter = new StringWriter ();
      var xmlWriter = XmlWriter.Create (stringWriter, settings);
      var toTextProvider = new ToTextProvider ();
      using (var toTextBuilderXml = new ToTextBuilderXml (toTextProvider, xmlWriter))
      {
        toTextBuilderXml.s ("Using ToTextBuilderXml");
      }
      Assert.That (stringWriter.ToString (), Is.EqualTo ("Using ToTextBuilderXml"));
      // XmlWriter should be closed when ToTextBuilder is disposed => Write attempt throws
      stringWriter.Write ("Sorry, we're alredy closed !");
    }


    [Test]
    public void IndentTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      //toTextBuilderXml.indent().nl().s ("line0").Flush ();
      toTextBuilderXml.Open ();
      toTextBuilderXml.indent ().nl ().s ("line0").unindent ().nl ().s ("line1").Flush ();
      toTextBuilderXml.Close ();
      string result = stringWriter.ToString ();
      //log.It (result);
      // Asert that indent has no effect
      Assert.That (result, Is.EqualTo ("<remotion><br />line0<br />line1</remotion>"));
    }


    public static ToTextProvider CreateTextProvider ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.Settings.UseAutomaticObjectToText = false;
      toTextProvider.Settings.UseAutomaticStringEnclosing = false;
      toTextProvider.Settings.UseAutomaticCharEnclosing = false;
      return toTextProvider;
    }


    public static ToTextBuilderXml CreateTextBuilderXml (TextWriter textWriter, bool indent)
    {
      XmlWriterSettings settings = new XmlWriterSettings ();

      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      settings.NewLineOnAttributes = false;
      //settings.ConformanceLevel = ConformanceLevel.Fragment;

      var xmlWriter = XmlWriter.Create (textWriter, settings);

      return new ToTextBuilderXml (CreateTextProvider(), xmlWriter);
    }



  }
}
