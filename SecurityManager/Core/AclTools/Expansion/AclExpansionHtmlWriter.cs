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

      WriteStartPage (html);

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

    private HtmlWriter WriteStartPage (HtmlWriter html)
    {
      //var html = _htmlWriter;

      // DOCTYPE
      html.XmlWriter.WriteDocType ("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
      // HTML
      html.Tag ("html");
      // HEAD
      html.Tag ("head");
      // TITLE
      html.Tag ("title");
      html.Value ("re-motion ACL Expansion");
      html.TagEnd ("title");

      // STYLE
      html.Tag ("style");
      html.Value ("@import \"AclExpansion.css\";");
      html.TagEnd ("style");
      html.TagEnd ("head");

      // BODY
      html.Tag ("body");
      return html;
    }


    void WriteHeaderCell (string columnName)
    {
      var html = _htmlWriter;
      html.td ().a ("class", "header");
      html.Value (columnName);
      html.tdEnd ();
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

    void WriteTableDataWithAddendum (string value, Object addendum)
    {
      var html = _htmlWriter;
      html.td ();
      WriteTableDataAddendum (addendum);
      html.Value (value);
      html.tdEnd ();
    }


    void WriteTableDataWithRowCount (string value, int rowCount)
    {
      var html = _htmlWriter;
      html.td ();
      WriteRowspanAttribute(rowCount);
      html.Value (value);
      WriteTableDataAddendum (rowCount);
      html.tdEnd ();
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
      var html = _htmlWriter;
      html.td ();
      WriteRowspanAttribute (rowCount);
      html.Value (role.Group.DisplayName);
      html.Value (", ");
      html.Value (role.Position.DisplayName);
      WriteTableDataAddendum (rowCount);
      html.tdEnd ();
    }
  

    void tdBodyStates (params StateDefinition[] stateDefinitions)
    {
      var html = _htmlWriter;
      html.td ();
      bool firstElement = true;
      foreach (StateDefinition stateDefiniton in stateDefinitions)
      {
        if (!firstElement)
        {
          html.br ();
        }
        html.Value (stateDefiniton.DisplayName);
        firstElement = false;
      }
      html.tdEnd ();
    }

    private void tdBodyAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      var html = _htmlWriter;
      html.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitions)
      {
        if (!firstElement)
        {
          html.Value (", ");
        }
        html.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      html.tdEnd ();
    }

    private void tdBodyConditions (AclExpansionAccessConditions conditions)
    {
      var html = _htmlWriter;

      tdBodyBooleanCondition (conditions.IsOwningUserRequired);
      tdBodyBooleanCondition (conditions.IsOwningGroupRequired);
      tdBodyBooleanCondition (conditions.IsOwningTenantRequired);

      html.td ();
      html.Value (conditions.IsAbstractRoleRequired ? conditions.AbstractRole.DisplayName : "");
      html.tdEnd ();
    }

    private void tdBodyBooleanCondition (bool required)
    {
      var html = _htmlWriter;
      _htmlWriter.td ();
      _htmlWriter.Value (required ? "X" : "");
      _htmlWriter.tdEnd ();
    }

    // Spike
    private void WriteTableBodySpike (HtmlWriter html, List<AclExpansionEntry> aclExpansion)
    {
      // TODO: Share with AclExpansionConsoleTextWriter
      var aclExpansionHierarchy =
          from expansion in aclExpansion
          group expansion by expansion.User
            into userGroup
            select new
            {
              User = userGroup.Key,
              RoleGroup =
                from user in userGroup
                group user by user.Role
                  into roleGroup
                  select new
                  {
                    Role = roleGroup.Key,
                    ClassGroup =
                    from role in roleGroup
                    group role by role.Class
                      into classGroup
                      select new
                      {
                        Class = classGroup.Key,
                        StatesGroup = classGroup
                      }
                  }
            };

      foreach (var userGroup in aclExpansionHierarchy)
      {
        bool newUserRow = true;
        var userName = userGroup.User.DisplayName;
        int userRowCount = userGroup.RoleGroup.SelectMany(x => x.ClassGroup).SelectMany(x => x.StatesGroup).Count();
        foreach (var roleGroup in userGroup.RoleGroup)
        {
          bool newRoleRow = true;
          var role = roleGroup.Role;
          //int roleRowCount = roleGroup.ClassGroup.Count ();
          int roleRowCount = roleGroup.ClassGroup.SelectMany (x => x.StatesGroup).Count ();
          foreach (var classGroup in roleGroup.ClassGroup)
          {
            bool newClassRow = true;
            var className = classGroup.Class.DisplayName;
            int classRowCount = classGroup.StatesGroup.Count ();
            
            foreach (var entry in classGroup.StatesGroup)
            {
              var stateArray = entry.StateCombinations.SelectMany (x => x.GetStates ()).ToArray ();

              _htmlWriter.tr();

              if (newUserRow)
              {
                newUserRow = false;
                WriteTableDataWithRowCount (userName, userRowCount);
              }

              if (newRoleRow)
              {
                newRoleRow = false;
                WriteTableDataForRole (role, roleRowCount);
              }

              if (newClassRow)
              {
                newClassRow = false;
                WriteTableDataWithRowCount (className, classRowCount);
              }

              //WriteTableDataWithAddendum (userName, userRowCount);
              //WriteTableDataForRole (role, roleRowCount);
              //WriteTableDataWithRowCount (className, classRowCount);

              tdBodyStates (stateArray);
              tdBodyConditions (entry.AccessConditions);
              tdBodyAccessTypes (entry.AccessTypeDefinitions);
              _htmlWriter.trEnd();

            }
          }
        }
      }
    }






    //--------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------






    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var aclExpansionUserGrouping = GetAclExpansionGrouping (aclExpansion, (aee => aee.User));

      foreach (var userGroup in aclExpansionUserGrouping)
      {
        //bool newUserRow = true;
        var userName = userGroup.Key.DisplayName;
        int userRowCount = userGroup.Items.Count();

        WriteTableRowBeginIfNotInTableRow();
        WriteTableDataWithRowCount (userName, userRowCount);

        var aclExpansionRoleGrouping = GetAclExpansionGrouping (userGroup, (x => x.Role));

        foreach (var roleGroup in aclExpansionRoleGrouping)
        {
          bool newRoleRow = true;

          var role = roleGroup.Key;
          int roleRowCount = roleGroup.Items.Count ();


          var aclExpansionClassGrouping = GetAclExpansionGrouping (roleGroup, (x => x.Class));

          foreach (var classGroup in aclExpansionClassGrouping)
          {
            bool newClassRow = true;
            var className = classGroup.Key.DisplayName;
            int classRowCount = classGroup.Items.Count ();

            foreach (var entry in classGroup.Items)
            {
              var stateArray = entry.StateCombinations.SelectMany (x => x.GetStates ()).ToArray ();

              WriteTableRowBeginIfNotInTableRow();

              //if (newUserRow)
              //{
              //  newUserRow = false;
              //  WriteTableDataWithRowCount (userName, userRowCount);
              //}

              if (newRoleRow)
              {
                newRoleRow = false;
                WriteTableDataForRole (role, roleRowCount);
              }

              if (newClassRow)
              {
                newClassRow = false;
                WriteTableDataWithRowCount (className, classRowCount);
              }

              //WriteTableDataWithAddendum (userName, userRowCount);
              //WriteTableDataForRole (role, roleRowCount);
              //WriteTableDataWithRowCount (className, classRowCount);

              tdBodyStates (stateArray);
              tdBodyConditions (entry.AccessConditions);
              tdBodyAccessTypes (entry.AccessTypeDefinitions);

              WriteTableRowEnd();

            }
          }
        }
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

  public class LinqGroup<TKey, Tentry>
  {
    public LinqGroup (IGrouping<TKey, Tentry> items)
    {
      Items = items;
    }

    public TKey Key
    {
      get { return Items.Key; }
    }
    public IGrouping<TKey, Tentry> Items { get; private set; }
  }

}


namespace ObjectMother
{
  public class LinqGroup
  {
    public static Remotion.SecurityManager.AclTools.Expansion.LinqGroup<TKey, Tentry> New<TKey, Tentry> (IGrouping<TKey, Tentry> items)
    {
      return new Remotion.SecurityManager.AclTools.Expansion.LinqGroup<TKey, Tentry> (items);
    }
  }
}
