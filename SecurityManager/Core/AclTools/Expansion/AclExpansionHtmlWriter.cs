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
using Remotion.Collections;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
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
  // TODO AE: Remove commented code. (Do not commit.)
  // TODO AE: Globalization is not supported for hard-coded strings. Is this a problem for this application?
  // TODO AE: Split this class! Extract most of the private methods to another class to improve clarity (and to allow for more fine-grained testability).
  public class AclExpansionHtmlWriter : AclExpansionHtmlWriterBase
  {
    //private readonly AclExpansionTree _aclExpansionTree;
    private AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();
    private string _statelessAclStateHtmlText = "(stateless)";
    private string _aclWithNoAssociatedStatesHtmlText = "(no associated states)";

    public AclExpansionHtmlWriter (TextWriter textWriter, bool indentXml)
    {
      htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }


    //public AclExpansionHtmlWriter (List<AclExpansionEntry> aclExpansion, TextWriter textWriter, bool indentXml)
    //{
    //  // TODO AE: Delegate to other ctor.
    //  _aclExpansionTree = new AclExpansionTree (aclExpansion);
    //  htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    //}

    //public AclExpansionHtmlWriter (AclExpansionTree aclExpansionTree, TextWriter textWriter, bool indentXml)
    //{
    //  _aclExpansionTree = aclExpansionTree;
    //  htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    //}
    

    public AclExpansionHtmlWriterSettings Settings
    {
      get { return _settings; }
      set { _settings = value; }
    }


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


    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
    
      var aclExpansionTree = new AclExpansionTree (aclExpansion);

      WritePageStart ("re-motion ACL Expansion");

      WriteTableStart ("remotion-ACL-expansion-table");
      WriteTableHeaders ();
      WriteTableBody (aclExpansionTree);
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
      WriteHeaderCell ("Owning Tenant Equals");
      WriteHeaderCell ("User Must Have Abstract Role");
      WriteHeaderCell ("Access Rights");
      if (Settings.OutputDeniedRights)
      { // TODO AE: Braces.
        WriteHeaderCell ("Denied Rights");
      }
      htmlTagWriter.Tags.trEnd ();
    }



    private void WriteTableDataAddendum (Object addendum) // TODO AE: Use "object".
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
      { // TODO AE: Braces.
        WriteTableDataAddendum (rowCount);
      }
      htmlTagWriter.Tags.tdEnd ();
    }


    // TODO AE: Changed unused return value to void.
    private HtmlTagWriter WriteRowspanAttribute (int rowCount)
    {
      if (rowCount > 0)
      { // TODO AE: Braces.
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
      { // TODO AE: Braces.
        WriteTableDataAddendum (rowCount);
      }
      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForStates (AclExpansionEntry aclExpansionEntry)
    {
      htmlTagWriter.Tags.td ();
      WriteTableDataBodyForSingleState(aclExpansionEntry);
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataBodyForSingleState (AclExpansionEntry aclExpansionEntry)
    {
      if (aclExpansionEntry.AccessControlList is StatelessAccessControlList)
      {
        htmlTagWriter.Value (StatelessAclStateHtmlText);
      }
      else
      {
        // TODO AE: Replace comment by extracting method and naming it appropriately.
        // Get the states by flattening the StateCombinations of the AccessControlEntry ACL 
        //var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates()).OrderBy (x => x.DisplayName).ToArray();
        var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).OrderBy (x => x.DisplayName);
        
        // TODO AE: Is this semantically equivalent to the following?
        // TODO AE: from combination in aclExpansionEntry.StateCombinations
        // TODO AE: from state in combination.GetStates()
        // TODO AE: orderby state.DisplayName
        // TODO AE: select state

        //htmlTagWriter.Value (aclExpansionEntry.StateCombinations + ", "); // !!!!!!!! SPIKE ONLY !!!!!!!!! // TODO AE: Remove it.

        if (!stateDefinitions.Any ())
        { // TODO AE: Braces.
          htmlTagWriter.Value (AclWithNoAssociatedStatesHtmlText);
        }
        else
        {
          bool firstElement = true;
          foreach (StateDefinition stateDefiniton in stateDefinitions)
          {
            if (!firstElement)
            {
              htmlTagWriter.Value (", ");
            }

            string stateName = Settings.ShortenNames ? stateDefiniton.ShortName() : stateDefiniton.DisplayName;
            //string stateName = stateDefiniton.DisplayName + "(" + stateDefiniton.Name + "," + stateDefiniton.Value + ")";

            htmlTagWriter.Value (stateName);
            firstElement = false;
          }
        }
      }
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
        { // TODO AE: Braces
          htmlTagWriter.Value (", ");
        }
        htmlTagWriter.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForBodyConditions (AclExpansionAccessConditions accessConditions)
    {
      WriteTableDataForBooleanCondition (accessConditions.IsOwningUserRequired);
      
      //WriteTableDataForBooleanCondition (conditions.HasOwningGroupCondition);
      WriteTableDataForOwningGroupCondition (accessConditions);
      
      //WriteTableDataForBooleanCondition (conditions.HasOwningTenantCondition);
      WriteTableDataForOwningTenantCondition (accessConditions);

      WriteTableDataForAbstractRoleCondition(accessConditions);
    }

    private void WriteTableDataForAbstractRoleCondition (AclExpansionAccessConditions accessConditions)
    {
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (accessConditions.IsAbstractRoleRequired ? accessConditions.AbstractRole.DisplayName : "");
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForOwningGroupCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.GroupHierarchyCondition == GroupHierarchyCondition.Undefined && conditions.OwningGroup != null);
      htmlTagWriter.Tags.td();
      htmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningGroup = conditions.OwningGroup;
      if (owningGroup != null)
      { // TODO AE: Braces
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


    private void WriteTableDataForOwningTenantCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.TenantHierarchyCondition == TenantHierarchyCondition.Undefined && conditions.OwningTenant != null);
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningTenant = conditions.OwningTenant;
      if (owningTenant != null)
      { // TODO AE: Braces
        htmlTagWriter.Value (owningTenant.DisplayName);
      }

      var tenantHierarchyCondition = conditions.TenantHierarchyCondition;
      if ((tenantHierarchyCondition & TenantHierarchyCondition.Parent) != 0)
      {
        //htmlTagWriter.Value (",");
        htmlTagWriter.Tags.br ();
        htmlTagWriter.Value ("or its parents");
      }

      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForBooleanCondition (bool required)
    {
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (required ? "X" : ""); // TODO AE: Test missing for one of these cases
      htmlTagWriter.Tags.tdEnd ();
    }



    private void WriteTableBody (AclExpansionTree aclExpansionTree)
    {
      foreach (var userNode in aclExpansionTree.Tree)
      {
        WriteTableBody_ProcessUser(userNode);
      }
    }

    private void WriteTableBody_ProcessUser (AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>> userNode)
    {
      WriteTableDataWithRowCount (userNode.Key.DisplayName, userNode.NumberLeafNodes);
  
      foreach (var roleNode in userNode.Children)
      {// TODO AE: Braces
        WriteTableBody_ProcessRole(roleNode);
      }
    }

    private void WriteTableBody_ProcessRole (AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>> roleNode)
    {
      WriteTableDataForRole (roleNode.Key, roleNode.NumberLeafNodes);
 
      foreach (var classNode in roleNode.Children)
      {// TODO AE: Braces
        WriteTableBody_ProcessClass(classNode);
      }
    }

    private void WriteTableBody_ProcessClass (AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> classNode)
    {
      if (classNode.Key != null)
      {
        string className = Settings.ShortenNames ? classNode.Key.ShortName () : classNode.Key.DisplayName;
        WriteTableDataWithRowCount (className, classNode.NumberLeafNodes);
      }
      else
      {// TODO AE: Braces
        WriteTableDataWithRowCount ("_NO_CLASSES_DEFINED_", classNode.NumberLeafNodes);
      }

      // AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>
      WriteTableBody_ProcessStates(classNode.Children);
    }

#if(false) // TODO AE: Remove conditional symbol, remove #else part
    private void WriteTableBody_ProcessStates (IList<AclExpansionEntry> states)
    {
      // States Output
      foreach (var aclExpansionEntry in states)
      {
        WriteTableRowBeginIfNotInTableRow ();

        WriteTableDataForStates (aclExpansionEntry);
        WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
        WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);
        if (Settings.OutputDeniedRights)
        { // TODO AE: Braces
          WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
        }

        WriteTableRowEnd ();
      }
    }
#else
    //private void WriteTableBody_ProcessStates (IList<AclExpansionEntry> states)
    private void WriteTableBody_ProcessStates (IList<AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> states)
    {
      //var statesGroupedByOnlyDiffersInStates = states.GroupBy (aee => aee, aee => aee, AclExpansionEntryIgnoreStateEqualityComparer);

      //var statesGroupedByOnlyDiffersInStates = states.GroupBy (aee => aee, aee => aee, 
      //  AclExpansionEntryIgnoreStateEqualityComparer).Select(x => AclExpansionTreeNode.New (x.Key,x.Count(),x.ToList()));

      //var xxx = from entry in states
      //          orderby entry.User.DisplayName
      //          group entry by entry.User
      //          into grouping
      //          select AclExpansionTreeNode.New (grouping.Key, grouping.Count(), grouping.ToList());


      //var xxx2 =
      //    states.OrderBy (entry => entry.User.DisplayName).GroupBy (entry => entry.User).Select (
      //        grouping => AclExpansionTreeNode.New (grouping.Key, grouping.Count(), grouping.ToList()));


      // TODO: Fix rowspan to take reduced number of table output rows into account:
      // Move grouping to AclExpansionTree (alas need to adapt all "AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, Ac...")
      // for this in this class).

      // States Output
      foreach (var aclExpansionTreeNode in states)
      {
        WriteTableRowBeginIfNotInTableRow ();

        // Write all states combined into one cell
        WriteTableDataForStates (aclExpansionTreeNode.Children);

        AclExpansionEntry aclExpansionEntry = aclExpansionTreeNode.Key;
        //WriteTableDataForStates (aclExpansionEntry); // TEST !!!!
        WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
        WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);
        if (Settings.OutputDeniedRights)
        {
          WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
        }

        WriteTableRowEnd ();
      }
    }


    //private void WriteTableDataForStates (IGrouping<AclExpansionEntry,AclExpansionEntry> aclExpansionEntryGrouping)
    private void WriteTableDataForStates (IList<AclExpansionEntry> aclExpansionEntriesWhichOnlyDiffersInStates)
    {
      htmlTagWriter.Tags.td ();

      bool firstElement = true;

      //To.ConsoleLine.e ("number of elements in statesGroupedByOnlyDiffersInStates: ", aclExpansionEntriesWhichOnlyDiffersInStates.Count ());

      foreach (AclExpansionEntry aclExpansionEntry in aclExpansionEntriesWhichOnlyDiffersInStates)
      {
        if (!firstElement)
        {
          htmlTagWriter.Value ("; ");
        }

        WriteTableDataBodyForSingleState (aclExpansionEntry);
        firstElement = false;
      }
      htmlTagWriter.Tags.tdEnd ();
    }
 

#endif

  }
}



