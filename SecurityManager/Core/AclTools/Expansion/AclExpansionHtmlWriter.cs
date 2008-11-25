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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Remotion.Development.UnitTesting.ObjectMother;
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
    private string _aclWithNoAssociatedStatesHtmlText = "(no associated states)";

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
      //WriteAclExpansion ();
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
      //WriteHeaderCell ("Tenant Must Own");
      WriteHeaderCell ("Owning Tenant Equals");
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
        // Get the states by flattening the StateCombinations of the AccessControlEntry ACL 
        //var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates()).OrderBy (x => x.DisplayName).ToArray();
        var stateDefinitions = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).OrderBy (x => x.DisplayName);

        //htmlTagWriter.Value (aclExpansionEntry.StateCombinations + ", "); // !!!!!!!! SPIKE ONLY !!!!!!!!!

        if (!stateDefinitions.Any ())
        {
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
        {
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


    private void WriteTableDataForOwningTenantCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.TenantHierarchyCondition == TenantHierarchyCondition.Undefined && conditions.OwningTenant != null);
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningTenant = conditions.OwningTenant;
      if (owningTenant != null)
      {
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

#if(true)
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
        {
          WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
        }

        WriteTableRowEnd ();
      }
    }
#else
    private void WriteTableBody_ProcessStates (IList<AclExpansionEntry> states)
    {
      var statesGroupedByOnlyDiffersInStates = states.GroupBy(aee => aee,aee => aee,new AclExpansionEntryIgnoreStateEqualityComparer());

      // States Output
      foreach (var aclExpansionEntryGrouping in statesGroupedByOnlyDiffersInStates)
      {
        WriteTableRowBeginIfNotInTableRow ();

        // Write all states combined into one cell
        WriteTableDataForStates (aclExpansionEntryGrouping);

        AclExpansionEntry aclExpansionEntry = aclExpansionEntryGrouping.Key; 
        WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
        WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);
        if (Settings.OutputDeniedRights)
        {
          WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
        }

        WriteTableRowEnd ();
      }
    }



    private void WriteTableDataForStates (IGrouping<AclExpansionEntry,AclExpansionEntry> aclExpansionEntryGrouping)
    {
      htmlTagWriter.Tags.td ();

      bool firstElement = true;
      foreach (AclExpansionEntry aclExpansionEntry in aclExpansionEntryGrouping)
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

 

    public class AclExpansionEntryIgnoreStateEqualityComparer : IEqualityComparer<AclExpansionEntry>
    {
      private static readonly EqualsAndGetHashCodeSupplier<AclExpansionEntry> _equalsAndGetHashCode = 
        new EqualsAndGetHashCodeSupplier<AclExpansionEntry>(
          a => a.AccessControlList, a => a.Class, a => a.Role, a => a.User,
          
          a => a.AccessConditions.AbstractRole,
          a => a.AccessConditions.GroupHierarchyCondition,
          a => a.AccessConditions.IsOwningUserRequired,
          a => a.AccessConditions.OwningGroup,
          a => a.AccessConditions.OwningTenant,
          a => a.AccessConditions.TenantHierarchyCondition,

          a => new EnumerableEqualsWrapper<AccessTypeDefinition>(a.AllowedAccessTypes),
          a => new EnumerableEqualsWrapper<AccessTypeDefinition>(a.DeniedAccessTypes)

      );


      public bool Equals (AclExpansionEntry x, AclExpansionEntry y)
      {
        return _equalsAndGetHashCode.Equals (x, y);

        ////bool referencesEqual = x.AccessControlList == y.AccessControlList
        //bool referencesEqual = Compare.It (x, y, a => a.AccessControlList, a => a.Class, a => a.Role, a => a.User);
        //if (!referencesEqual)
        //{
        //  return false;
        //}

        //bool accessConditionsEqual = Compare.It (
        //    x.AccessConditions,
        //    y.AccessConditions,
        //    a => a.AbstractRole,
        //    a => a.GroupHierarchyCondition,
        //    a => a.IsOwningUserRequired,
        //    a => a.OwningGroup,
        //    a => a.OwningTenant,
        //    a => a.TenantHierarchyCondition);

        //throw new System.NotImplementedException();
      }

      public int GetHashCode (AclExpansionEntry x)
      {
        return _equalsAndGetHashCode.GetHashCode (x);
        //return EqualityUtility.GetRotatedHashCode (x.AccessConditions);
      }
    }


    public class EqualsAndGetHashCodeSupplier<T> where T : class
    {
      private readonly Func<T, object>[] _classMembersUsedForComparison;

      public EqualsAndGetHashCodeSupplier(params Func<T, object>[] membersUsedForComparison)
      {
        _classMembersUsedForComparison = membersUsedForComparison;
      }

      public bool Equals (T x, T y)
      {
        if (x == null || y == null)
        {
          return false;
        }
        foreach (var member in _classMembersUsedForComparison)
        {
          if (!(member (x) == member (y)))
          //if (!member (x).Equals (member (y)))
          {
            return false;
          }
        }
        return true;
      }

      public int GetHashCode (T x)
      {
        ArgumentUtility.CheckNotNull ("x", x);
        //if (x == null)
        //{
        //  return 0;
        //}
        return EqualityUtility.GetRotatedHashCode (_classMembersUsedForComparison.Select(m => m(x)));
      }
    }


    public class EnumerableEqualsWrapper<TElement> : IEnumerable<TElement>

    {
      private readonly IEnumerable<TElement> _enumerable;

      public EnumerableEqualsWrapper(IEnumerable<TElement> enumerable)
      {
        _enumerable = enumerable;
      }



      public override bool Equals(object obj)
      {
        if (ReferenceEquals (null, obj))
          return false;
        if (ReferenceEquals (this, obj))
          return true;

        if(obj is EnumerableEqualsWrapper<TElement>)
        {
          return obj.Equals (_enumerable);
        }

        if(!(obj is IEnumerable<TElement>))
        {
          return false;
        }

        IEnumerable<TElement> enumerable = (IEnumerable<TElement>) obj;
        IEnumerator<TElement> enumerator0 = _enumerable.GetEnumerator();
        IEnumerator<TElement> enumerator1 = enumerable.GetEnumerator();
        while (true)
        {
          bool hasNext0 = enumerator0.MoveNext();
          bool hasNext1 = enumerator1.MoveNext();

          if (hasNext0 && hasNext1)
          {
            // Both enumerators have next element => continue comparing
            if (!enumerator0.Current.Equals (enumerator1.Current))
            {
              return false;
            }
          }
          else
          {
            // Only if both enumerators are false are the sequences equal
            return hasNext0 == hasNext1;
          }

          //if(hasNext0 != hasNext1)
          //{
          //  // Number of elements not equal => sequence not equal
          //  return false;
          //}
          //if(!hasNext0)
          //{
          //  // Both enumerators don't have more elements => sequence equal
          //  return true;
          //}
          //if(!enumerator0.Current.Equals(enumerator1.Current))
          //{
          //  return false;
          //}
        }
      }

      public IEnumerator<TElement> GetEnumerator ()
      {
        return _enumerable.GetEnumerator();
      }
      
      IEnumerator IEnumerable.GetEnumerator ()
      {
        return GetEnumerator();
      }

      
      public override int GetHashCode ()
      {
        return EqualityUtility.GetRotatedHashCode(_enumerable);
      }
    }



    //private static class Compare
    //{
    //  public static bool It<T> (T x, T y, params Func<T, object>[] members)
    //  {
    //    foreach (var member in members)
    //    {
    //      if (!member (x).Equals (member (y)))
    //      {
    //        return false;
    //      }
    //    }
    //    return true;
    //  }
    //}
#endif

  }
}



