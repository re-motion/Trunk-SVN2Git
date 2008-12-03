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
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionHtmlWriterImplementation : AclExpansionHtmlWriterImplementationBase
  {
    //private AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();
    private readonly AclExpansionHtmlWriterSettings _settings;
    private string _statelessAclStateHtmlText = "(stateless)";
    private string _aclWithNoAssociatedStatesHtmlText = "(no associated states)";

    //AclExpansionHtmlWriterImplementation (TextWriter textWriter, bool indentXml)
    //{
    //  htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    //}

    public AclExpansionHtmlWriterImplementation (TextWriter textWriter, bool indentXml, AclExpansionHtmlWriterSettings settings)
      : base (textWriter, indentXml)
    {
      _settings = settings;
    }

    //public AclExpansionHtmlWriterSettings Settings
    //{
    //  get { return _settings; }
    //  set { _settings = value; }
    //}


    public string StatelessAclStateHtmlText
    {
      get { return _statelessAclStateHtmlText; }
      set { _statelessAclStateHtmlText = value; }
    }
    
    public string AclWithNoAssociatedStatesHtmlText
    {
      get { return _aclWithNoAssociatedStatesHtmlText; }
      set { _aclWithNoAssociatedStatesHtmlText = value; }
    }


    private void WriteTableDataAddendum (object addendum) 
    {
      if (addendum != null)
      {
        HtmlTagWriter.Value (" (");
        HtmlTagWriter.Value (addendum);
        HtmlTagWriter.Value (") ");
      }
    }


    public void WriteTableDataWithRowCount (string value, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow ();
      HtmlTagWriter.Tags.td ();
      WriteRowspanAttribute(rowCount);
      HtmlTagWriter.Value (value);
      if (_settings.OutputRowCount)
      { 
        WriteTableDataAddendum (rowCount);
      }
      HtmlTagWriter.Tags.tdEnd ();
    }


    private void WriteRowspanAttribute (int rowCount)
    {
      if (rowCount > 0)
      { 
        HtmlTagWriter.Attribute ("rowspan", Convert.ToString (rowCount));
      }
    }

    public void WriteTableDataForRole (Role role, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow();
      HtmlTagWriter.Tags.td ();
      WriteRowspanAttribute (rowCount);
      HtmlTagWriter.Value (role.Group.DisplayName);
      HtmlTagWriter.Value (", ");
      HtmlTagWriter.Value (role.Position.DisplayName);
      if (_settings.OutputRowCount)
      { 
        WriteTableDataAddendum (rowCount);
      }
      HtmlTagWriter.Tags.tdEnd ();
    }


    public void WriteTableDataBodyForSingleState (AclExpansionEntry aclExpansionEntry)
    {
      if (aclExpansionEntry.AccessControlList is StatelessAccessControlList)
      {
        HtmlTagWriter.Value (StatelessAclStateHtmlText);
      }
      else
      {
        IOrderedEnumerable<StateDefinition> stateDefinitions = GetAllStatesForAclExpansionEntry(aclExpansionEntry);

        if (!stateDefinitions.Any ())
        { 
          HtmlTagWriter.Value (AclWithNoAssociatedStatesHtmlText);
        }
        else
        {
          bool firstElement = true;
          foreach (StateDefinition stateDefiniton in stateDefinitions)
          {
            if (!firstElement)
            {
              HtmlTagWriter.Value (", ");
            }

            string stateName = _settings.ShortenNames ? stateDefiniton.ShortName () : stateDefiniton.DisplayName;

            HtmlTagWriter.Value (stateName);
            firstElement = false;
          }
        }
      }
    }

    private IOrderedEnumerable<StateDefinition> GetAllStatesForAclExpansionEntry (AclExpansionEntry aclExpansionEntry)
    {
      // Get all states for AclExpansionEntry by flattening the StateCombinations of the AccessControlEntry ACL.
      return aclExpansionEntry.GetStateCombinations().SelectMany (x => x.GetStates ()).OrderBy (x => x.DisplayName);
    }


    public void WriteTableDataForAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      var accessTypeDefinitionsSorted = from atd in accessTypeDefinitions
                                        orderby atd.DisplayName
                                        select atd;

      HtmlTagWriter.Tags.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitionsSorted)
      {
        if (!firstElement)
        { 
          HtmlTagWriter.Value (", ");
        }
        HtmlTagWriter.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      HtmlTagWriter.Tags.tdEnd ();
    }

    public void WriteTableDataForBodyConditions (AclExpansionAccessConditions accessConditions)
    {
      WriteTableDataForBooleanCondition (accessConditions.IsOwningUserRequired);
      WriteTableDataForOwningGroupCondition (accessConditions);
      WriteTableDataForOwningTenantCondition (accessConditions);
      WriteTableDataForAbstractRoleCondition(accessConditions);
    }

    private void WriteTableDataForAbstractRoleCondition (AclExpansionAccessConditions accessConditions)
    {
      HtmlTagWriter.Tags.td ();
      HtmlTagWriter.Value (accessConditions.IsAbstractRoleRequired ? accessConditions.AbstractRole.DisplayName : "");
      HtmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForOwningGroupCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.GroupHierarchyCondition == GroupHierarchyCondition.Undefined && conditions.OwningGroup != null);
      HtmlTagWriter.Tags.td();
      HtmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningGroup = conditions.OwningGroup;
      if (owningGroup != null)
      { 
        HtmlTagWriter.Value (owningGroup.DisplayName);
      }

      var groupHierarchyCondition = conditions.GroupHierarchyCondition;

      // Bitwise operation is OK (alas marking GroupHierarchyCondition with [Flags] is not supported). 
      if ((groupHierarchyCondition & GroupHierarchyCondition.Parent) != 0)
      {
        HtmlTagWriter.Tags.br ();
        HtmlTagWriter.Value ("or its parents");
      }

      // Bitwise operation is OK (alas marking GroupHierarchyCondition with [Flags] is not supported). 
      if ((groupHierarchyCondition & GroupHierarchyCondition.Children) != 0)
      {
        HtmlTagWriter.Tags.br ();
        HtmlTagWriter.Value ("or its children");
      }

      HtmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForOwningTenantCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.TenantHierarchyCondition == TenantHierarchyCondition.Undefined && conditions.OwningTenant != null);
      HtmlTagWriter.Tags.td ();
      HtmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningTenant = conditions.OwningTenant;
      if (owningTenant != null)
      { 
        HtmlTagWriter.Value (owningTenant.DisplayName);
      }

      var tenantHierarchyCondition = conditions.TenantHierarchyCondition;
      // Bitwise operation is OK (alas marking TenantHierarchyCondition with [Flags] is not supported). 
      if ((tenantHierarchyCondition & TenantHierarchyCondition.Parent) != 0)
      {
        HtmlTagWriter.Tags.br ();
        HtmlTagWriter.Value ("or its parents");
      }

      HtmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForBooleanCondition (bool required)
    {
      HtmlTagWriter.Tags.td ();
      HtmlTagWriter.Value (required ? "X" : ""); // TODO AE: Test missing for one of these cases
      HtmlTagWriter.Tags.tdEnd ();
    }



    //private void WriteTableBody (AclExpansionTree aclExpansionTree)
    //{
    //  foreach (var userNode in aclExpansionTree.Tree)
    //  {
    //    WriteTableBody_ProcessUser(userNode);
    //  }
    //}

    //private void WriteTableBody_ProcessUser (AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>> userNode)
    //{
    //  WriteTableDataWithRowCount (userNode.Key.DisplayName, userNode.NumberLeafNodes);
  
    //  foreach (var roleNode in userNode.Children)
    //  {
    //    WriteTableBody_ProcessRole(roleNode);
    //  }
    //}

    //private void WriteTableBody_ProcessRole (AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>> roleNode)
    //{
    //  WriteTableDataForRole (roleNode.Key, roleNode.NumberLeafNodes);
 
    //  foreach (var classNode in roleNode.Children)
    //  {
    //    WriteTableBody_ProcessClass(classNode);
    //  }
    //}

    //private void WriteTableBody_ProcessClass (AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> classNode)
    //{
    //  if (classNode.Key != null)
    //  {
    //    string className = Settings.ShortenNames ? classNode.Key.ShortName () : classNode.Key.DisplayName;
    //    WriteTableDataWithRowCount (className, classNode.NumberLeafNodes);
    //  }
    //  else
    //  {
    //    WriteTableDataWithRowCount ("_NO_CLASSES_DEFINED_", classNode.NumberLeafNodes);
    //  }

    //  WriteTableBody_ProcessStates(classNode.Children);
    //}


    //private void WriteTableBody_ProcessStates (IList<AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> states)
    //{
    //  // States Output
    //  foreach (var aclExpansionTreeNode in states)
    //  {
    //    WriteTableRowBeginIfNotInTableRow ();

    //    // Write all states combined into one cell
    //    WriteTableDataForStates (aclExpansionTreeNode.Children);

    //    AclExpansionEntry aclExpansionEntry = aclExpansionTreeNode.Key;
    //    WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
    //    WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);
    //    if (Settings.OutputDeniedRights)
    //    {
    //      WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
    //    }

    //    WriteTableRowEnd ();
    //  }
    //}


  //  private void WriteTableDataForStates (IList<AclExpansionEntry> aclExpansionEntriesWhichOnlyDiffersInStates)
  //  {
  //    HtmlTagWriter.Tags.td ();

  //    bool firstElement = true;

  //    foreach (AclExpansionEntry aclExpansionEntry in aclExpansionEntriesWhichOnlyDiffersInStates)
  //    {
  //      if (!firstElement)
  //      {
  //        HtmlTagWriter.Value ("; ");
  //      }

  //      WriteTableDataBodyForSingleState (aclExpansionEntry);
  //      firstElement = false;
  //    }
  //    HtmlTagWriter.Tags.tdEnd ();
  //  }    

  }
}