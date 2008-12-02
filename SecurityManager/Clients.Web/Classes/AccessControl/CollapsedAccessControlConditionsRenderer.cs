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
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes.AccessControl
{
  /// <summary>
  /// The <see cref="CollapsedAccessControlConditionsRenderer"/> type is responsible for generating the collapsed view on an <see cref="AccessControlEntry"/>
  /// </summary>
  public class CollapsedAccessControlConditionsRenderer
  {
    private readonly AccessControlEntry _accessControlEntry;

    public CollapsedAccessControlConditionsRenderer (AccessControlEntry accessControlEntry)
    {
      ArgumentUtility.CheckNotNull ("currentAccessControlEntry", accessControlEntry);
      _accessControlEntry = accessControlEntry;
    }

    public AccessControlEntry AccessControlEntry
    {
      get { return _accessControlEntry; }
    }

    public void RenderTenant (HtmlTextWriter writer, Control container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

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
          RenderLabelAndPropertyPathString (writer, "SpecificTenant.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderGroup (HtmlTextWriter writer, Control container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.GroupCondition)
      {
        case GroupCondition.None:
          break;
        case GroupCondition.OwningGroup:
          RenderGroupHierarchyIcon (writer, container);
          RenderPropertyPathString (writer, "GroupCondition");
          break;
        case GroupCondition.SpecificGroup:
          RenderGroupHierarchyIcon (writer, container);
          RenderLabelAndPropertyPathString (writer, "SpecificGroup.ShortName");
          break;
        case GroupCondition.BranchOfOwningGroup:
          RenderLabelAndPropertyPathString (writer, AccessControlResources.BranchOfOwningGroupLabel, "SpecificGroupType.DisplayName");
          break;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          RenderLabelAndPropertyPathString (writer, "SpecificGroupType.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderUser (HtmlTextWriter writer, Control container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.UserCondition)
      {
        case UserCondition.None:
          break;
        case UserCondition.Owner:
          RenderPropertyPathString (writer, "UserCondition");
          break;
        case UserCondition.SpecificUser:
          RenderLabelAndPropertyPathString (writer, "SpecificUser.DisplayName");
          break;
        case UserCondition.SpecificPosition:
          RenderLabelAndPropertyPathString (writer, "SpecificPosition.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderAbstractRole (HtmlTextWriter writer, Control container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      RenderLabelAndPropertyPathString (writer, string.Empty, "SpecificAbstractRole.DisplayName");
    }

    private void RenderLabelAndPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      string label = GetPropertyDisplayName (propertyPathIdentifier);
      RenderLabelAndPropertyPathString (writer, label, propertyPathIdentifier);
    }

    private void RenderLabelAndPropertyPathString (HtmlTextWriter writer, string label, string propertyPathIdentifier)
    {
      writer.Write (HtmlUtility.HtmlEncode (label));
      writer.Write (" ");
      writer.RenderBeginTag (HtmlTextWriterTag.Em);
      RenderPropertyPathString (writer, propertyPathIdentifier);
      writer.RenderEndTag ();
    }

    private void RenderPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.Parse (businessObject.BusinessObjectClass, propertyPathIdentifier);
      writer.Write (HtmlUtility.HtmlEncode (propertyPath.GetString (businessObject, null)));
    }

    private string GetPropertyDisplayName (string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.Parse (businessObject.BusinessObjectClass, propertyPathIdentifier);
      Assertion.IsTrue (propertyPath.Properties.Length >= 2);
      return propertyPath.Properties[propertyPath.Properties.Length-2].DisplayName;
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
      return ResourceUrlResolver.GetResourceUrl (container, typeof (CollapsedAccessControlConditionsRenderer), ResourceType.Image, url);
    }
  }
}