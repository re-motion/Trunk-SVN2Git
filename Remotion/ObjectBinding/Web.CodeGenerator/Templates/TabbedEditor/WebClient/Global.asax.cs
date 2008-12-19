using System;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;

using $DOMAIN_ROOTNAMESPACE$;

namespace $PROJECT_ROOTNAMESPACE$
{
  public class Global : System.Web.HttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {
      BindableObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (BindableDomainObjectGetObjectService), new BindableDomainObjectGetObjectService ());

      // This statement registers the code with the BindableObjectProvider. ISearchAvailableObjectService
      // is implemented in SearchAllObjectsService.cs, which must be part of your
      // domain project. [reinhard.gantar@rubicon.eu]
      BindableObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (ISearchAvailableObjectsService), new SearchAllObjectsService());
    }

    protected void Application_End (object sender, EventArgs e)
    {

    }
  }
}