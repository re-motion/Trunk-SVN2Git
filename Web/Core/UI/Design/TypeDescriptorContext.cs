using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Remotion.Web.UI.Design
{

public class TypeDescriptorContext : ITypeDescriptorContext
{
  private IDesigner _designer;
  private IServiceProvider _provider;
  private PropertyDescriptor _propertyDescriptor;

  public TypeDescriptorContext (
      IDesigner designer,
      IServiceProvider provider,
      PropertyDescriptor propertyDescriptor)
  {
    _designer = designer;
    _provider = provider;
    _propertyDescriptor = propertyDescriptor;
  }

  private IComponentChangeService ComponentChangeService
  {
    get { return (IComponentChangeService) this.GetService (typeof (IComponentChangeService)); }
  }

  public object GetService (Type serviceType)
  {
    return _provider.GetService(serviceType);
  }

  public IContainer Container
  {
    get { return _designer.Component.Site.Container; }
  }

  public object Instance
  {
    get { return _designer.Component; }
  }

  public PropertyDescriptor PropertyDescriptor
  {
    get { return _propertyDescriptor; }
  }

  public void OnComponentChanged()
  {
    if (ComponentChangeService != null)
      ComponentChangeService.OnComponentChanged (Instance, PropertyDescriptor, null, null);
  }

  public bool OnComponentChanging()
  {
    if (ComponentChangeService != null)
    {
      try
      {
        ComponentChangeService.OnComponentChanging (Instance, PropertyDescriptor);
      }
      catch (CheckoutException e)
      {
        if (e == CheckoutException.Canceled)
          return false;
        throw e;
      }
    }
    return true;
  }
}

}
