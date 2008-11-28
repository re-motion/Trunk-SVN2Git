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
        { // TODO AE: Braces
          sb.Append ('_');
        }
        else
        {// TODO AE: Braces
          sb.Append (c);
        }
      }
      return sb.ToString ();
    }

    protected HtmlTagWriter htmlTagWriter;
    private bool _isInTableRow;


    protected void WriteTableEnd ()
    {
      htmlTagWriter.Tags.tableEnd ();
    }

    protected virtual void WriteTableStart (string tableId)
    {
      htmlTagWriter.Tags.table ().Attribute ("style", "width: 100%;").Attribute ("class", "aclExpansionTable").Attribute ("id", tableId);
    }


    protected virtual HtmlTagWriter WritePageStart (string pageTitle)
    {
      htmlTagWriter.WritePageHeader (pageTitle, "AclExpansion.css");

      // BODY // TODO AE: Required?
      htmlTagWriter.Tag ("body");
      return htmlTagWriter;
    }


    protected virtual void WritePageEnd ()
    {
      htmlTagWriter.TagEnd ("body");
      htmlTagWriter.TagEnd ("html");

      htmlTagWriter.Close ();
    }

    protected virtual void WriteHeaderCell (string columnName)
    {
      htmlTagWriter.Tags.th ().Attribute ("class", "header");
      htmlTagWriter.Value (columnName);
      htmlTagWriter.Tags.thEnd ();
    }

    protected virtual void WriteTableData (string value)
    {
      WriteTableRowBeginIfNotInTableRow ();
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (value);
      htmlTagWriter.Tags.tdEnd ();
    }



    public virtual void WriteTableRowBeginIfNotInTableRow ()
    {
      if (!_isInTableRow)
      {
        htmlTagWriter.Tags.tr ();
        _isInTableRow = true;
      }
    }

    public virtual void WriteTableRowEnd ()
    {
      htmlTagWriter.Tags.trEnd ();
      _isInTableRow = false;
    }

    public abstract void WriteAclExpansion (List<AclExpansionEntry> aclExpansion); // TODO AE: Remove abstract method, it is not used from base class variables.
  }
}