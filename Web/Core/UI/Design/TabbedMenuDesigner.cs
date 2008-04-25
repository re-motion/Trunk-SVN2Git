using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{
public class TabbedMenuDesigner: WebControlDesigner, IServiceProvider
{
  private DesignerVerbCollection _verbs = null;

	public TabbedMenuDesigner()
	{
    _verbs = new DesignerVerbCollection();
    _verbs.Add (new DesignerVerb ("Edit Menu Tabs", new EventHandler(OnVerbEditFixedColumns)));
  }

  private void OnVerbEditFixedColumns (object sender, EventArgs e) 
  {
    TabbedMenu tabStripMenu = Component as TabbedMenu;
    if (tabStripMenu == null)
      throw new InvalidOperationException ("Cannot use TabbedMenuDesigner for objects other than TabbedMenu.");

    PropertyDescriptorCollection propertyDescriptors = TypeDescriptor.GetProperties (tabStripMenu);
    PropertyDescriptor propertyDescriptor = propertyDescriptors["Tabs"];

    TypeDescriptorContext context = new TypeDescriptorContext (this, this, propertyDescriptor);
    object value = propertyDescriptor.GetValue (Component);
    MainMenuTabCollectionEditor editor = null;
    editor = (MainMenuTabCollectionEditor) TypeDescriptor.GetEditor (value, typeof(UITypeEditor));
    editor.EditValue (context, this, value);
  }

  public override DesignerVerbCollection Verbs 
  {
    get { return _verbs; }
  }

  protected override object GetService (Type serviceType)
  {
    return base.GetService (serviceType);
  }

  object IServiceProvider.GetService (Type serviceType)
  {
    return GetService (serviceType);
  }
}

}
