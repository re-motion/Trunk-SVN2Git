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
using System.Linq;
using System.Text;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  /// <summary>
  /// Base implementation class for <see cref="AclExpansionHtmlWriter"/> and <see cref="AclExpansionMultiFileHtmlWriter"/>.
  /// </summary>
  public class AclExpansionHtmlWriterImplementationBase
  {
    public static string ToValidFileName (string name)
    {
      var sb = new StringBuilder ();
      List<char> invalidFileNameCharsSortedList = Path.GetInvalidFileNameChars ().ToList ();
      invalidFileNameCharsSortedList.Sort ();
      foreach (char c in name)
      {
        if (invalidFileNameCharsSortedList.BinarySearch (c) >= 0)
        { 
          sb.Append ('_');
        }
        else
        {
          sb.Append (c);
        }
      }
      return sb.ToString ();
    }

    private readonly HtmlTagWriter _htmlTagWriter;
    private bool _isInTableRow;


    public AclExpansionHtmlWriterImplementationBase (TextWriter textWriter, bool indentXml)
    {
      _htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }


    public void WriteTableEnd ()
    {
      _htmlTagWriter.Tags.tableEnd ();
    }

    public virtual void WriteTableStart (string tableId)
    {
      _htmlTagWriter.Tags.table ().Attribute ("style", "width: 100%;").Attribute ("class", "aclExpansionTable").Attribute ("id", tableId);
    }


    public virtual HtmlTagWriter WritePageStart (string pageTitle)
    {
      _htmlTagWriter.WritePageHeader (pageTitle, "AclExpansion.css");
      _htmlTagWriter.Tag ("body");
      return _htmlTagWriter;
    }


    public virtual void WritePageEnd ()
    {
      _htmlTagWriter.TagEnd ("body");
      _htmlTagWriter.TagEnd ("html");

      _htmlTagWriter.Close ();
    }

    public virtual void WriteHeaderCell (string columnName)
    {
      _htmlTagWriter.Tags.th ().Attribute ("class", "header");
      _htmlTagWriter.Value (columnName);
      _htmlTagWriter.Tags.thEnd ();
    }

    public virtual void WriteTableData (string value)
    {
      WriteTableRowBeginIfNotInTableRow ();
      _htmlTagWriter.Tags.td ();
      _htmlTagWriter.Value (value);
      _htmlTagWriter.Tags.tdEnd ();
    }



    public virtual void WriteTableRowBeginIfNotInTableRow ()
    {
      if (!_isInTableRow)
      {
        _htmlTagWriter.Tags.tr ();
        _isInTableRow = true;
      }
    }

    public virtual void WriteTableRowEnd ()
    {
      _htmlTagWriter.Tags.trEnd ();
      _isInTableRow = false;
    }

    public HtmlTagWriter HtmlTagWriter
    {
      get { return _htmlTagWriter; }
    }
  }
}