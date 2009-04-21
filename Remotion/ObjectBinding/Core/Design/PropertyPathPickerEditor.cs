// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel;
using System.Windows.Forms.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design
{
  /// <summary>
  ///   Editor applied to the string property used to set the 
  ///   <see cref="BusinessObjectPropertyPath.Identifier">BusinessObjectPropertyPath.Identifier</see>.
  /// </summary>
  public class PropertyPathPickerEditor : DropDownEditorBase
  {
    protected override EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("editorService", editorService);

      IBusinessObjectClassSource propertySource = context.Instance as IBusinessObjectClassSource;
      if (propertySource == null)
        throw new InvalidOperationException ("Cannot use PropertyPathPickerEditor for objects other than IBusinessObjectClassSource.");

      return new PropertyPathPickerControl (propertySource, provider, editorService);
    }
  }
}
