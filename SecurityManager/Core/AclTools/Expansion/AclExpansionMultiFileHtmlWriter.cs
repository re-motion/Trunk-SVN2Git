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
using System.Linq;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="IAclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/> as a master HTML table consisting
  /// of users linking to detail HTML tables conaining the access rights of the respective user. All HTML files are written
  /// into an automatically generated directory.
  /// </summary>
  // TODO AE: Remove commented code. (Do not commit.)
  public class AclExpansionMultiFileHtmlWriter : AclExpansionHtmlWriterBase
  {
    public const string MasterFileName = "_AclExpansionMain_";

    private readonly ITextWriterFactory _textWriterFactory;
    private readonly bool _indentXml;
    private AclExpansionHtmlWriterSettings _detailHtmlWriterSettings = new AclExpansionHtmlWriterSettings();

    public AclExpansionMultiFileHtmlWriter (ITextWriterFactory textWriterFactory, bool indentXml)
    {
      _textWriterFactory = textWriterFactory;
      _indentXml = indentXml;
      //var textWriter = _textWriterFactory.NewTextWriter (MasterFileName);
      //htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }


    public AclExpansionHtmlWriterSettings DetailHtmlWriterSettings
    {
      get { return _detailHtmlWriterSettings; }
      set { _detailHtmlWriterSettings = value; }
    }


    //public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    //{
    //  ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
    //  WriteAclExpansion (aclExpansion);
    //}


    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      using (var textWriter = _textWriterFactory.NewTextWriter (MasterFileName))
      {
        htmlTagWriter = new HtmlTagWriter (textWriter, _indentXml);

        WritePageStart ("re-motion ACL Expansion - User Master Table");
        WriteTableStart ("remotion-user-table");
        WriteTableHeaders ();
        WriteTableBody (aclExpansion);
        WriteTableEnd ();
        WritePageEnd ();
      }
    }

    private void WriteTableHeaders ()
    {
      htmlTagWriter.Tags.tr (); // TODO AE: Consider using <TH>?
      WriteHeaderCell ("User");
      WriteHeaderCell ("First Name");
      WriteHeaderCell ("Last Name");
      WriteHeaderCell ("Access Rights");
      htmlTagWriter.Tags.trEnd ();
    }




    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var users = GetUsers (aclExpansion);

      foreach (var user in users)
      {
        WriteTableRowBeginIfNotInTableRow (); // TODO AE: Isn't it well-defined here if in a table row or not?
        WriteTableBody_ProcessUser (user, aclExpansion);
        WriteTableRowEnd ();
      }
    }

    // TODO AE: Rename to WriteUser or ProcessUser.
    private void WriteTableBody_ProcessUser (User user, List<AclExpansionEntry> aclExpansion)
    {
      WriteTableData (user.UserName);
      WriteTableData (user.FirstName);
      WriteTableData (user.LastName);

      string userDetailFileName = ToValidFileName (user.UserName); // TODO AE: Is UserName guaranteed to be unique regarding that forbidden characters are replaced by "_"?
      using (var detailTextWriter = _textWriterFactory.NewTextWriter (userDetailFileName))
      {

        var aclExpansionSingleUser = GetAccessControlEntriesForUser (aclExpansion, user);
        var detailAclExpansionHtmlWriter = new AclExpansionHtmlWriter (aclExpansionSingleUser, detailTextWriter, false);
        detailAclExpansionHtmlWriter.Settings = _detailHtmlWriterSettings;
        detailAclExpansionHtmlWriter.WriteAclExpansionAsHtml();
      }

      string relativePath = _textWriterFactory.GetRelativePath (MasterFileName, userDetailFileName);
      WriteTableRowBeginIfNotInTableRow (); // TODO AE: Isn't it well-defined here if in a table row or not?
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Tag ("a");
      htmlTagWriter.Attribute ("href", relativePath);
      htmlTagWriter.Attribute ("target", "_blank");
      htmlTagWriter.Value (relativePath);
      htmlTagWriter.TagEnd ("a");
      htmlTagWriter.Tags.tdEnd ();
    }


    public IEnumerable<User> GetUsers (IEnumerable<AclExpansionEntry> aclExpansion)
    {
      return (from aee in aclExpansion
             let user = aee.User
             orderby user.LastName, user.FirstName, user.UserName
             select user).Distinct();
    }

    // TODO AE: Is list required? (Asymmetric when compared to GetUsers.)
    public List<AclExpansionEntry> GetAccessControlEntriesForUser (IEnumerable<AclExpansionEntry> aclExpansion, User user)
    {
      return (from aee in aclExpansion
             where aee.User == user
             select aee).ToList();
    }
  }
}



