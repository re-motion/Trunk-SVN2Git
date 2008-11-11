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
using Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="IAclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>
  /// as a single HTML table.
  /// </summary>
  public class AclExpansionHtmlWriter : AclExpansionHtmlWriterBase
  {
    private readonly AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();
    private string _statelessAclStateHtmlText = "(stateless)";


    public AclExpansionHtmlWriter (TextWriter textWriter, bool indentXml)
    {
      htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }

    public AclExpansionHtmlWriter (XmlWriter xmlWriter)
    {
      htmlTagWriter = new HtmlTagWriter (xmlWriter);
    }

    public string StatelessAclStateHtmlText
    {
      get { return _statelessAclStateHtmlText; }
      set { _statelessAclStateHtmlText = value; }
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

      WritePageStart ("re-motion ACL Expansion");

      WriteTableStart ("remotion-ACL-expansion-table");
      WriteTableHeaders ();
      WriteTableBody (aclExpansion);
      WriteTableEnd ();

      WritePageEnd ();
    }


    private void WriteTableHeaders ()
    {
      htmlTagWriter.Tags.tr ();
      WriteHeaderCell ("User");
      WriteHeaderCell ("Role");
      WriteHeaderCell ("Class");
      WriteHeaderCell ("States");
      WriteHeaderCell ("User Must Own");
      WriteHeaderCell ("Group Must Own");
      WriteHeaderCell ("Tenant Must Own");
      WriteHeaderCell ("User Must Have Abstract Role");
      WriteHeaderCell ("Access Rights");
      htmlTagWriter.Tags.trEnd ();
    }



    private void WriteTableDataAddendum (Object addendum)
    {
      if (addendum != null)
      {
        htmlTagWriter.Value (" (");
        htmlTagWriter.Value (addendum);
        htmlTagWriter.Value (") ");
      }
    }


    private void WriteTableDataWithRowCount (string value, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow ();
      htmlTagWriter.Tags.td ();
      WriteRowspanAttribute(rowCount);
      htmlTagWriter.Value (value);
      if (Settings.OutputRowCount)
      {
        WriteTableDataAddendum (rowCount);
      }
      htmlTagWriter.Tags.tdEnd ();
    }



    private HtmlTagWriter WriteRowspanAttribute (int rowCount)
    {
      if (rowCount > 0)
      {
        htmlTagWriter.Attribute ("rowspan", Convert.ToString (rowCount));
      }
      return htmlTagWriter;
    }

    private void WriteTableDataForRole (Role role, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow();
      htmlTagWriter.Tags.td ();
      WriteRowspanAttribute (rowCount);
      htmlTagWriter.Value (role.Group.DisplayName);
      htmlTagWriter.Value (", ");
      htmlTagWriter.Value (role.Position.DisplayName);
      if (Settings.OutputRowCount)
      {
        WriteTableDataAddendum (rowCount);
      }
      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForBodyStates (AclExpansionEntry aclExpansionEntry)
    {
      htmlTagWriter.Tags.td ();

      if (aclExpansionEntry.AccessControlList is StatelessAccessControlList)
      {
        htmlTagWriter.Value (StatelessAclStateHtmlText);
      }
      else
      {
        var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates()).OrderBy (x => x.DisplayName).ToArray();
        bool firstElement = true;
        foreach (StateDefinition stateDefiniton in stateDefinitions)
        {
          if (!firstElement)
          {
            //_htmlWriter.br ();
            htmlTagWriter.Value (", ");
          }

          string stateName = Settings.ShortenNames ? stateDefiniton.ShortName() : stateDefiniton.DisplayName;
          //To.ConsoleLine.e (() => stateName);

          htmlTagWriter.Value (stateName);
          firstElement = false;
        }
      }
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      var accessTypeDefinitionsSorted = from atd in accessTypeDefinitions
                                        orderby atd.DisplayName
                                        select atd;

      htmlTagWriter.Tags.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitionsSorted)
      {
        if (!firstElement)
        {
          htmlTagWriter.Value (", ");
        }
        htmlTagWriter.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForBodyConditions (AclExpansionAccessConditions conditions)
    {
      WriteTableDataForBooleanCondition (conditions.IsOwningUserRequired);
      WriteTableDataForBooleanCondition (conditions.IsOwningGroupRequired);
      WriteTableDataForBooleanCondition (conditions.IsOwningTenantRequired);

      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (conditions.IsAbstractRoleRequired ? conditions.AbstractRole.DisplayName : "");
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForBooleanCondition (bool required)
    {
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (required ? "X" : "");
      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);

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
        string className = Settings.ShortenNames ? classGroup.Key.ShortName () : classGroup.Key.DisplayName;
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
        WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);

        WriteTableRowEnd ();
      }
    }

  }
}



