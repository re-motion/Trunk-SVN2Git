// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Specialized;
using System.Web.UI;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Development.Web.UnitTesting.AspNetFramework
{
  public class PageMock : Page
  {
    // types

    // static members and constants

    // member fields

    private PageStatePersister _pageStatePersister;

    // construction and disposing

    public PageMock ()
    {
    }

    // methods and properties

    public NameValueCollection RequestValueCollection
    {
      get { return (NameValueCollection) PrivateInvoke.GetNonPublicField (this,"_requestValueCollection"); }
    }

    public void SetRequestValueCollection (NameValueCollection requestValueCollection)
    {
      ArgumentUtility.CheckNotNull ("requestValueCollection", requestValueCollection);

      PrivateInvoke.SetNonPublicField (this, "_requestValueCollection", requestValueCollection);
    }

    protected override PageStatePersister PageStatePersister
    {
      get { return GetPageStatePersister (); }
    }

    public PageStatePersister GetPageStatePersister ()
    {
      EnsurePageStatePersister ();
      return _pageStatePersister;
    }

    public void SetPageStatePersister (PageStatePersister pageStatePersister)
    {
      ArgumentUtility.CheckNotNull ("pageStatePersister", pageStatePersister);

      _pageStatePersister = pageStatePersister;
    }

    private void EnsurePageStatePersister ()
    {
      if (_pageStatePersister == null)
        _pageStatePersister = new HiddenFieldPageStatePersister (this);
    }

    public void LoadAllState ()
    {
      PrivateInvoke.InvokeNonPublicMethod (this, typeof (Page), "LoadAllState", new object[0]);
    }

    public void SaveAllState ()
    {
      PrivateInvoke.InvokeNonPublicMethod (this, typeof (Page), "SaveAllState", new object[0]);
    }
  }
}
