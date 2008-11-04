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


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="IAclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/> as a master HTML table containing
  /// of users linking to detail HTML tables conaining the access rights of the respective user. All HTML files are written
  /// into an automatically generated directory.
  /// </summary>
  public class AclExpansionMultiFileHtmlWriter : AclExpansionHtmlWriterBase
  {
    private const string _masterFileName = "AclExpansionMain";

    private readonly ITextWriterFactory _textWriterFactory;

    public AclExpansionMultiFileHtmlWriter (ITextWriterFactory textWriterFactory, bool indentXml)
    {
      _textWriterFactory = textWriterFactory;
      var textWriter = _textWriterFactory.NewTextWriter (_masterFileName);
      htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }
   

    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      WriteAclExpansionAsHtml (aclExpansion);
    }


    public void WriteAclExpansionAsHtml (List<AclExpansionEntry> aclExpansion)
    {
      WritePageStart ("re-motion ACL Expansion - User Master Table");

      WriteTableStart ("remotion-user-table");
      WriteTableHeaders ();
      WriteTableBody (aclExpansion);
      WriteTableEnd ();

      WritePageEnd ();
    }

    private void WriteTableHeaders ()
    {
      htmlTagWriter.Tags.tr ();
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

    public List<AclExpansionEntry> GetAccessControlEntriesForUser (IEnumerable<AclExpansionEntry> aclExpansion, User user)
    {
      return (from aee in aclExpansion
             where aee.User == user
             select aee).ToList();
    }
  }
}



