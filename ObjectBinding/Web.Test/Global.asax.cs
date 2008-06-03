/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Web;
using log4net;
using log4net.Config;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web;
using Remotion.Web.Configuration;

namespace OBWTest
{
  public class Global : HttpApplication // , IResourceUrlResolver
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    private WaiConformanceLevel _waiConformanceLevelBackup;

    public Global ()
    {
      //  Initialize Logger
      LogManager.GetLogger (typeof (Global));
      InitializeComponent();
    }

    public XmlReflectionBusinessObjectStorageProvider XmlReflectionBusinessObjectStorageProvider
    {
      get { return XmlReflectionBusinessObjectStorageProvider.Current; }
    }

    //  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    //  {
    //    return this.Request.ApplicationPath + "/" + resourceType.Name + "/" + relativeUrl;
    //  }

    protected void Application_Start (Object sender, EventArgs e)
    {
      XmlConfigurator.Configure();

      string objectPath = Server.MapPath ("~/objects");
      if (!Directory.Exists (objectPath))
        Directory.CreateDirectory (objectPath);

      XmlReflectionBusinessObjectStorageProvider provider = new XmlReflectionBusinessObjectStorageProvider (objectPath);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent (provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IGetObjectService), provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (ISearchAvailableObjectsService), new BindableXmlObjectSearchService ());
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService ());
    }

    protected void Session_Start (Object sender, EventArgs e)
    {
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_AuthenticateRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_PreRequestHandlerExecute (Object sender, EventArgs e)
    {
      _waiConformanceLevelBackup = WebConfiguration.Current.Wcag.ConformanceLevel;
    }

    protected void Application_PostRequestHandlerExecute (Object sender, EventArgs e)
    {
      WebConfiguration.Current.Wcag.ConformanceLevel = _waiConformanceLevelBackup;
    }

    protected void Application_EndRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_Error (Object sender, EventArgs e)
    {
    }

    protected void Session_End (Object sender, EventArgs e)
    {
    }

    protected void Application_End (Object sender, EventArgs e)
    {
    }

    #region Web Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.components = new System.ComponentModel.Container();
    }

    #endregion
  }
}
