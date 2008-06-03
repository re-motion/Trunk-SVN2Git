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
using Remotion.ObjectBinding.Design;

namespace Remotion.ObjectBinding.UnitTests.Core.Design
{
  public abstract class MockDropDownEditorBase : DropDownEditorBase
  {
    public abstract EditorControlBase NewCreateEditorControl (ITypeDescriptorContext context, IWindowsFormsEditorService editorService);

    protected override EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService editorService)
    {
      return NewCreateEditorControl (context, editorService);
    }
  }
}
