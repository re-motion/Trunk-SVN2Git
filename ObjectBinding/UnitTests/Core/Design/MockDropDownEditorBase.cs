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