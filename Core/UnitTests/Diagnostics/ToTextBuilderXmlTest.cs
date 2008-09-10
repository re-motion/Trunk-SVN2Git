/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;

using List = Remotion.Development.UnitTesting.ObjectMother.List;


namespace Remotion.UnitTests.Diagnostics
{
  internal class TestSimpleToTextBuilderXmlTest
  {
    public TestSimpleToTextBuilderXmlTest ()
    {
      Name = "ABC abc";
      Int = 54321;
    }

    public TestSimpleToTextBuilderXmlTest (string name, int i)
    {
      Name = name;
      Int = i;
    }

    public string Name { get; set; }
    public int Int { get; set; }
    public TestSimpleToTextBuilderXmlTestOwned Talk { get; set; }

    //public override string ToString ()
    //{
    //  return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
    //}
  }

  internal class TestSimpleToTextBuilderXmlTestOwned
  {
    public TestSimpleToTextBuilderXmlTestOwned (string name)
    {
      Name = name;
    }

    public string Name { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
    public System.Collections.Generic.List<String> Participants { get; set; }

    //public override string ToString ()
    //{
    //  return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
    //}
  }
  
  [TestFixture]
  public class ToTextBuilderXmlTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (true);

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
      toTextBuilderXml.Begin();
      toTextBuilderXml.s (toXmlInputString);
      toTextBuilderXml.End ();
      var result = stringWriter.ToString();
      log.It ("xml=" + result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains (toXmlresultString));
    }

    [Test]
    public void ToTextXmlTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml =  CreateTextBuilderXml(stringWriter, false);
      toTextBuilderXml.Begin ();
      toTextBuilderXml.e (toXmlInputString);
      toTextBuilderXml.End ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains (toXmlresultString));
    }

    [Test]
    public void ToTextXmlSequenceTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Begin ();
      toTextBuilderXml.sb ().e (toXmlInputString).se ();
      toTextBuilderXml.End ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, Is.EqualTo ("<remotion><seq><e>" + toXmlresultString  + "</e></seq></remotion>"));
    }


    [Test]
    public void ToTextXmlVarTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);
      toTextBuilderXml.Begin ();

      var x = 123.456;
      toTextBuilderXml.sb ().e(y => x).se ();
      toTextBuilderXml.End ();
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
      toTextBuilderXml.Begin ();
      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb().e ("This is a sample specifc type handler - important members listed first:").e (x => t.Int).e (x => t.Name).se ());

      //var simpleTest = new ToTextProviderTest.TestSimple ();
      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      var aNumber = 123.456;
      toTextBuilderXml.sb ().e (y => aNumber).e("ABC").e(simpleTest).se ();
      toTextBuilderXml.End ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      //Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }


    [Test]
    public void ToTextXmlBeginEndTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter, false);

      toTextBuilderXml.Begin ();
      toTextBuilderXml.End ();
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

      toTextBuilderXml.Begin();

      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb ().e ("This is a sample specifc type handler - important members listed first:").e (x => t.Int).e (x => t.Name).se ());

      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      int counter = 1;
      toTextBuilderXml.sb ().e (x => counter).e (simpleTest).se ();
      toTextBuilderXml.sb ().e (x => counter).e (simpleTest).se ();
      toTextBuilderXml.End ();
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

      toTextBuilderXml.Begin ();

      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb ().e ("This is a sample specifc type handler - important members listed first:").e (x => t.Int).e (x => t.Name).se ());

      //var simpleTest = new ToTextProviderTest.TestSimple ();
      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      for (int counter = 1; counter < 20; ++counter)
      {
        toTextBuilderXml.sb ().e (x => counter).e (simpleTest).se ();
        simpleTest.Int += 13;
        simpleTest.Name += ".";
      }

      toTextBuilderXml.End ();

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

      toTextBuilderXml.Begin ();

      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb ().e ("This is a sample specifc type handler - important members listed first:").e (x => t.Int).e (x => t.Name).e(x => t.Talk).se ());

      //var simpleTest = new ToTextProviderTest.TestSimple ();
      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      simpleTest.Talk = new TestSimpleToTextBuilderXmlTestOwned("Silverlight");
      simpleTest.Talk.Short = "Interesting stuff about silver lines and  moonlighting.";
      simpleTest.Talk.Participants = List.New("Markus","Fabian","Heinz","Peter");
      for (int counter = 1; counter < 5; ++counter)
      {
        toTextBuilderXml.sb ().e (x => counter).e (simpleTest).se ();
        simpleTest.Int += 13;
        simpleTest.Name += ".";
      }

      toTextBuilderXml.End ();

      string result = stringWriter.ToString ();
      log.It (result);
      //Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }




    //[ToTextSpecificHandler]
    //class TestSimpleToTextBuilderXmlTestToTextSpecificTypeHandler : ToTextSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>
    //{
    //  public override void ToText (TestSimpleToTextBuilderXmlTest t, IToTextBuilderBase toTextBuilder)
    //  {
    //    toTextBuilder.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ();
    //  }
    //}

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