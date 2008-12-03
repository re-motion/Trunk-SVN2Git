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
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
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
  public class AclExpansionMultiFileHtmlWriter : IAclExpansionWriter
  {
    public const string MasterFileName = "_AclExpansionMain_";

    private readonly ITextWriterFactory _textWriterFactory;
    private readonly bool _indentXml;
    private AclExpansionHtmlWriterSettings _detailHtmlWriterSettings = new AclExpansionHtmlWriterSettings();
    private AclExpansionHtmlWriterImplementationBase _implementation;

    public AclExpansionMultiFileHtmlWriter (ITextWriterFactory textWriterFactory, bool indentXml)
    {
      _textWriterFactory = textWriterFactory;
      _indentXml = indentXml;
    }


    public AclExpansionHtmlWriterSettings DetailHtmlWriterSettings
    {
      get { return _detailHtmlWriterSettings; }
      set { _detailHtmlWriterSettings = value; }
    }

    
    public void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      using (var textWriter = _textWriterFactory.NewTextWriter (MasterFileName))
      {
        _implementation = new AclExpansionHtmlWriterImplementationBase (textWriter, _indentXml);

        _implementation.WritePageStart ("re-motion ACL Expansion - User Master Table");
        _implementation.WriteTableStart ("remotion-user-table");
        WriteTableHeaders ();
        WriteTableBody (aclExpansion);
        _implementation.WriteTableEnd ();
        _implementation.WritePageEnd ();
      }
    }

    private void WriteTableHeaders ()
    {
      _implementation.HtmlTagWriter.Tags.tr (); // TODO AE: Consider using <TH>?
      _implementation.WriteHeaderCell ("User");
      _implementation.WriteHeaderCell ("First Name");
      _implementation.WriteHeaderCell ("Last Name");
      _implementation.WriteHeaderCell ("Access Rights");
      _implementation.HtmlTagWriter.Tags.trEnd ();
    }




    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var users = GetUsers (aclExpansion);

      foreach (var user in users)
      {
        _implementation.WriteTableRowBeginIfNotInTableRow (); // TODO QAE: Isn't it well-defined here if in a table row or not?; MGi: No, due to rowspan
        WriteTableBody_ProcessUser (user, aclExpansion);
        _implementation.WriteTableRowEnd ();
      }
    }

    // TODO AE: Rename to WriteUser or ProcessUser.
    private void WriteTableBody_ProcessUser (User user, List<AclExpansionEntry> aclExpansion)
    {
      _implementation.WriteTableData (user.UserName);
      _implementation.WriteTableData (user.FirstName);
      _implementation.WriteTableData (user.LastName);

      string userDetailFileName = AclExpansionHtmlWriterImplementationBase.ToValidFileName (user.UserName); // TODO AE: Is UserName guaranteed to be unique regarding that forbidden characters are replaced by "_"?
      using (var detailTextWriter = _textWriterFactory.NewTextWriter (userDetailFileName))
      {

        var aclExpansionSingleUser = GetAccessControlEntriesForUser (aclExpansion, user);
        var detailAclExpansionHtmlWriter = new AclExpansionHtmlWriter (detailTextWriter, false, _detailHtmlWriterSettings);
        detailAclExpansionHtmlWriter.WriteAclExpansion (aclExpansionSingleUser);
      }

      string relativePath = _textWriterFactory.GetRelativePath (MasterFileName, userDetailFileName);
      _implementation.WriteTableRowBeginIfNotInTableRow (); 
      _implementation.HtmlTagWriter.Tags.td ();
      _implementation.HtmlTagWriter.Tag ("a");
      _implementation.HtmlTagWriter.Attribute ("href", relativePath);
      _implementation.HtmlTagWriter.Attribute ("target", "_blank");
      _implementation.HtmlTagWriter.Value (relativePath);
      _implementation.HtmlTagWriter.TagEnd ("a");
      _implementation.HtmlTagWriter.Tags.tdEnd ();
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



