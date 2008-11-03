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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="IAclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/> as a master HTML table containing
  /// of users linking to detail HTML tables conaining the access rights of the respective user. All HTML files are written
  /// into an automatically generated directory.
  /// </summary>
  public class AclExpansionMultiFileHtmlWriter : IAclExpansionWriter
  {
    private const string _masterFileName = "AclExpansionMain";

    private readonly HtmlWriter _htmlWriter;
    private readonly ITextWriterFactory _textWriterFactory;
    private bool _isInTableRow;

    public AclExpansionMultiFileHtmlWriter (ITextWriterFactory textWriterFactory, bool indentXml)
    {
      _textWriterFactory = textWriterFactory;
      var textWriter = _textWriterFactory.NewTextWriter (_masterFileName);
      _htmlWriter = new HtmlWriter (textWriter, indentXml);
    }
   

    public void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      WriteAclExpansionAsHtml (aclExpansion);
    }


    public void WriteAclExpansionAsHtml (List<AclExpansionEntry> aclExpansion)
    {
      WritePageStart ();

      WriteTableStart ();
      WriteTableHeaders ();
      WriteTableBody (aclExpansion);
      WriteTableEnd ();

      WritePageEnd ();
    }

    private void WriteTableEnd ()
    {
      _htmlWriter.tableEnd ();
    }

    private void WriteTableStart ()
    {
      _htmlWriter.table ().a ("style", "width: 100%;").a ("class", "aclExpansionTable").a ("id", "remotion-user-table");
    }

    private void WriteTableHeaders ()
    {
      _htmlWriter.tr ();
      WriteHeaderCell ("User");
      WriteHeaderCell ("First Name");
      WriteHeaderCell ("Last Name");
      WriteHeaderCell ("Access Rights");
      _htmlWriter.trEnd ();
    }

    private void WritePageEnd ()
    {
      _htmlWriter.TagEnd ("body");
      _htmlWriter.TagEnd ("html");

      _htmlWriter.Close ();
    }

    private HtmlWriter WritePageStart ()
    {
      _htmlWriter.WritePageHeader ("re-motion ACL Expansion - User Master Table", "AclExpansion.css");

      // BODY
      _htmlWriter.Tag ("body");
      return _htmlWriter;
    }


    private void WriteHeaderCell (string columnName)
    {
      _htmlWriter.th ().a ("class", "header");
      _htmlWriter.Value (columnName);
      _htmlWriter.thEnd ();
    }


    private void WriteTableData (string value)
    {
      WriteTableRowBeginIfNotInTableRow ();
      _htmlWriter.td ();
      _htmlWriter.Value (value);
      _htmlWriter.tdEnd ();
    }


    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var users = GetUsers (aclExpansion);

      foreach (var user in users)
      {
        WriteTableRowBeginIfNotInTableRow ();
        WriteTableBody_ProcessUser (user, aclExpansion);
        WriteTableRowEnd ();
      }
    }

    private void WriteTableBody_ProcessUser (User user, List<AclExpansionEntry> aclExpansion)
    {
      WriteTableData (user.UserName);
      WriteTableData (user.FirstName);
      WriteTableData (user.LastName);

      string userDetailFileName = ToValidFileName (user.UserName); //+".html";
      var detailTextWriter = _textWriterFactory.NewTextWriter (userDetailFileName);

      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (detailTextWriter, false);
      var aclExpansionSingleUser = GetAccessControlEntriesForUser (aclExpansion, user);
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansionSingleUser);

      string relativePath = _textWriterFactory.GetRelativePath (_masterFileName, userDetailFileName);
      WriteTableRowBeginIfNotInTableRow ();
      _htmlWriter.td ();
      _htmlWriter.Tag ("a");
      _htmlWriter.a ("href", relativePath);
      _htmlWriter.a ("target", "_blank");
      _htmlWriter.Value (relativePath);
      _htmlWriter.TagEnd ("a");
      _htmlWriter.tdEnd ();
    }

    public static string ToValidFileName (string name)
    {
      var sb = new StringBuilder();
      List<char> invalidFileNameCharsSortedList = Path.GetInvalidFileNameChars ().ToList ();
      invalidFileNameCharsSortedList.Sort();
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
      return sb.ToString();
    }


    public void WriteTableRowBeginIfNotInTableRow ()
    {
      if (!_isInTableRow)
      {
        _htmlWriter.tr ();
        _isInTableRow = true;
      }
    }

    public void WriteTableRowEnd ()
    {
      _htmlWriter.trEnd ();
      _isInTableRow = false;
    }


    public IEnumerable<User> GetUsers (IEnumerable<AclExpansionEntry> aclExpansion)
    {
      return (from aee in aclExpansion
             let user = aee.User
             orderby user.LastName, user.FirstName, user.UserName
             select user).Distinct();
    }

    public List<AclExpansionEntry> GetAccessControlEntriesForUser (IEnumerable<AclExpansionEntry> aclExpansion, User user)
    {
      return (from aee in aclExpansion
             where aee.User == user
             select aee).ToList();
    }
  }
}



