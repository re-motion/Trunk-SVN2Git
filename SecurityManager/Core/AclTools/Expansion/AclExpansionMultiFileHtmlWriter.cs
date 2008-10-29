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
using System.Xml;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Write a <see cref="List{T}"/>&lt;<see cref="AclExpansionEntry"/>&gt; as a master HTML table containing
  /// only the users linking to detail HTML tables conatining the access rights of the respective user
  /// to the given directory.
  /// </summary>
  public class AclExpansionMultiFileHtmlWriter : AclExpansionWriter
  {
    private readonly HtmlWriter _htmlWriter;
    private bool _isInTableRow;
    private readonly AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();

    public AclExpansionMultiFileHtmlWriter (TextWriter textWriter, bool indentXml)
    {
      _htmlWriter = new HtmlWriter (textWriter, indentXml);
    }

    public AclExpansionMultiFileHtmlWriter (XmlWriter xmlWriter)
    {
      _htmlWriter = new HtmlWriter (xmlWriter);
    }


    public AclExpansionHtmlWriterSettings Settings
    {
      get { return _settings; }
    }


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
      _htmlWriter.table ().a ("style", "width: 100%;").a ("class", "aclExpansionTable").a ("id", "remotion-ACL-expansion-table");
    }

    private void WriteTableHeaders ()
    {
      _htmlWriter.tr ();
      WriteHeaderCell ("User");
      WriteHeaderCell ("Role");
      WriteHeaderCell ("Class");
      WriteHeaderCell ("States");
      WriteHeaderCell ("User Must Own");
      WriteHeaderCell ("Group Must Own");
      WriteHeaderCell ("Tenant Must Own");
      WriteHeaderCell ("User Must Have Abstract Role");
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
      // DOCTYPE
      _htmlWriter.XmlWriter.WriteDocType ("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
      // HTML
      _htmlWriter.Tag ("html");
      // HEAD
      _htmlWriter.Tag ("head");
      // TITLE
      _htmlWriter.Tag ("title");
      _htmlWriter.Value ("re-motion ACL Expansion");
      _htmlWriter.TagEnd ("title");

      // STYLE
      _htmlWriter.Tag ("style");
      _htmlWriter.Value ("@import \"AclExpansion.css\";");
      _htmlWriter.TagEnd ("style");
      _htmlWriter.TagEnd ("head");

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

    void WriteTableDataAddendum (Object addendum)
    {
      if (addendum != null)
      {
        _htmlWriter.Value (" (");
        _htmlWriter.Value (addendum);
        _htmlWriter.Value (") ");
      }
    }


    void WriteTableDataWithRowCount (string value, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow ();
      _htmlWriter.td ();
      WriteRowspanAttribute(rowCount);
      _htmlWriter.Value (value);
      if (Settings.OutputRowCount)
      {
        WriteTableDataAddendum (rowCount);
      }
      _htmlWriter.tdEnd ();
    }



    private HtmlWriter WriteRowspanAttribute (int rowCount)
    {
      if (rowCount > 0)
      {
        _htmlWriter.a ("rowspan", Convert.ToString (rowCount));
      }
      return _htmlWriter;
    }

    public void WriteTableDataForRole (Role role, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow();
      _htmlWriter.td ();
      WriteRowspanAttribute (rowCount);
      _htmlWriter.Value (role.Group.DisplayName);
      _htmlWriter.Value (", ");
      _htmlWriter.Value (role.Position.DisplayName);
      if (Settings.OutputRowCount)
      {
        WriteTableDataAddendum (rowCount);
      }
      _htmlWriter.tdEnd ();
    }
  

    void tdBodyStates (AclExpansionEntry aclExpansionEntry) // params StateDefinition[] stateDefinitions)
    {
      //To.ConsoleLine.s ("tdBodyStates").e (aclExpansionEntry);
      var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).ToArray ();
      _htmlWriter.td ();
      bool firstElement = true;
      foreach (StateDefinition stateDefiniton in stateDefinitions)
      {
        if (!firstElement)
        {
          //_htmlWriter.br ();
          _htmlWriter.Value (", ");
        }

        string stateName = Settings.UseShortNames ? stateDefiniton.ShortName () : stateDefiniton.DisplayName;
        //To.ConsoleLine.e (() => stateName);

        _htmlWriter.Value (stateName);
        firstElement = false;
      }
      _htmlWriter.tdEnd ();
    }

    private void tdBodyAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      _htmlWriter.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitions)
      {
        if (!firstElement)
        {
          _htmlWriter.Value (", ");
        }
        _htmlWriter.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      _htmlWriter.tdEnd ();
    }

    private void tdBodyConditions (AclExpansionAccessConditions conditions)
    {
      tdBodyBooleanCondition (conditions.IsOwningUserRequired);
      tdBodyBooleanCondition (conditions.IsOwningGroupRequired);
      tdBodyBooleanCondition (conditions.IsOwningTenantRequired);

      _htmlWriter.td ();
      _htmlWriter.Value (conditions.IsAbstractRoleRequired ? conditions.AbstractRole.DisplayName : "");
      _htmlWriter.tdEnd ();
    }

    private void tdBodyBooleanCondition (bool required)
    {
      _htmlWriter.td ();
      _htmlWriter.Value (required ? "X" : "");
      _htmlWriter.tdEnd ();
    }


    public void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var aclExpansionUserGrouping = GetAclExpansionGrouping (aclExpansion, (aee => aee.User));

      foreach (var userGroup in aclExpansionUserGrouping)
      {
        WriteTableBody_ProcessUserGroup(userGroup);
      }
    }

    public void WriteTableBody_ProcessUserGroup (IGrouping<User, AclExpansionEntry> userGroup)
    {
      WriteTableDataWithRowCount (userGroup.Key.DisplayName, userGroup.Count ());
  
      var aclExpansionRoleGrouping = GetAclExpansionGrouping (userGroup, (x => x.Role));

      foreach (var roleGroup in aclExpansionRoleGrouping)
      {
        WriteTableBody_ProcessRoleGroup(roleGroup);
      }
    }

    public void WriteTableBody_ProcessRoleGroup (IGrouping<Role, AclExpansionEntry> roleGroup)
    {
      WriteTableDataForRole (roleGroup.Key, roleGroup.Count ());
 
      var aclExpansionClassGrouping = GetAclExpansionGrouping (roleGroup, (x => x.Class));

      foreach (var classGroup in aclExpansionClassGrouping)
      {
        WriteTableBody_ProcessClassGroup(classGroup);
      }
    }

    public void WriteTableBody_ProcessClassGroup (IGrouping<SecurableClassDefinition, AclExpansionEntry> classGroup)
    {
      if (classGroup.Key != null)
      {
        string className = Settings.UseShortNames ? classGroup.Key.ShortName () : classGroup.Key.DisplayName;
        //To.ConsoleLine.e (() => className);
        WriteTableDataWithRowCount (className, classGroup.Count ());
      }
      else
      {
        WriteTableDataWithRowCount ("_NO_CLASSES_DEFINED_", classGroup.Count ());
      }

      foreach (var aclExpansionEntry in classGroup)
      {
        WriteTableRowBeginIfNotInTableRow ();

        tdBodyStates (aclExpansionEntry);
        tdBodyConditions (aclExpansionEntry.AccessConditions);
        tdBodyAccessTypes (aclExpansionEntry.AccessTypeDefinitions);

        WriteTableRowEnd ();
      }
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


    public IEnumerable<IGrouping<T, AclExpansionEntry>> GetAclExpansionGrouping<T, TIn> (
      IGrouping<TIn, AclExpansionEntry> linqGroup,
      Func<AclExpansionEntry, T> groupingKeyFunc)
    {
      return from aee in linqGroup
             group aee by groupingKeyFunc (aee);
    }


    public IEnumerable<IGrouping<User, AclExpansionEntry>> GetAclExpansionGrouping (IEnumerable<AclExpansionEntry> aclExpansion,
      Func<AclExpansionEntry, User> groupingKeyFunc)
    {
      return from aee in aclExpansion
             group aee by groupingKeyFunc (aee);    
    }
  }
}



