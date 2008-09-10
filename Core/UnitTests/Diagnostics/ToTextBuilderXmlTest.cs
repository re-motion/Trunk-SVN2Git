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
using Remotion.Utilities;

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
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter);
      toTextBuilderXml.s (toXmlInputString);
      toTextBuilderXml.Flush ();
      var result = stringWriter.ToString();
      log.It ("xml=" + result);
      Assert.That (result, Is.EqualTo (toXmlresultString));
    }

    [Test]
    public void ToTextXmlTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml =  CreateTextBuilderXml(stringWriter);
      toTextBuilderXml.e (toXmlInputString);
      toTextBuilderXml.Flush ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, Is.EqualTo ("<e>"+  toXmlresultString + "</e>"));
    }

    [Test]
    public void ToTextXmlSequenceTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter);
      toTextBuilderXml.sb().e (toXmlInputString).se();
      toTextBuilderXml.Flush ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, Is.EqualTo ("<e><seq><e>" + toXmlresultString + "</e></seq></e>"));
    }


    [Test]
    public void ToTextXmlSvTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter);

      var x = 123.456;
      toTextBuilderXml.sb ().e(y => x).se ();
      toTextBuilderXml.Flush ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }


    [Test]
    [Ignore]
    public void ToTextXmlInstanceTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter);
      var toTextProvider = toTextBuilderXml.ToTextProvider;
      toTextProvider.Settings.UseAutomaticObjectToText = true;
      //toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>((t,tb) => tb.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ());
      toTextProvider.RegisterSpecificTypeHandler<TestSimpleToTextBuilderXmlTest> ((t, tb) => tb.sb().e ("This is a sample specifc type handler - important members listed first:").e (x => t.Int).e (x => t.Name).se ());

      //var simpleTest = new ToTextProviderTest.TestSimple ();
      var simpleTest = new TestSimpleToTextBuilderXmlTest ("ToTextXmlInstanceTest", 333);
      var aNumber = 123.456;
      toTextBuilderXml.sb ().e (y => aNumber).e("ABC").e(simpleTest).se ();
      toTextBuilderXml.Flush ();
      var result = stringWriter.ToString ();
      log.It ("xml=" + result);
      //Assert.That (result, Is.EqualTo ("<e><seq><var name=\"x\"><e>123.456</e></var></seq></e>"));
    }


    [Test]
    public void ToTextXmlBeginEndTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter);

      toTextBuilderXml.Begin ();
      toTextBuilderXml.End ();
      string result = stringWriter.ToString ();
      log.It (result);
      Assert.That (result, Is.EqualTo ("<remotion />"));
    }


    [Test]
    public void ToTextXmlMultiTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter);

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
    public void ToTextXmlLoopTest ()
    {
      var stringWriter = new StringWriter ();
      var toTextBuilderXml = CreateTextBuilderXml (stringWriter);

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



    [ToTextSpecificHandler]
    class TestSimpleToTextBuilderXmlTestToTextSpecificTypeHandler : ToTextSpecificTypeHandler<TestSimpleToTextBuilderXmlTest>
    {
      public override void ToText (TestSimpleToTextBuilderXmlTest t, IToTextBuilderBase toTextBuilder)
      {
        toTextBuilder.sbLiteral ("[", ",", "]").e ("TestSimple").e (t.Name).e (t.Int).se ();
      }
    }

    public static ToTextProvider CreateTextProvider ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.Settings.UseAutomaticObjectToText = false;
      toTextProvider.Settings.UseAutomaticStringEnclosing = false;
      toTextProvider.Settings.UseAutomaticCharEnclosing = false;
      return toTextProvider;
    }


    public static ToTextBuilderXml CreateTextBuilderXml (TextWriter textWriter)
    {
      XmlWriterSettings settings = new XmlWriterSettings ();
      settings.Indent = true;
      settings.OmitXmlDeclaration = true;
      settings.NewLineOnAttributes = false;
      var xmlWriter = XmlWriter.Create (textWriter, settings);

      return new ToTextBuilderXml (CreateTextProvider(), xmlWriter);
    }


  }
}