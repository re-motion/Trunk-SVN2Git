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
    private readonly AclExpansionTree _aclExpansionTree;
    private AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();
    private string _statelessAclStateHtmlText = "(stateless)";

    public AclExpansionHtmlWriter (List<AclExpansionEntry> aclExpansion, TextWriter textWriter, bool indentXml)
    {
      _aclExpansionTree = new AclExpansionTree (aclExpansion);
      htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }

    public AclExpansionHtmlWriter (AclExpansionTree aclExpansionTree, TextWriter textWriter, bool indentXml)
    {
      _aclExpansionTree = aclExpansionTree;
      htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }


    public string StatelessAclStateHtmlText
    {
      get { return _statelessAclStateHtmlText; }
      set { _statelessAclStateHtmlText = value; }
    }

    public AclExpansionHtmlWriterSettings Settings
    {
      get { return _settings; }
      set { _settings = value; }
    }


    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      //WriteAclExpansionAsHtml ();
      throw new NotImplementedException();
    }


    public void WriteAclExpansionAsHtml ()
    {
      //ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);

      //var aclExpansionTree = new AclExpansionTree (aclExpansion);


      WritePageStart ("re-motion ACL Expansion");

      WriteTableStart ("remotion-ACL-expansion-table");
      WriteTableHeaders ();
      WriteTableBody ();
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
      WriteHeaderCell ("Owning Group Equals");
      WriteHeaderCell ("Tenant Must Own");
      WriteHeaderCell ("User Must Have Abstract Role");
      WriteHeaderCell ("Access Rights");
      if (Settings.OutputDeniedRights)
      {
        WriteHeaderCell ("Denied Rights");
      }
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
        // Get the states by flattening the StateCombinations of the AccessControlEntry ACL 
        var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates()).OrderBy (x => x.DisplayName).ToArray();
        bool firstElement = true;
        foreach (StateDefinition stateDefiniton in stateDefinitions)
        {
          if (!firstElement)
          {
            htmlTagWriter.Value (", ");
          }

          string stateName = Settings.ShortenNames ? stateDefiniton.ShortName() : stateDefiniton.DisplayName;

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
      
      //WriteTableDataForBooleanCondition (conditions.HasOwningGroupCondition);
      WriteTableDataForOwningGroupCondition (conditions);
      
      WriteTableDataForBooleanCondition (conditions.IsOwningTenantRequired);

      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (conditions.IsAbstractRoleRequired ? conditions.AbstractRole.DisplayName : "");
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForOwningGroupCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.GroupHierarchyCondition == GroupHierarchyCondition.Undefined && conditions.OwningGroup != null);
      htmlTagWriter.Tags.td();
      htmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningGroup = conditions.OwningGroup;
      if (owningGroup != null)
      {
        htmlTagWriter.Value (owningGroup.DisplayName);
      }

      var groupHierarchyCondition = conditions.GroupHierarchyCondition;
      if ((groupHierarchyCondition & GroupHierarchyCondition.Parent) != 0)
      {
        //htmlTagWriter.Value (",");
        htmlTagWriter.Tags.br ();
        htmlTagWriter.Value ("or its parents");
      }

      if ((groupHierarchyCondition & GroupHierarchyCondition.Children) != 0)
      {
        //htmlTagWriter.Value (",");
        htmlTagWriter.Tags.br ();
        htmlTagWriter.Value ("or its children");
      }

      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForBooleanCondition (bool required)
    {
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (required ? "X" : "");
      htmlTagWriter.Tags.tdEnd ();
    }



    private void WriteTableBody ()
    {
      foreach (var userNode in _aclExpansionTree.Tree)
      {
        WriteTableBody_ProcessUser(userNode);
      }
    }

    private void WriteTableBody_ProcessUser (AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry>>> userNode)
    {
      WriteTableDataWithRowCount (userNode.Key.DisplayName, userNode.NumberLeafNodes);
  
      foreach (var roleNode in userNode.Children)
      {
        WriteTableBody_ProcessRole(roleNode);
      }
    }

    private void WriteTableBody_ProcessRole (AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry>> roleNode)
    {
      WriteTableDataForRole (roleNode.Key, roleNode.NumberLeafNodes);
 
      foreach (var classNode in roleNode.Children)
      {
        WriteTableBody_ProcessClass(classNode);
      }
    }

    private void WriteTableBody_ProcessClass (AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry> classNode)
    {
      if (classNode.Key != null)
      {
        string className = Settings.ShortenNames ? classNode.Key.ShortName () : classNode.Key.DisplayName;
        WriteTableDataWithRowCount (className, classNode.NumberLeafNodes);
      }
      else
      {
        WriteTableDataWithRowCount ("_NO_CLASSES_DEFINED_", classNode.NumberLeafNodes);
      }

      WriteTableBody_ProcessStates(classNode.Children);
    }


    //private void WriteTableBody_ProcessStates (AclExpansionTreeNode<SecurableClassDefinition, AclExpansionEntry> classNode)
    private void WriteTableBody_ProcessStates (IList<AclExpansionEntry> states)
    {
      // States Output
      foreach (var aclExpansionEntry in states)
      {
        WriteTableRowBeginIfNotInTableRow ();

        WriteTableDataForBodyStates (aclExpansionEntry);
        WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
        WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);
        if (Settings.OutputDeniedRights)
        {
          WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
        }

        WriteTableRowEnd ();
      }
    }
  }
}



