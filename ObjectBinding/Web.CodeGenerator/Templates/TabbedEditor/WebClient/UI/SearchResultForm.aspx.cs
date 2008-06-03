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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using $PROJECT_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;
using $DOMAIN_ROOTNAMESPACE$;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  // <WxePageFunction>
  //   <Parameter name="query" type="IQuery" />
  //   <Variable name="searchResult" type="DomainObjectCollection" />
  // </WxePageFunction>
  public partial class SearchResult$DOMAIN_CLASSNAME$Form : BasePage
  {
    private void Page_Load(object sender, System.EventArgs e)
    {
      if (!IsPostBack)
      {
        if (query == null)
          query = new Query ("All$DOMAIN_CLASSNAME$s");
        searchResult = ClientTransaction.Current.QueryManager.GetCollection (query);
      }
      $DOMAIN_CLASSNAME$List.LoadUnboundValue (searchResult, IsPostBack);
    }

    protected void $DOMAIN_CLASSNAME$List_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (e.Column.ItemID == "Edit")
      {
        try
        {
          Edit$DOMAIN_CLASSNAME$Form.Call (this, ($DOMAIN_CLASSNAME$)e.BusinessObject);
          ClientTransaction.Current.Commit ();
        }
        catch (WxeUserCancelException)
        {
        }
      }
    }
  }
}
