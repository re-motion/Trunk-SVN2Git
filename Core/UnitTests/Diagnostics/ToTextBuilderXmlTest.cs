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

namespace Remotion.UnitTests.Diagnostics
{
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
      //XmlWriter xmlWriter = XmlWriter.Create(textWriter);
      XmlTextWriter xmlWriter = new XmlTextWriter (textWriter);
      return new ToTextBuilderXml (CreateTextProvider(), xmlWriter);
    }


  }
}