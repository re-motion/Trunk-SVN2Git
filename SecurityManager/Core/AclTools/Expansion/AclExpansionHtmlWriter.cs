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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionHtmlWriter : AclExpansionWriter
  {
    private readonly HtmlWriter _htmlWriter;

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
      WriteTableBody (html, aclExpansion);
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
      tdHeader ("User");
      tdHeader ("Role");
      tdHeader ("Class");
      tdHeader ("States");
      tdHeader ("User Must Own");
      tdHeader ("Group Must Own");
      tdHeader ("Tenant Must Own");
      tdHeader ("User Must Have Abstract Role");
      tdHeader ("Access Rights");
      html.trEnd ();
    }

    private void WriteEndPage (HtmlWriter html)
    {
      html.end ("body");
      html.end ("html");

      html.Close ();
    }

    private HtmlWriter WriteStartPage (HtmlWriter html)
    {
      //var html = _htmlWriter;

      // DOCTYPE
      html.XmlWriter.WriteDocType ("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
      // HTML
      html.e ("html");
      // HEAD
      html.e ("head");
      // TITLE
      html.e ("title");
      html.value ("re-motion ACL Expansion");
      html.end ("title");

      // STYLE
      html.e ("style");
      html.value ("@import \"AclExpansion.css\";");
      html.end ("style");
      html.end ("head");

      // BODY
      html.e ("body");
      return html;
    }


    void tdHeader (string columnName)
    {
      var html = _htmlWriter;
      html.td ().a ("class", "header");
      html.value (columnName);
      html.tdEnd ();
    }

    void WriteTabldeDataAddendum (Object addendum)
    {
      if (addendum != null)
      {
        _htmlWriter.value (" (");
        _htmlWriter.value (addendum);
        _htmlWriter.value (") ");
      }
    }

    void tdBody (string value, int rowCount)
    {
      var html = _htmlWriter;
      html.td ();
      if (rowCount > 0)
      {
        html.a ("rowspan", Convert.ToString (rowCount));
      }
      html.value (value);
      WriteTabldeDataAddendum (rowCount);
      html.tdEnd ();
    }

    private void tdBodyRole (Role role, Object addendum)
    {
      var html = _htmlWriter;
      html.td ();
      html.value (role.Group.DisplayName);
      html.value (", ");
      html.value (role.Position.DisplayName);
      WriteTabldeDataAddendum (addendum);
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
        html.value (stateDefiniton.DisplayName);
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
          html.value (", ");
        }
        html.value (accessTypeDefinition.DisplayName);
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
      html.value (conditions.IsAbstractRoleRequired ? conditions.AbstractRole.DisplayName : "");
      html.tdEnd ();
    }

    private void tdBodyBooleanCondition (bool required)
    {
      var html = _htmlWriter;
      html.td ();
      html.value (required ? "X" : "");
      html.tdEnd ();
    }

    // Spike
    // TODO: Introduce rowspan|s 
    private void WriteTableBody (HtmlWriter html, List<AclExpansionEntry> aclExpansion)
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
                        ClassGroup = classGroup
                      }
                  }
            };

      foreach (var userGroup in aclExpansionHierarchy)
      {
        var userName = userGroup.User.DisplayName;
        int userRowCount = userGroup.RoleGroup.SelectMany(x => x.ClassGroup).Count();
        bool newUserRow = true;
        foreach (var roleGroup in userGroup.RoleGroup)
        {
          var role = roleGroup.Role;
          int roleRowCount = roleGroup.ClassGroup.Count ();
          foreach (var classGroup in roleGroup.ClassGroup)
          {
            var className = classGroup.Class.DisplayName;
            foreach (var aclExpansionEntry in classGroup.ClassGroup)
            {
              var stateArray = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).ToArray ();
              _htmlWriter.tr ();
              if (newUserRow)
              {
                tdBody (userName, userRowCount);
              }
              tdBody (userName, 0);
              
              tdBodyRole (role, roleRowCount);
              tdBody (className,1);
              tdBodyStates (stateArray);
              tdBodyConditions (aclExpansionEntry.AccessConditions);
              tdBodyAccessTypes (aclExpansionEntry.AccessTypeDefinitions);
              _htmlWriter.trEnd ();

              newUserRow = false;
            }
          }
        }
      }
    }
  }


  // Spike
  public class HtmlWriter : IDisposable
  {
    private readonly XmlWriter _xmlWriter;
    private readonly Stack<string> _openElementStack = new Stack<string>();


    public HtmlWriter (TextWriter textWriter, bool indentXml)
    {
      _xmlWriter = CreateXmlWriter (textWriter, indentXml);
    }

    public HtmlWriter (XmlWriter xmlWriter)
    {
      _xmlWriter = xmlWriter;
    }

    public XmlWriter XmlWriter
    {
      get { return _xmlWriter; }
    }

    public HtmlWriter e (string elementName)
    {
      _xmlWriter.WriteStartElement (elementName);
      _openElementStack.Push (elementName);
      return this;
    }

    public HtmlWriter end (string elementName)
    {
      string ElementNameExpected = _openElementStack.Pop();
      if (ElementNameExpected != elementName)
      {
        //_xmlWriter.Settings.ConformanceLevel = ConformanceLevel.Fragment;
        //_xmlWriter.Close();
        throw new XmlException (String.Format ("Wrong closing tag in XML: Expected {0} but was {1}:\n{2}", ElementNameExpected, elementName, _xmlWriter.ToString()));
      }
      _xmlWriter.WriteEndElement ();
      return this;
    }

    public HtmlWriter a (string attributeName, string attributeValue)
    {
      _xmlWriter.WriteAttributeString (attributeName,attributeValue);
      return this;
    }

    public HtmlWriter table ()
    {
      e ("table");
      return this;
    }

    public HtmlWriter tableEnd ()
    {
      end ("table");
      return this;
    }

    public HtmlWriter tr ()
    {
      e ("tr");
      return this;
    }

    public HtmlWriter trEnd ()
    {
      end ("tr");
      return this;
    }

    public HtmlWriter td ()
    {
      e ("td");
      return this;
    }

    public HtmlWriter tdEnd ()
    {
      end ("td");
      return this;
    }

    public XmlWriter CreateXmlWriter (TextWriter textWriter, bool indent)
    {
      XmlWriterSettings settings = new XmlWriterSettings ();

      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      settings.NewLineOnAttributes = false;
      //settings.ConformanceLevel = ConformanceLevel.Fragment;

      return XmlWriter.Create (textWriter, settings);
    }

    public void value (string s)
    {
      _xmlWriter.WriteValue(s);
    }

    public void value (object obj)
    {
      _xmlWriter.WriteValue (obj);
    }

    public void Close ()
    {
      Dispose ();
    }

    public void Dispose ()
    {
      _xmlWriter.Close();
    }

    public void br ()
    {
      _xmlWriter.WriteStartElement ("br");
      _xmlWriter.WriteEndElement ();
    }
  }

}