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
using Microsoft.Practices.ServiceLocation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UberProfIntegration;
using Remotion.Implementation;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.Security.UI;
using Remotion.Web.UI;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web.Test
{
  public class Global : SecurityManagerHttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {

      var defaultServiceLocator = new DefaultServiceLocator();

      //defaultServiceLocator.Register (typeof (IClientTransactionListenerFactory), typeof (LinqToSqlListenerFactory), LifetimeKind.Singleton);
      //defaultServiceLocator.Register (typeof (IPersistenceListenerFactory), typeof (LinqToSqlListenerFactory), LifetimeKind.Singleton);

      ServiceLocator.SetLocatorProvider (() => defaultServiceLocator);

      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter ());
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), new WebSecurityAdapter ());
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), new WxeSecurityAdapter ());
    }

    protected void Application_End (object sender, EventArgs e)
    {
    }
  }
}