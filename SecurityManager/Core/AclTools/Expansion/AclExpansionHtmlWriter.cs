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
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="AclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>
  /// as a single HTML table.
  /// </summary>
  public class AclExpansionHtmlWriter : AclExpansionWriter
  {
    private readonly HtmlWriter _htmlWriter;
    private bool _isInTableRow;
    private readonly AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();

    public AclExpansionHtmlWriter (TextWriter textWriter, bool indentXml)
    {
      _htmlWriter = new HtmlWriter (textWriter, indentXml);
    }

    public AclExpansionHtmlWriter (XmlWriter xmlWriter)
    {
      _htmlWriter = new HtmlWriter (xmlWriter);
    }


    public AclExpansionHtmlWriterSettings Settings
    {
      get { return _settings; }
    }


    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      WriteAclExpansionAsHtml (aclExpansion);
    }


    public void WriteAclExpansionAsHtml (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);

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

    private void WritePageEnd ()
    {
      _htmlWriter.TagEnd ("body");
      _htmlWriter.TagEnd ("html");

      _htmlWriter.Close ();
    }

    private HtmlWriter WritePageStart ()
    {
      _htmlWriter.WritePageHeader ("re-motion ACL Expansion", "AclExpansion.css");

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

    private void WriteTableDataAddendum (Object addendum)
    {
      if (addendum != null)
      {
        _htmlWriter.Value (" (");
        _htmlWriter.Value (addendum);
        _htmlWriter.Value (") ");
      }
    }


    private void WriteTableDataWithRowCount (string value, int rowCount)
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

    private void WriteTableDataForRole (Role role, int rowCount)
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


    private void WriteTableDataForBodyStates (AclExpansionEntry aclExpansionEntry)
    {
      var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).OrderBy(x => x.DisplayName).ToArray ();
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

    private void WriteTableDataForAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      var accessTypeDefinitionsSorted = from atd in accessTypeDefinitions
                                        orderby atd.DisplayName
                                        select atd;

      _htmlWriter.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitionsSorted)
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

    private void WriteTableDataForBodyConditions (AclExpansionAccessConditions conditions)
    {
      WriteTableDataForBooleanCondition (conditions.IsOwningUserRequired);
      WriteTableDataForBooleanCondition (conditions.IsOwningGroupRequired);
      WriteTableDataForBooleanCondition (conditions.IsOwningTenantRequired);

      _htmlWriter.td ();
      _htmlWriter.Value (conditions.IsAbstractRoleRequired ? conditions.AbstractRole.DisplayName : "");
      _htmlWriter.tdEnd ();
    }

    private void WriteTableDataForBooleanCondition (bool required)
    {
      _htmlWriter.td ();
      _htmlWriter.Value (required ? "X" : "");
      _htmlWriter.tdEnd ();
    }


    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      //var aclExpansionUserGrouping = GetAclExpansionGrouping (aclExpansion, (aee => aee.User));

      var aclExpansionUserGrouping = from aee in aclExpansion
                                     orderby aee.User.DisplayName
                                     group aee by aee.User;

      foreach (var userGroup in aclExpansionUserGrouping)
      {
        WriteTableBody_ProcessUserGroup(userGroup);
      }
    }

    private void WriteTableBody_ProcessUserGroup (IGrouping<User, AclExpansionEntry> userGroup)
    {
      WriteTableDataWithRowCount (userGroup.Key.DisplayName, userGroup.Count ());
  
      //var aclExpansionRoleGrouping = GetAclExpansionGrouping (userGroup, (x => x.Role));
      var aclExpansionRoleGrouping = from aee in userGroup
                                     orderby aee.Role.Group.DisplayName, aee.Role.Position.DisplayName
                                     group aee by aee.Role;

      foreach (var roleGroup in aclExpansionRoleGrouping)
      {
        WriteTableBody_ProcessRoleGroup(roleGroup);
      }
    }

    private void WriteTableBody_ProcessRoleGroup (IGrouping<Role, AclExpansionEntry> roleGroup)
    {
      WriteTableDataForRole (roleGroup.Key, roleGroup.Count ());
 
      //var aclExpansionClassGrouping = GetAclExpansionGrouping (roleGroup, (x => x.Class));
      var aclExpansionClassGrouping = from aee in roleGroup
                                      orderby aee.Class.DisplayName
                                      group aee by aee.Class;


      foreach (var classGroup in aclExpansionClassGrouping)
      {
        WriteTableBody_ProcessClassGroup(classGroup);
      }
    }

    private void WriteTableBody_ProcessClassGroup (IGrouping<SecurableClassDefinition, AclExpansionEntry> classGroup)
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

        WriteTableDataForBodyStates (aclExpansionEntry);
        WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
        WriteTableDataForAccessTypes (aclExpansionEntry.AccessTypeDefinitions);

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


    //public IEnumerable<IGrouping<T, AclExpansionEntry>> GetAclExpansionGrouping<T, TIn> (
    //  IGrouping<TIn, AclExpansionEntry> linqGroup,
    //  Func<AclExpansionEntry, T> groupingKeyFunc)
    //{
    //  return from aee in linqGroup
    //         group aee by groupingKeyFunc (aee);
    //}


    //public IEnumerable<IGrouping<User, AclExpansionEntry>> GetAclExpansionGrouping (IEnumerable<AclExpansionEntry> aclExpansion,
    //  Func<AclExpansionEntry, User> groupingKeyFunc)
    //{
    //  return from aee in aclExpansion
    //         group aee by groupingKeyFunc (aee);    
    //}

  }
}



