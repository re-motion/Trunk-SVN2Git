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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionHtmlWriter : AclExpansionWriter
  {
    private readonly HtmlWriter _htmlWriter;
    private bool _inTableRow;

    public AclExpansionHtmlWriter (TextWriter textWriter, bool indentXml)
    {
      _htmlWriter = new HtmlWriter (textWriter, indentXml);
    }

    public AclExpansionHtmlWriter (XmlWriter xmlWriter)
    {
      _htmlWriter = new HtmlWriter (xmlWriter);
    }

    // Spike implementation using HtmlWriter
    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      WriteAclExpansionAsHtml (aclExpansion);
    }


    public void WriteAclExpansionAsHtml (List<AclExpansionEntry> aclExpansion)
    {
      var html = _htmlWriter;

      WriteStartPage ();

      //html.value ("re-motion ACL Expansion body");

      WriteTableStart (html);
      WriteTableHeaders (html);
      WriteTableBody (aclExpansion);
      WriteTableEnd (html);

      WriteEndPage (html);
    }

    private void WriteTableEnd (HtmlWriter html)
    {
      html.tableEnd ();
    }

    private void WriteTableStart (HtmlWriter html)
    {
      html.table ().a ("style", "width: 100%;").a ("class", "aclExpansionTable").a ("id", "remotion-ACL-expansion-table");
    }

    private void WriteTableHeaders (HtmlWriter html)
    {
      html.tr ();
      WriteHeaderCell ("User");
      WriteHeaderCell ("Role");
      WriteHeaderCell ("Class");
      WriteHeaderCell ("States");
      WriteHeaderCell ("User Must Own");
      WriteHeaderCell ("Group Must Own");
      WriteHeaderCell ("Tenant Must Own");
      WriteHeaderCell ("User Must Have Abstract Role");
      WriteHeaderCell ("Access Rights");
      html.trEnd ();
    }

    private void WriteEndPage (HtmlWriter html)
    {
      html.TagEnd ("body");
      html.TagEnd ("html");

      html.Close ();
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
      _htmlWriter.td ().a ("class", "header");
      _htmlWriter.Value (columnName);
      _htmlWriter.tdEnd ();
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

    //void WriteTableDataWithAddendum (string value, Object addendum)
    //{
    //  _htmlWriter.td ();
    //  WriteTableDataAddendum (addendum);
    //  _htmlWriter.Value (value);
    //  _htmlWriter.tdEnd ();
    //}


    void WriteTableDataWithRowCount (string value, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow ();
      _htmlWriter.td ();
      WriteRowspanAttribute(rowCount);
      _htmlWriter.Value (value);
      WriteTableDataAddendum (rowCount);
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

    private void WriteTableDataForRole (Role role, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow();
      _htmlWriter.td ();
      WriteRowspanAttribute (rowCount);
      _htmlWriter.Value (role.Group.DisplayName);
      _htmlWriter.Value (", ");
      _htmlWriter.Value (role.Position.DisplayName);
      WriteTableDataAddendum (rowCount);
      _htmlWriter.tdEnd ();
    }
  

    void tdBodyStates (AclExpansionEntry aclExpansionEntry) // params StateDefinition[] stateDefinitions)
    {
      var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).ToArray ();
      _htmlWriter.td ();
      bool firstElement = true;
      foreach (StateDefinition stateDefiniton in stateDefinitions)
      {
        if (!firstElement)
        {
          _htmlWriter.br ();
        }
        _htmlWriter.Value (stateDefiniton.DisplayName);
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

 

    //private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    //{
    //  var aclExpansionUserGrouping = GetAclExpansionGrouping (aclExpansion, (aee => aee.User));

    //  foreach (var userGroup in aclExpansionUserGrouping)
    //  {
    //    var userName = userGroup.Key.DisplayName;
    //    WriteTableRowBeginIfNotInTableRow();
    //    WriteTableDataWithRowCount (userName, userGroup.Items.Count());

    //    var aclExpansionRoleGrouping = GetAclExpansionGrouping (userGroup, (x => x.Role));

    //    foreach (var roleGroup in aclExpansionRoleGrouping)
    //    {
    //      var role = roleGroup.Key;
    //      WriteTableRowBeginIfNotInTableRow ();
    //      WriteTableDataForRole (role, roleGroup.Items.Count ());

    //      var aclExpansionClassGrouping = GetAclExpansionGrouping (roleGroup, (x => x.Class));

    //      foreach (var classGroup in aclExpansionClassGrouping)
    //      {
    //        var className = classGroup.Key.DisplayName;
    //        WriteTableRowBeginIfNotInTableRow ();
    //        WriteTableDataWithRowCount (className, classGroup.Items.Count ());

    //        foreach (var aclExpansionEntry in classGroup.Items)
    //        {
    //          WriteTableRowBeginIfNotInTableRow();

    //          tdBodyStates (aclExpansionEntry);
    //          tdBodyConditions (aclExpansionEntry.AccessConditions);
    //          tdBodyAccessTypes (aclExpansionEntry.AccessTypeDefinitions);

    //          WriteTableRowEnd();

    //        }
    //      }
    //    }
    //  }
    //}



    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var aclExpansionUserGrouping = GetAclExpansionGrouping (aclExpansion, (aee => aee.User));
      foreach (var userGroup in aclExpansionUserGrouping)
      {
        WriteTableBody_ProcessUserGroup(userGroup);
      }
    }

    private void WriteTableBody_ProcessUserGroup (LinqGroup<User, AclExpansionEntry> userGroup)
    {
      var userName = userGroup.Key.DisplayName;
      //WriteTableRowBeginIfNotInTableRow ();
      WriteTableDataWithRowCount (userName, userGroup.Items.Count ());

      var aclExpansionRoleGrouping = GetAclExpansionGrouping (userGroup, (x => x.Role));

      foreach (var roleGroup in aclExpansionRoleGrouping)
      {
        WriteTableBody_ProcessRoleGroup(roleGroup);
      }
    }

    private void WriteTableBody_ProcessRoleGroup (LinqGroup<Role, AclExpansionEntry> roleGroup)
    {
      //WriteTableRowBeginIfNotInTableRow ();
      WriteTableDataForRole (roleGroup.Key, roleGroup.Items.Count ());

      var aclExpansionClassGrouping = GetAclExpansionGrouping (roleGroup, (x => x.Class));

      foreach (var classGroup in aclExpansionClassGrouping)
      {
        WriteTableBody_ProcessClassGroup(classGroup);
      }
    }

    private void WriteTableBody_ProcessClassGroup (LinqGroup<SecurableClassDefinition, AclExpansionEntry> classGroup)
    {
      WriteTableDataWithRowCount (classGroup.Key.DisplayName, classGroup.Items.Count ());

      foreach (var aclExpansionEntry in classGroup.Items)
      {
        WriteTableRowBeginIfNotInTableRow ();

        tdBodyStates (aclExpansionEntry);
        tdBodyConditions (aclExpansionEntry.AccessConditions);
        tdBodyAccessTypes (aclExpansionEntry.AccessTypeDefinitions);

        WriteTableRowEnd ();
      }
    }


    private void WriteTableRowBeginIfNotInTableRow ()
    {
      if (!_inTableRow)
      {
        _htmlWriter.tr ();
        _inTableRow = true;
      }
    }

    private void WriteTableRowEnd ()
    {
      _htmlWriter.trEnd ();
      _inTableRow = false;
    }


    private IEnumerable<LinqGroup<T, AclExpansionEntry>> GetAclExpansionGrouping<T, TIn> (
      LinqGroup<TIn, AclExpansionEntry> linqGroup,
      Func<AclExpansionEntry, T> groupingKeyFunc)
    {
      return from aee in linqGroup.Items
             //group aee by aee.Role
             group aee by groupingKeyFunc (aee)
               into groupEntries
               select ObjectMother.LinqGroup.New (groupEntries);
    }


    private IEnumerable<LinqGroup<User, AclExpansionEntry>> GetAclExpansionGrouping (IEnumerable<AclExpansionEntry> aclExpansion,
      Func<AclExpansionEntry, User> groupingKeyFunc)
    {
      return from aee in aclExpansion
             group aee by groupingKeyFunc (aee)
             into groupEntries
                 select ObjectMother.LinqGroup.New(groupEntries);
    }
  }
}



