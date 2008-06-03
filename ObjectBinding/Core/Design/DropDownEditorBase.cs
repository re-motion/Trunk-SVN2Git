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
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Remotion.ObjectBinding.Design
{
  public abstract class DropDownEditorBase : UITypeEditor
  {
    protected DropDownEditorBase ()
    {
    }

    protected abstract EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService editorService);

    public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      if (context != null && context.Instance != null && provider != null)
      {
        IWindowsFormsEditorService editorService = (IWindowsFormsEditorService) provider.GetService (typeof (IWindowsFormsEditorService));

        if (editorService != null)
        {
          EditorControlBase control = CreateEditorControl (context, provider, editorService);
          control.Value = value;
          editorService.DropDownControl (control);
          value = control.Value;
        }
      }
      return value;
    }

    public override UITypeEditorEditStyle GetEditStyle (ITypeDescriptorContext context)
    {
      if (context != null && context.Instance != null)
        return UITypeEditorEditStyle.DropDown;
      return base.GetEditStyle (context);
    }
  }
}
