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

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Abstract base class for <see cref="IAclExpansionWriter"/>|s which write HTML format.
  /// </summary>
  public abstract class AclExpansionHtmlWriterBase : IAclExpansionWriter
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

    protected HtmlWriter htmlWriter;
    private bool _isInTableRow;


    protected void WriteTableEnd ()
    {
      htmlWriter.Tags.tableEnd ();
    }

    protected virtual void WriteTableStart (string tableId)
    {
      htmlWriter.Tags.table ().a ("style", "width: 100%;").a ("class", "aclExpansionTable").a ("id", tableId);
    }


    protected virtual HtmlWriter WritePageStart (string pageTitle)
    {
      htmlWriter.WritePageHeader (pageTitle, "AclExpansion.css");

      // BODY
      htmlWriter.Tag ("body");
      return htmlWriter;
    }


    protected virtual void WritePageEnd ()
    {
      htmlWriter.TagEnd ("body");
      htmlWriter.TagEnd ("html");

      htmlWriter.Close ();
    }

    protected virtual void WriteHeaderCell (string columnName)
    {
      htmlWriter.Tags.th ().a ("class", "header");
      htmlWriter.Value (columnName);
      htmlWriter.Tags.thEnd ();
    }

    protected virtual void WriteTableData (string value)
    {
      WriteTableRowBeginIfNotInTableRow ();
      htmlWriter.Tags.td ();
      htmlWriter.Value (value);
      htmlWriter.Tags.tdEnd ();
    }



    public virtual void WriteTableRowBeginIfNotInTableRow ()
    {
      if (!_isInTableRow)
      {
        htmlWriter.Tags.tr ();
        _isInTableRow = true;
      }
    }

    public virtual void WriteTableRowEnd ()
    {
      htmlWriter.Tags.trEnd ();
      _isInTableRow = false;
    }

    public abstract void WriteAclExpansion (List<AclExpansionEntry> aclExpansion);
  }
}