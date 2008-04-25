using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design
{
  /// <summary>
  ///   Editor applied to an <see cref="IBusinessObjectBoundControl.PropertyIdentifier">IBusinessObjectBoundControl.PropertyIdentifier</see>.
  /// </summary>
  public class PropertyPickerEditor : DropDownEditorBase
  {
    protected override EditorControlBase CreateEditorControl (ITypeDescriptorContext context, IServiceProvider provider, IWindowsFormsEditorService editorService)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("editorService", editorService);

      IBusinessObjectBoundControl control = context.Instance as IBusinessObjectBoundControl;
      if (control == null)
        throw new InvalidOperationException ("Cannot use PropertyPickerEditor for objects other than IBusinessObjectBoundControl.");

      return new PropertyPickerControl (control, provider, editorService);
    }
  }
}