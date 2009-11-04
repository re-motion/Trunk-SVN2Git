// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Web.UI;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode
{
  /// <summary>
  /// Abstract base class for column renderers that can handle derived classes of <see cref="BocCommandEnabledColumnDefinition"/>.
  /// Defines common utility methods.
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocCommandEnabledColumnRendererBase<TBocColumnDefinition> : BocColumnRendererBase<TBocColumnDefinition>
      where TBocColumnDefinition: BocCommandEnabledColumnDefinition
  {
    protected BocCommandEnabledColumnRendererBase (
        IHttpContext context, HtmlTextWriter writer, IBocList list, TBocColumnDefinition columnDefintion, CssClassContainer cssClasses)
        : base (context, writer, list, columnDefintion, cssClasses)
    {
    }

    protected void RenderCellIcon (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      IconInfo icon = BusinessObjectBoundWebControl.GetIcon (businessObject, businessObject.BusinessObjectClass.BusinessObjectProvider);

      if (icon != null)
      {
        RenderIcon (icon, null);
        Writer.Write (c_whiteSpace);
      }
    }

    protected void RenderValueColumnCellText (string contents)
    {
      Writer.AddAttribute ("class", CssClasses.CommandText);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      
      contents = HtmlUtility.HtmlEncode (contents);
      if (StringUtility.IsNullOrEmpty (contents))
        contents = c_whiteSpace;
      Writer.Write (contents);

      Writer.RenderEndTag();
    }

    protected bool RenderBeginTagDataCellCommand (
        IBusinessObject businessObject,
        int originalRowIndex)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      BocListItemCommand command = Column.Command;
      if (command == null)
        return false;

      bool isReadOnly = List.IsReadOnly;
      bool isActive = command.Show == CommandShow.Always
                      || isReadOnly && command.Show == CommandShow.ReadOnly
                      || !isReadOnly && command.Show == CommandShow.EditMode;

      bool isCommandAllowed = (command.Type != CommandType.None) && !List.EditModeController.IsRowEditModeActive;
      bool isCommandEnabled = (command.CommandState == null) || command.CommandState.IsEnabled ( List, businessObject, Column);
      bool isCommandWaiCompliant = (!WcagHelper.Instance.IsWaiConformanceLevelARequired() || command.Type == CommandType.Href);
      if (isActive && isCommandAllowed && isCommandEnabled && isCommandWaiCompliant)
      {
        string objectID = null;
        IBusinessObjectWithIdentity businessObjectWithIdentity = businessObject as IBusinessObjectWithIdentity;
        if (businessObjectWithIdentity != null)
          objectID = businessObjectWithIdentity.UniqueIdentifier;

        string argument = List.GetListItemCommandArgument (ColumnIndex, originalRowIndex);
        string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument) + ";";
        string onClick = List.HasClientScript ? c_onCommandClickScript : string.Empty;
        if (command.Type == CommandType.None)
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDisabled);
        command.RenderBegin (Writer, postBackEvent, onClick, originalRowIndex, objectID, businessObject as ISecurableObject);

        return true;
        
      }
      return false;
    }

    protected void RenderEndTagDataCellCommand ()
    {
      Column.Command.RenderEnd (Writer);
    }
  }
}
