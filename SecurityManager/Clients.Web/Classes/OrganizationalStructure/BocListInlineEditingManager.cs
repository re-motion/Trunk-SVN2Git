// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Globalization.UI;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  public class BocListInlineEditingManager<TBusinessObject>
      where TBusinessObject: BaseSecurityManagerObject
  {
    private readonly BocList _bocList;
    private readonly Func<TBusinessObject> _newObjectFactory;
    private readonly IResourceUrlFactory _resourceUrlFactory;
    private bool _isNewObject;

    public BocListInlineEditingManager (BocList bocList, Func<TBusinessObject> newObjectFactory, IResourceUrlFactory resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("bocList", bocList);
      ArgumentUtility.CheckNotNull ("newObjectFactory", newObjectFactory);
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);

      _bocList = bocList;
      _newObjectFactory = newObjectFactory;
      _resourceUrlFactory = resourceUrlFactory;

      if (!ControlHelper.IsDesignMode (_bocList))
      {
        _bocList.EditModeControlFactory = new EditableRowAutoCompleteControlFactory();

        _bocList.FixedColumns.Insert (
            0,
            new BocRowEditModeColumnDefinition
            {
                Width = Unit.Pixel (40),
                EditIcon = GetIcon ("EditItem.gif", GlobalResources.Edit),
                SaveIcon = GetIcon ("ApplyButton.gif", GlobalResources.Apply),
                CancelIcon = GetIcon ("CancelButton.gif", GlobalResources.Cancel)
            });

        _bocList.MenuItemClick += HandleMenuItemClick;
        _bocList.EditableRowChangesCanceled += HandleEditableRowChangesCanceled;
        _bocList.EditableRowChangesSaved += HandleEditableRowChangesSaved;

        _bocList.ListMenuItems.Add (
            new BocMenuItem
            {
                ItemID = "NewItem",
                Text = "$res:New",
                Command = new BocMenuItemCommand { Show = CommandShow.EditMode }
            });
        _bocList.ListMenuItems.Add (
            new BocMenuItem
            {
                ItemID = "DeleteItem",
                Text = "$res:Delete",
                RequiredSelection = RequiredSelection.OneOrMore,
                Command = new BocMenuItemCommand { Show = CommandShow.EditMode }
            });
      }
    }

    public void LoadControlState (object state)
    {
      _isNewObject = (bool) state;
    }

    public object SaveControlState ()
    {
      return _isNewObject;
    }

    private void HandleMenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        _bocList.AddAndEditRow (_newObjectFactory());
        _isNewObject = true;
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (TBusinessObject businessObject in _bocList.GetSelectedBusinessObjects())
        {
          _bocList.RemoveRow (businessObject);
          businessObject.Delete();
        }
      }

      _bocList.ClearSelectedRows();
    }

    private void HandleEditableRowChangesCanceled (object sender, BocListItemEventArgs e)
    {
      var substitution = (TBusinessObject) e.BusinessObject;
      if (_isNewObject)
        substitution.Delete();
      _isNewObject = false;
    }

    private void HandleEditableRowChangesSaved (object sender, BocListItemEventArgs e)
    {
      _isNewObject = false;
    }

    private IconInfo GetIcon (string resourceUrl, string alternateText)
    {
      var url = _resourceUrlFactory.CreateThemedResourceUrl (typeof (BocListInlineEditingManager<>), ResourceType.Image, resourceUrl).GetUrl();
      return new IconInfo (url) { AlternateText = alternateText };
    }
  }
}