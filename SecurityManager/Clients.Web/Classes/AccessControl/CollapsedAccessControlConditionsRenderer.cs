// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Web;
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes.AccessControl
{
  /// <summary>
  /// The <see cref="CollapsedAccessControlConditionsRenderer"/> type is responsible for generating the collapsed view on an <see cref="AccessControlEntry"/>
  /// </summary>
  public class CollapsedAccessControlConditionsRenderer
  {
    private readonly AccessControlEntry _accessControlEntry;
    private readonly IResourceUrlFactory _resourceUrlFactory;

    public CollapsedAccessControlConditionsRenderer (AccessControlEntry accessControlEntry, IResourceUrlFactory resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("currentAccessControlEntry", accessControlEntry);
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);

      _accessControlEntry = accessControlEntry;
      _resourceUrlFactory = resourceUrlFactory;
    }

    public AccessControlEntry AccessControlEntry
    {
      get { return _accessControlEntry; }
    }

    public void RenderTenant (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.TenantCondition)
      {
        case TenantCondition.None:
          writer.WriteEncodedText (AccessControlResources.TenantCondition_None);
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

    public void RenderGroup (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.GroupCondition)
      {
        case GroupCondition.None:
          writer.WriteEncodedText (AccessControlResources.GroupCondition_None);
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

    public void RenderUser (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.UserCondition)
      {
        case UserCondition.None:
          writer.WriteEncodedText (AccessControlResources.UserCondition_None);
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

    public void RenderAbstractRole (HtmlTextWriter writer, IControl container)
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
      writer.WriteEncodedText (label);
      writer.Write (")");
    }

    private void RenderLabelBeforePropertyPathString (HtmlTextWriter writer, string label, string propertyPathIdentifier)
    {
      writer.WriteEncodedText (label);
      writer.Write (" ");
      writer.RenderBeginTag (HtmlTextWriterTag.Em);
      RenderPropertyPathString (writer, propertyPathIdentifier);
      writer.RenderEndTag();
    }

    private void RenderPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.CreateStatic (businessObject.BusinessObjectClass, propertyPathIdentifier);
      var result = propertyPath.GetResult (
          businessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);
      writer.WriteEncodedText (result.GetString (null));
    }

    private string GetPropertyDisplayName (string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.CreateStatic (businessObject.BusinessObjectClass, propertyPathIdentifier);
      Assertion.IsTrue (propertyPath.Properties.Count >= 2);
      return propertyPath.Properties[propertyPath.Properties.Count - 2].DisplayName;
    }

    private void RenderTenantHierarchyIcon (HtmlTextWriter writer, IControl container)
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

      var icon = new IconInfo (GetIconUrl (url).GetUrl()) { AlternateText = text };
      icon.Render (writer, container);
    }

    private void RenderGroupHierarchyIcon (HtmlTextWriter writer, IControl container)
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

      var icon = new IconInfo (GetIconUrl (url).GetUrl()) { AlternateText = text };
      icon.Render (writer, container);
    }

    private IResourceUrl GetIconUrl (string url)
    {
      return _resourceUrlFactory.CreateThemedResourceUrl (typeof (CollapsedAccessControlConditionsRenderer), ResourceType.Image, url);
    }
  }
}