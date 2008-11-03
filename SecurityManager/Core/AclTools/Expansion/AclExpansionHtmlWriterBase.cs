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
      htmlWriter.tableEnd ();
    }

    protected void WriteTableStart (string tableId)
    {
      htmlWriter.table ().a ("style", "width: 100%;").a ("class", "aclExpansionTable").a ("id", tableId);
    }

    protected void WritePageEnd ()
    {
      htmlWriter.TagEnd ("body");
      htmlWriter.TagEnd ("html");

      htmlWriter.Close ();
    }

    protected void WriteHeaderCell (string columnName)
    {
      htmlWriter.th ().a ("class", "header");
      htmlWriter.Value (columnName);
      htmlWriter.thEnd ();
    }

    protected void WriteTableData (string value)
    {
      WriteTableRowBeginIfNotInTableRow ();
      htmlWriter.td ();
      htmlWriter.Value (value);
      htmlWriter.tdEnd ();
    }



    public void WriteTableRowBeginIfNotInTableRow ()
    {
      if (!_isInTableRow)
      {
        htmlWriter.tr ();
        _isInTableRow = true;
      }
    }

    public void WriteTableRowEnd ()
    {
      htmlWriter.trEnd ();
      _isInTableRow = false;
    }

    public abstract void WriteAclExpansion (List<AclExpansionEntry> aclExpansion);
  }
}