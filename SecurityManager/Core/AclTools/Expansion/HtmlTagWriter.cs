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
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class HtmlTagWriter : IDisposable
  {
    private readonly XmlWriter _xmlWriter;
    private readonly Stack<string> _openElementStack = new Stack<string>();

    private readonly HtmlTagWriterTags _htmlTagWriterTags;


    public HtmlTagWriter (TextWriter textWriter, bool indentXml)
      : this (CreateXmlWriter (textWriter, indentXml))
    {}

    public HtmlTagWriter (XmlWriter xmlWriter)
    {
      _xmlWriter = xmlWriter;
      _htmlTagWriterTags = new HtmlTagWriterTags (this);
    }

    public XmlWriter XmlWriter
    {
      get { return _xmlWriter; }
    }

    public HtmlTagWriterTags Tags
    {
      get { return _htmlTagWriterTags; }
    }

    public HtmlTagWriter Tag (string elementName)
    {
      _xmlWriter.WriteStartElement (elementName);
      _openElementStack.Push (elementName);
      return this;
    }

    public HtmlTagWriter TagEnd (string elementName)
    {
      string ElementNameExpected = _openElementStack.Pop();
      if (ElementNameExpected != elementName)
      {
        throw new XmlException (String.Format ("Wrong closing tag in HTML: Expected {0} but was {1}:\n{2}", ElementNameExpected, elementName, _xmlWriter.ToString()));
      }
      _xmlWriter.WriteEndElement ();
      return this;
    }

    public HtmlTagWriter Attribute (string attributeName, string attributeValue)
    {
      _xmlWriter.WriteAttributeString (attributeName,attributeValue);
      return this;
    }

    public static XmlWriter CreateXmlWriter (TextWriter textWriter, bool indent)
    {
      XmlWriterSettings settings = new XmlWriterSettings ();

      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      settings.NewLineOnAttributes = false;
      //settings.ConformanceLevel = ConformanceLevel.Fragment;

      return XmlWriter.Create (textWriter, settings);
    }

    public HtmlTagWriter Value (string s)
    {
      //_xmlWriter.WriteValue(s);
      _xmlWriter.WriteValue (StringUtility.NullToEmpty(s));
      return this;
    }

    public HtmlTagWriter Value (object obj)
    {
      _xmlWriter.WriteValue (obj);
      return this;
    }




    public HtmlTagWriter WritePageHeader (string pageTitle, string cssFileName)
    {
      // DOCTYPE
      XmlWriter.WriteDocType ("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
      // HTML
      Tag ("html");
      // HEAD
      Tag ("head");
      // TITLE
      if (pageTitle != null)
      {
        Tag ("title");
        Value (pageTitle);
        TagEnd ("title");
      }

      // STYLE
      if (cssFileName != null)
      {
        Tag ("style");
        Value ("@import \"" + cssFileName + "\";");
        TagEnd ("style");
        TagEnd ("head");
      }

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