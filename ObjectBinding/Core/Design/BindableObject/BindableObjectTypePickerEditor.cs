using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public class BindableObjectTypePickerEditor : DropDownEditorBase
  {
    public BindableObjectTypePickerEditor ()
    {
    }

    protected override EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("editorService", editorService);


      return new BindableObjectTypePickerControl (provider, editorService, new BindableObjectTypeFinder (provider));
    }
  }
}