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