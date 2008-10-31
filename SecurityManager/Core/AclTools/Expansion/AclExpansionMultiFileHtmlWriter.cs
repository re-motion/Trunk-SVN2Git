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
using System.Xml;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Write a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/> as a master HTML table containing
  /// only the users linking to detail HTML tables conatining the access rights of the respective user
  /// to the given directory.
  /// </summary>
  public class AclExpansionMultiFileHtmlWriter : AclExpansionWriter
  {
    //private const string _masterFileName = "AclExpansionMain.html";
    private const string _masterFileName = "AclExpansionMain";

    private readonly HtmlWriter _htmlWriter;
    private readonly ITextWriterFactory _textWriterFactory;
    private bool _isInTableRow;
    //private readonly AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();

    //public AclExpansionMultiFileHtmlWriter (TextWriter textWriter, bool indentXml)
    //{
    //  _htmlWriter = new HtmlWriter (textWriter, indentXml);
    //}

    public AclExpansionMultiFileHtmlWriter (ITextWriterFactory textWriterFactory, bool indentXml)
    {
      _textWriterFactory = textWriterFactory;
      var textWriter = _textWriterFactory.NewTextWriter (_masterFileName);
      _htmlWriter = new HtmlWriter (textWriter, indentXml);
    }
   

    //public AclExpansionMultiFileHtmlWriter (XmlWriter xmlWriter)
    //{
    //  _htmlWriter = new HtmlWriter (xmlWriter);
    //}


    //public AclExpansionHtmlWriterSettings Settings
    //{
    //  get { return _settings; }
    //}


    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      WriteAclExpansionAsHtml (aclExpansion);
    }


    public void WriteAclExpansionAsHtml (List<AclExpansionEntry> aclExpansion)
    {
      WriteStartPage ();

      //html.value ("re-motion ACL Expansion body");

      WriteTableStart ();
      WriteTableHeaders ();
      WriteTableBody (aclExpansion);
      WriteTableEnd ();

      WriteEndPage ();
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

    private void WriteEndPage ()
    {
      _htmlWriter.TagEnd ("body");
      _htmlWriter.TagEnd ("html");

      _htmlWriter.Close ();
    }

    private HtmlWriter WriteStartPage ()
    {
      //// DOCTYPE
      //_htmlWriter.XmlWriter.WriteDocType ("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
      //// HTML
      //_htmlWriter.Tag ("html");
      //// HEAD
      //_htmlWriter.Tag ("head");
      //// TITLE
      //_htmlWriter.Tag ("title");
      //_htmlWriter.Value ("re-motion ACL Expansion - User Master Table");
      //_htmlWriter.TagEnd ("title");

      //// STYLE
      //_htmlWriter.Tag ("style");
      //_htmlWriter.Value ("@import \"AclExpansion.css\";");
      //_htmlWriter.TagEnd ("style");
      //_htmlWriter.TagEnd ("head");

      _htmlWriter.WritePageHeader ("re-motion ACL Expansion - User Master Table", "AclExpansion.css");

      // BODY
      _htmlWriter.Tag ("body");
      return _htmlWriter;
    }


    void WriteHeaderCell (string columnName)
    {
      _htmlWriter.th ().a ("class", "header");
      _htmlWriter.Value (columnName);
      _htmlWriter.thEnd ();
    }

    //void WriteTableDataAddendum (Object addendum)
    //{
    //  if (addendum != null)
    //  {
    //    _htmlWriter.Value (" (");
    //    _htmlWriter.Value (addendum);
    //    _htmlWriter.Value (") ");
    //  }
    //}


    void WriteTableData (string value)
    {
      WriteTableRowBeginIfNotInTableRow ();
      _htmlWriter.td ();
      _htmlWriter.Value (value);
      _htmlWriter.tdEnd ();
    }


    public void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var users = GetUsers (aclExpansion);

      foreach (var user in users)
      {
        WriteTableRowBeginIfNotInTableRow ();
        WriteTableBody_ProcessUser (user, aclExpansion);
        WriteTableRowEnd ();
      }
    }

    public void WriteTableBody_ProcessUser (User user, List<AclExpansionEntry> aclExpansion)
    {
      WriteTableData (user.UserName);
      WriteTableData (user.FirstName);
      WriteTableData (user.LastName);
      //WriteTableData ("Permissions be here");

      string userDetailFileName = ToValidFileName (user.UserName); //+".html";
      var detailTextWriter = _textWriterFactory.NewTextWriter (userDetailFileName);
      //detailTextWriter.WriteLine("user display name = " + user.DisplayName);

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
        //if (invalidFileNameCharsSortedList.FindIndex (x => (x == c)) >= 0)
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



