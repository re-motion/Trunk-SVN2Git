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
