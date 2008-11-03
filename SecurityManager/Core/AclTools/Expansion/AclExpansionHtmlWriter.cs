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

    public AclExpansionHtmlWriter (TextWriter textWriter, bool indentXml)
    {
      htmlWriter = new HtmlWriter (textWriter, indentXml);
    }

    public AclExpansionHtmlWriter (XmlWriter xmlWriter)
    {
      htmlWriter = new HtmlWriter (xmlWriter);
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
      htmlWriter.Tags.tr ();
      WriteHeaderCell ("User");
      WriteHeaderCell ("Role");
      WriteHeaderCell ("Class");
      WriteHeaderCell ("States");
      WriteHeaderCell ("User Must Own");
      WriteHeaderCell ("Group Must Own");
      WriteHeaderCell ("Tenant Must Own");
      WriteHeaderCell ("User Must Have Abstract Role");
      WriteHeaderCell ("Access Rights");
      htmlWriter.Tags.trEnd ();
    }



    private void WriteTableDataAddendum (Object addendum)
    {
      if (addendum != null)
      {
        htmlWriter.Value (" (");
        htmlWriter.Value (addendum);
        htmlWriter.Value (") ");
      }
    }


    private void WriteTableDataWithRowCount (string value, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow ();
      htmlWriter.Tags.td ();
      WriteRowspanAttribute(rowCount);
      htmlWriter.Value (value);
      if (Settings.OutputRowCount)
      {
        WriteTableDataAddendum (rowCount);
      }
      htmlWriter.Tags.tdEnd ();
    }



    private HtmlWriter WriteRowspanAttribute (int rowCount)
    {
      if (rowCount > 0)
      {
        htmlWriter.a ("rowspan", Convert.ToString (rowCount));
      }
      return htmlWriter;
    }

    private void WriteTableDataForRole (Role role, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow();
      htmlWriter.Tags.td ();
      WriteRowspanAttribute (rowCount);
      htmlWriter.Value (role.Group.DisplayName);
      htmlWriter.Value (", ");
      htmlWriter.Value (role.Position.DisplayName);
      if (Settings.OutputRowCount)
      {
        WriteTableDataAddendum (rowCount);
      }
      htmlWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForBodyStates (AclExpansionEntry aclExpansionEntry)
    {
      var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).OrderBy(x => x.DisplayName).ToArray ();
      htmlWriter.Tags.td ();
      bool firstElement = true;
      foreach (StateDefinition stateDefiniton in stateDefinitions)
      {
        if (!firstElement)
        {
          //_htmlWriter.br ();
          htmlWriter.Value (", ");
        }

        string stateName = Settings.UseShortNames ? stateDefiniton.ShortName () : stateDefiniton.DisplayName;
        //To.ConsoleLine.e (() => stateName);

        htmlWriter.Value (stateName);
        firstElement = false;
      }
      htmlWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      var accessTypeDefinitionsSorted = from atd in accessTypeDefinitions
                                        orderby atd.DisplayName
                                        select atd;

      htmlWriter.Tags.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitionsSorted)
      {
        if (!firstElement)
        {
          htmlWriter.Value (", ");
        }
        htmlWriter.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      htmlWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForBodyConditions (AclExpansionAccessConditions conditions)
    {
      WriteTableDataForBooleanCondition (conditions.IsOwningUserRequired);
      WriteTableDataForBooleanCondition (conditions.IsOwningGroupRequired);
      WriteTableDataForBooleanCondition (conditions.IsOwningTenantRequired);

      htmlWriter.Tags.td ();
      htmlWriter.Value (conditions.IsAbstractRoleRequired ? conditions.AbstractRole.DisplayName : "");
      htmlWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForBooleanCondition (bool required)
    {
      htmlWriter.Tags.td ();
      htmlWriter.Value (required ? "X" : "");
      htmlWriter.Tags.tdEnd ();
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
        string className = Settings.UseShortNames ? classGroup.Key.ShortName () : classGroup.Key.DisplayName;
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

  }
}



