// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Collections.Generic;
using System.IO;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Text.StringExtensions;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="IAclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>
  /// as a single HTML table.
  /// </summary>
  public class AclExpansionHtmlWriter : IAclExpansionWriter
  {
    private readonly AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();

    private readonly AclExpansionHtmlWriterImplementation _implementation;

    public AclExpansionHtmlWriter (TextWriter textWriter, bool indentXml, AclExpansionHtmlWriterSettings settings)
    {
      _settings = settings;
      _implementation = new AclExpansionHtmlWriterImplementation (textWriter, indentXml, settings);
    }
   

    public AclExpansionHtmlWriterImplementation Implementation
    {
      get { return _implementation; }
    }


    public void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
    
      var aclExpansionTree = new AclExpansionTree (aclExpansion);

      Implementation.WritePageStart ("re-motion ACL Expansion");
      Implementation.WriteTableStart ("remotion-ACL-expansion-table");
      WriteTableHeaders ();
      WriteTableBody (aclExpansionTree);
      Implementation.WriteTableEnd ();
      Implementation.WritePageEnd ();
    }


    private void WriteTableHeaders ()
    {
      Implementation.HtmlTagWriter.Tags.tr ();
      Implementation.WriteHeaderCell ("User");
      Implementation.WriteHeaderCell ("Role");
      Implementation.WriteHeaderCell ("Class");
      Implementation.WriteHeaderCell ("States");
      Implementation.WriteHeaderCell ("User Must Own");
      Implementation.WriteHeaderCell ("Owning Group Equals");
      Implementation.WriteHeaderCell ("Owning Tenant Equals");
      Implementation.WriteHeaderCell ("User Must Have Abstract Role");
      Implementation.WriteHeaderCell ("Access Rights");
      if (_settings.OutputDeniedRights)
      {
        Implementation.WriteHeaderCell ("Denied Rights");
      }
      Implementation.HtmlTagWriter.Tags.trEnd ();
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
      Implementation.WriteTableDataWithRowCount (userNode.Key.DisplayName, userNode.NumberLeafNodes);
  
      foreach (var roleNode in userNode.Children)
      {
        WriteTableBody_ProcessRole(roleNode);
      }
    }

    private void WriteTableBody_ProcessRole (AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>> roleNode)
    {
      Implementation.WriteTableDataForRole (roleNode.Key, roleNode.NumberLeafNodes);
 
      foreach (var classNode in roleNode.Children)
      {
        WriteTableBody_ProcessClass(classNode);
      }
    }

    private void WriteTableBody_ProcessClass (AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> classNode)
    {
      if (classNode.Key != null)
      {
        string className = _settings.ShortenNames ? classNode.Key.ShortName () : classNode.Key.DisplayName;
        Implementation.WriteTableDataWithRowCount (className, classNode.NumberLeafNodes);
      }
      else
      {
        Implementation.WriteTableDataWithRowCount ("_NO_CLASSES_DEFINED_", classNode.NumberLeafNodes);
      }

      WriteTableBody_ProcessStates(classNode.Children);
    }


    private void WriteTableBody_ProcessStates (IList<AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> states)
    {
      // States Output
      foreach (var aclExpansionTreeNode in states)
      {
        Implementation.WriteTableRowBeginIfNotInTableRow ();

        // Write all states combined into one cell
        WriteTableDataForStates (aclExpansionTreeNode.Children);

        AclExpansionEntry aclExpansionEntry = aclExpansionTreeNode.Key;
        Implementation.WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
        Implementation.WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);
        if (_settings.OutputDeniedRights)
        {
          Implementation.WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
        }

        Implementation.WriteTableRowEnd ();
      }
    }


    private void WriteTableDataForStates (IList<AclExpansionEntry> aclExpansionEntriesWhichOnlyDiffersInStates)
    {
      Implementation.HtmlTagWriter.Tags.td ();

      bool firstElement = true;

      foreach (AclExpansionEntry aclExpansionEntry in aclExpansionEntriesWhichOnlyDiffersInStates)
      {
        if (!firstElement)
        {
          Implementation.HtmlTagWriter.Value ("; ");
        }

        Implementation.WriteTableDataBodyForSingleState (aclExpansionEntry);
        firstElement = false;
      }
      Implementation.HtmlTagWriter.Tags.tdEnd ();
    }

  }

  public static class StateDefinitionExtensions
  {
    public static string ShortName (this StateDefinition stateDefinition)
    {
      return stateDefinition.Name.LeftUntilChar ('|');
      //return stateDefinition.DisplayName;
    }
  }

  public static class SecurableClassDefinitionExtensions
  {
    public static string ShortName (this SecurableClassDefinition securableClassDefinition)
    {
      return securableClassDefinition.Name.RightUntilChar ('.');
      //return securableClassDefinition.DisplayName;
    }
  }

}



