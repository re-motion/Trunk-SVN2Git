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
using System.IO;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Helper class for convenient writing of opening/closing HTML-tags to a <see cref="HtmlWriter"/>.
  /// <example>
  /// Example writing HTML to <see cref="StringWriter"/>
  /// <code><![CDATA[
  /// var textWriter = new StringWriter ();
  /// using (var htmlWriter = new HtmlWriter (textWriter, false))
  /// {
  ///   htmlWriter.Tags.head();
  /// }
  /// ]]></code>
  /// </example>
  /// </summary>
  public class HtmlWriterTagWriter
  {
    private readonly HtmlWriter _htmlWriter;

    public HtmlWriterTagWriter (HtmlWriter htmlWriter)
    {
      _htmlWriter = htmlWriter;
    }

    public HtmlWriter br ()
    {
      //_xmlWriter.WriteStartElement ("br");
      //_xmlWriter.WriteEndElement ();

      _htmlWriter.Tag ("br");
      _htmlWriter.TagEnd ("br");

      return _htmlWriter;
    }

    public HtmlWriter body ()
    {
      _htmlWriter.Tag ("body");
      return _htmlWriter;
    }

    public HtmlWriter bodyEnd ()
    {
      _htmlWriter.TagEnd ("body");
      return _htmlWriter;
    }

    public HtmlWriter table ()
    {
      _htmlWriter.Tag ("table");
      return _htmlWriter;
    }

    public HtmlWriter tableEnd ()
    {
      _htmlWriter.TagEnd ("table");
      return _htmlWriter;
    }

    public HtmlWriter tr ()
    {
      _htmlWriter.Tag ("tr");
      return _htmlWriter;
    }

    public HtmlWriter trEnd ()
    {
      _htmlWriter.TagEnd ("tr");
      return _htmlWriter;
    }

    public HtmlWriter td ()
    {
      _htmlWriter.Tag ("td");
      return _htmlWriter;
    }

    public HtmlWriter tdEnd ()
    {
      _htmlWriter.TagEnd ("td");
      return _htmlWriter;
    }

    public HtmlWriter th ()
    {
      _htmlWriter.Tag ("th");
      return _htmlWriter;
    }

    public HtmlWriter thEnd ()
    {
      _htmlWriter.TagEnd ("th");
      return _htmlWriter;
    }

    public HtmlWriter head ()
    {
      _htmlWriter.Tag ("head");
      return _htmlWriter;
    }

    public HtmlWriter headEnd ()
    {
      _htmlWriter.TagEnd ("head");
      return _htmlWriter;
    }

    public HtmlWriter title ()
    {
      _htmlWriter.Tag ("title");
      return _htmlWriter;
    }

    public HtmlWriter titleEnd ()
    {
      _htmlWriter.TagEnd ("title");
      return _htmlWriter;
    }

    public HtmlWriter html ()
    {
      _htmlWriter.Tag ("html");
      return _htmlWriter;
    }

    public HtmlWriter htmlEnd ()
    {
      _htmlWriter.TagEnd ("html");
      return _htmlWriter;
    }


    public HtmlWriter style ()
    {
      _htmlWriter.Tag ("style");
      return _htmlWriter;
    }

    public HtmlWriter styleEnd ()
    {
      _htmlWriter.TagEnd ("style");
      return _htmlWriter;
    }

    public HtmlWriter p ()
    {
      _htmlWriter.Tag ("p");
      return _htmlWriter;
    }

    public HtmlWriter pEnd ()
    {
      _htmlWriter.TagEnd ("p");
      return _htmlWriter;
    }

  }
}