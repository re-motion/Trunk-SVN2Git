// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
      // BindableObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
      //    typeof (ISearchAvailableObjectsService), new SearchAllObjectsService());
    }

    protected void Application_End (object sender, EventArgs e)
    {

    }
  }
}
