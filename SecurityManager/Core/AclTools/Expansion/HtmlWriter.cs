// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  // Spike
  // TODO: Write tests
  public class HtmlWriter : IDisposable
  {
    private readonly XmlWriter _xmlWriter;
    private readonly Stack<string> _openElementStack = new Stack<string>();


    public HtmlWriter (TextWriter textWriter, bool indentXml)
    {
      _xmlWriter = CreateXmlWriter (textWriter, indentXml);
    }

    public HtmlWriter (XmlWriter xmlWriter)
    {
      _xmlWriter = xmlWriter;
    }

    public XmlWriter XmlWriter
    {
      get { return _xmlWriter; }
    }

    public HtmlWriter Tag (string elementName)
    {
      _xmlWriter.WriteStartElement (elementName);
      _openElementStack.Push (elementName);
      return this;
    }

    public HtmlWriter TagEnd (string elementName)
    {
      string ElementNameExpected = _openElementStack.Pop();
      if (ElementNameExpected != elementName)
      {
        throw new XmlException (String.Format ("Wrong closing tag in XML: Expected {0} but was {1}:\n{2}", ElementNameExpected, elementName, _xmlWriter.ToString()));
      }
      _xmlWriter.WriteEndElement ();
      return this;
    }

    public HtmlWriter a (string attributeName, string attributeValue)
    {
      _xmlWriter.WriteAttributeString (attributeName,attributeValue);
      return this;
    }

    public HtmlWriter table ()
    {
      Tag ("table");
      return this;
    }

    public HtmlWriter tableEnd ()
    {
      TagEnd ("table");
      return this;
    }

    public HtmlWriter tr ()
    {
      Tag ("tr");
      return this;
    }

    public HtmlWriter trEnd ()
    {
      TagEnd ("tr");
      return this;
    }

    public HtmlWriter td ()
    {
      Tag ("td");
      return this;
    }

    public HtmlWriter tdEnd ()
    {
      TagEnd ("td");
      return this;
    }

    public XmlWriter CreateXmlWriter (TextWriter textWriter, bool indent)
    {
      XmlWriterSettings settings = new XmlWriterSettings ();

      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      settings.NewLineOnAttributes = false;
      //settings.ConformanceLevel = ConformanceLevel.Fragment;

      return XmlWriter.Create (textWriter, settings);
    }

    public HtmlWriter Value (string s)
    {
      _xmlWriter.WriteValue(s);
      return this;
    }

    public HtmlWriter Value (object obj)
    {
      _xmlWriter.WriteValue (obj);
      return this;
    }

    public HtmlWriter br ()
    {
      _xmlWriter.WriteStartElement ("br");
      _xmlWriter.WriteEndElement ();
      return this;
    }


    //------------------------------------------------------------
    // Dispose
    //------------------------------------------------------------
    
    public void Close ()
    {
      Dispose ();
    }

    public void Dispose ()
    {
      _xmlWriter.Close ();
    }
  }
}