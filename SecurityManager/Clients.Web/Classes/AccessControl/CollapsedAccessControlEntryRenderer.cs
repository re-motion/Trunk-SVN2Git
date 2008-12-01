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
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes.AccessControl
{
  /// <summary>
  /// The <see cref="CollapsedAccessControlEntryRenderer"/> type is responsible for generating the collapsed view on an <see cref="AccessControlEntry"/>
  /// </summary>
  public class CollapsedAccessControlEntryRenderer
  {
    private readonly AccessControlEntry _accessControlEntry;

    public CollapsedAccessControlEntryRenderer (AccessControlEntry accessControlEntry)
    {
      ArgumentUtility.CheckNotNull ("currentAccessControlEntry", accessControlEntry);
      _accessControlEntry = accessControlEntry;
    }

    public void Render (HtmlTextWriter writer, Control container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      RenderTenant(writer,container);
      RenderGroup (writer, container);
      RenderUser (writer, container);
      RenderAbstractRole (writer, container);
      RenderPermissions (writer, container);
    }

    public decimal GetColumnCount ()
    {
      return 4 + _accessControlEntry.AccessControlList.Class.AccessTypes.Count;
    }

    public AccessControlEntry AccessControlEntry
    {
      get { return _accessControlEntry; }
    }

    private void RenderTenant (HtmlTextWriter writer, Control container)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      switch (_accessControlEntry.TenantCondition)
      {
        case TenantCondition.None:
          break;
        case TenantCondition.OwningTenant:
          RenderTenantHierarchyIcon (writer, container);
          RenderPropertyPathString (writer, "TenantCondition");
          break;
        case TenantCondition.SpecificTenant:
          RenderTenantHierarchyIcon (writer, container);
          RenderPropertyPathString (writer, "SpecificTenant.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      writer.RenderEndTag ();
    }

    private void RenderGroup (HtmlTextWriter writer, Control container)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      switch (_accessControlEntry.GroupCondition)
      {
        case GroupCondition.None:
          break;
        case GroupCondition.OwningGroup:
          RenderGroupHierarchyIcon (writer, container);
          writer.RenderBeginTag (HtmlTextWriterTag.Em);
          RenderPropertyPathString (writer, "GroupCondition");
          writer.RenderEndTag();
          break;
        case GroupCondition.SpecificGroup:
          RenderGroupHierarchyIcon (writer, container);
          writer.Write (HtmlUtility.HtmlEncode ("Group"));
          writer.Write (" ");
          writer.RenderBeginTag (HtmlTextWriterTag.Em);
          RenderPropertyPathString (writer, "SpecificGroup.ShortName");
          writer.RenderEndTag();
          break;
        case GroupCondition.BranchOfOwningGroup:
          writer.Write (HtmlUtility.HtmlEncode ("Same"));
          writer.Write (" ");
          writer.RenderBeginTag (HtmlTextWriterTag.Em);
          RenderPropertyPathString (writer, "SpecificGroupType.DisplayName");
          writer.RenderEndTag();
          break;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          writer.Write (HtmlUtility.HtmlEncode ("GT"));
          writer.Write (" ");
          writer.RenderBeginTag (HtmlTextWriterTag.Em);
          RenderPropertyPathString (writer, "SpecificGroupType.DisplayName");
          writer.RenderEndTag();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      writer.RenderEndTag ();
    }

    private void RenderUser (HtmlTextWriter writer, Control container)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      switch (_accessControlEntry.UserCondition)
      {
        case UserCondition.None:
          break;
        case UserCondition.Owner:
          RenderPropertyPathString (writer, "UserCondition");
          break;
        case UserCondition.SpecificUser:
          RenderPropertyPathString (writer, "SpecificUser.DisplayName");
          break;
        case UserCondition.SpecificPosition:
          RenderPropertyPathString (writer, "SpecificPosition.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      writer.RenderEndTag ();
    }

    private void RenderAbstractRole (HtmlTextWriter writer, Control container)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderPropertyPathString (writer, "SpecificAbstractRole.DisplayName");
      writer.RenderEndTag ();
    }

    private void RenderPermissions (HtmlTextWriter writer, Control container)
    {
      var grantedIcon = new IconInfo (GetIconUrl ("PermissionGranted.gif", container));
      var deniedIcon = new IconInfo (GetIconUrl ("PermissionDenied.gif", container));
      var undefinedIcon = new IconInfo (GetIconUrl ("PermissionUndefined.gif", container));

      foreach (var permission in _accessControlEntry.Permissions)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, "permissionCell");
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        if (permission.Allowed.HasValue)
        {
          if (permission.Allowed.Value)
            grantedIcon.Render (writer);
          else
            deniedIcon.Render (writer);
        }
        else
          undefinedIcon.Render (writer);
        writer.RenderEndTag();
      }
    }

    private void RenderPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.Parse (businessObject.BusinessObjectClass, propertyPathIdentifier);
      writer.Write (HtmlUtility.HtmlEncode (propertyPath.GetString (businessObject, null)));
    }

    private void RenderTenantHierarchyIcon (HtmlTextWriter writer, Control container)
    {
      string url;
      switch (_accessControlEntry.TenantHierarchyCondition)
      {
        case TenantHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.This:
          url = "HierarchyThis.gif";
          break;
        case TenantHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url, container));
      icon.Render (writer);
    }

    private void RenderGroupHierarchyIcon (HtmlTextWriter writer, Control container)
    {
      string url;
      switch (_accessControlEntry.GroupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.This:
          url = "HierarchyThis.gif";
          break;
        case GroupHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.Children:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          break;
        case GroupHierarchyCondition.ThisAndChildren:
          url = "HierarchyThisAndChildren.gif";
          break;
        case GroupHierarchyCondition.ThisAndParentAndChildren:
          url = "HierarchyThisAndParentAndChildren.gif";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url, container));
      icon.Render (writer);
    }

    private string GetIconUrl (string url, Control container)
    {
      return ResourceUrlResolver.GetResourceUrl (container, typeof (CollapsedAccessControlEntryRenderer), ResourceType.Image, url);
    }
  }
}