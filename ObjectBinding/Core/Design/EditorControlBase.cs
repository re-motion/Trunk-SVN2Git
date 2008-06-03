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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design
{
  [TypeDescriptionProvider (typeof (EditorControlBaseClassProvider))]
  public abstract class EditorControlBase : UserControl
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IWindowsFormsEditorService _editorService;

    protected EditorControlBase (IServiceProvider provider, IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("editorService", editorService);
      
      _serviceProvider = provider;
      _editorService = editorService;
    }

    protected EditorControlBase ()
    {
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public abstract object Value { get; set; }

    public IServiceProvider ServiceProvider
    {
      get { return _serviceProvider; }
    }

    public IWindowsFormsEditorService EditorService
    {
      get { return _editorService; }
    }
  }
}
