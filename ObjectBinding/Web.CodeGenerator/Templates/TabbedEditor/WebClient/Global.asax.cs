using System;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Data.DomainObjects.ObjectBinding;

namespace $PROJECT_ROOTNAMESPACE$
{
  public class Global : System.Web.HttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {
      BindableObjectProvider.Current.AddService (
          typeof (BindableDomainObjectSearchService), new BindableDomainObjectSearchService ());
      BindableObjectProvider.Current.AddService (
          typeof (BindableDomainObjectGetObjectService), new BindableDomainObjectGetObjectService ());

    }

    protected void Application_End (object sender, EventArgs e)
    {

    }
  }
}