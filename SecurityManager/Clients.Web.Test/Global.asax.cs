// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Autofac;
using AutofacContrib.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UberProfIntegration;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.Security.UI;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web.Test
{
  public class Global : SecurityManagerHttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter());
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), new WebSecurityAdapter());
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), new WxeSecurityAdapter());

      var builder = new ContainerBuilder();

      builder.RegisterAssemblyTypes (typeof (RendererBase<>).Assembly, typeof (BocRendererBase<>).Assembly)
          .Where (t => t.Namespace.EndsWith (".Factories")).AsImplementedInterfaces().SingleInstance();

      builder.RegisterInstance (ResourceTheme.NovaBlue).SingleInstance();

      builder.Register (c => new ScriptUtility()).As<IScriptUtility>().InstancePerDependency();

      //builder.Register (c => new LinqToSqlListenerFactory ())
      //    .As<IClientTransactionListenerFactory, IPersistenceListenerFactory> ()
      //    .InstancePerDependency ();

      var autofacServiceLocator = new AutofacServiceLocator (builder.Build());
      ServiceLocator.SetLocatorProvider (() => autofacServiceLocator);
    }

    protected void Application_End (object sender, EventArgs e)
    {
    }
  }
}