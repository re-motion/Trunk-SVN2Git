// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Web.UI;
using Remotion.Globalization;
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
          writer.Write (HtmlUtility.HtmlEncode (AccessControlResources.TenantCondition_None));
          break;
        case TenantCondition.OwningTenant:
          RenderTenantHierarchyIcon (writer, container);
          RenderPropertyPathString (writer, "TenantCondition");
          break;
        case TenantCondition.SpecificTenant:
          RenderTenantHierarchyIcon (writer, container);
          RenderLabelAfterPropertyPathString (writer, "SpecificTenant.DisplayName");
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
          writer.Write (HtmlUtility.HtmlEncode (AccessControlResources.GroupCondition_None));
          break;
        case GroupCondition.OwningGroup:
          RenderGroupHierarchyIcon (writer, container);
          RenderPropertyPathString (writer, "GroupCondition");
          break;
        case GroupCondition.SpecificGroup:
          RenderGroupHierarchyIcon (writer, container);
          RenderLabelAfterPropertyPathString (writer, "SpecificGroup.ShortName");
          break;
        case GroupCondition.BranchOfOwningGroup:
          RenderLabelBeforePropertyPathString (writer, AccessControlResources.BranchOfOwningGroupLabel, "SpecificGroupType.DisplayName");
          break;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          RenderLabelAfterPropertyPathString (writer, "SpecificGroupType.DisplayName");
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
          writer.Write (HtmlUtility.HtmlEncode (AccessControlResources.UserCondition_None));
          break;
        case UserCondition.Owner:
          RenderPropertyPathString (writer, "UserCondition");
          break;
        case UserCondition.SpecificUser:
          RenderLabelAfterPropertyPathString (writer, "SpecificUser.DisplayName");
          break;
        case UserCondition.SpecificPosition:
          RenderLabelAfterPropertyPathString (writer, "SpecificPosition.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderAbstractRole (HtmlTextWriter writer, Control container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      RenderLabelBeforePropertyPathString (writer, string.Empty, "SpecificAbstractRole.DisplayName");
    }

    private void RenderLabelAfterPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Em);
      RenderPropertyPathString (writer, propertyPathIdentifier);
      writer.RenderEndTag();
      writer.Write (" (");
      string label = GetPropertyDisplayName (propertyPathIdentifier);
      writer.Write (HtmlUtility.HtmlEncode (label));
      writer.Write (")");
    }

    private void RenderLabelBeforePropertyPathString (HtmlTextWriter writer, string label, string propertyPathIdentifier)
    {
      writer.Write (HtmlUtility.HtmlEncode (label));
      writer.Write (" ");
      writer.RenderBeginTag (HtmlTextWriterTag.Em);
      RenderPropertyPathString (writer, propertyPathIdentifier);
      writer.RenderEndTag();
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
      return propertyPath.Properties[propertyPath.Properties.Length - 2].DisplayName;
    }

    private void RenderTenantHierarchyIcon (HtmlTextWriter writer, Control container)
    {
      string url;
      string text;
      switch (_accessControlEntry.TenantHierarchyCondition)
      {
        case TenantHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.This:
          url = "HierarchyThis.gif";
          text = AccessControlResources.TenantHierarchyCondition_This;
          break;
        case TenantHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          text = AccessControlResources.TenantHierarchyCondition_ThisAndParent;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url, container)) { AlternateText = text };
      icon.Render (writer);
    }

    private void RenderGroupHierarchyIcon (HtmlTextWriter writer, Control container)
    {
      string url;
      string text;
      switch (_accessControlEntry.GroupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.This:
          url = "HierarchyThis.gif";
          text = AccessControlResources.GroupHierarchyCondition_This;
          break;
        case GroupHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.Children:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          text = AccessControlResources.GroupHierarchyCondition_ThisAndParent;
          break;
        case GroupHierarchyCondition.ThisAndChildren:
          url = "HierarchyThisAndChildren.gif";
          text = AccessControlResources.GroupHierarchyCondition_ThisAndChildren;
          break;
        case GroupHierarchyCondition.ThisAndParentAndChildren:
          url = "HierarchyThisAndParentAndChildren.gif";
          text = AccessControlResources.GroupHierarchyCondition_ThisAndParentAndChildren;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url, container)) { AlternateText = text };
      icon.Render (writer);
    }

    private string GetIconUrl (string url, Control container)
    {
      return ResourceUrlResolver.GetResourceUrl (container, typeof (CollapsedAccessControlConditionsRenderer), ResourceType.Image, url);
    }
  }
}
