using System;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;

// ATTENTION! This global.asax.cs file is an interim version
// and supposed to be
// used with Remotion 1.11.5.x. It contains a hack for making
// the supplementary code in SearchAllObjectsService.cs work.
// This code is part of the domain, so a "using <YOUR DOMAIN HERE>"
// must be provided: 
using $DOMAIN_ROOTNAMESPACE$;

namespace PhoneBook.Web
{
  public class Global : System.Web.HttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {
      BindableObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (BindableDomainObjectGetObjectService), new BindableDomainObjectGetObjectService ());

      // This statement registers the code with the BindableObjectProvider. ISearchAvailableObjectService
      // is implemented in the aforementioned SearchAllObjectsService.cs.
      BindableObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (ISearchAvailableObjectsService), new SearchAllObjectsService());
    }

    protected void Application_End (object sender, EventArgs e)
    {

    }
  }
}