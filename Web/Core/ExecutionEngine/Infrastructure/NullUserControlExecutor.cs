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
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class NullUserControlExecutor : IUserControlExecutor, IObjectReference
  {
    public static readonly IUserControlExecutor Null = new NullUserControlExecutor();

    private NullUserControlExecutor ()
    {
    }

    public bool IsNull
    {
      get { return true; }
    }

    public void Execute (WxeContext context)
    {
      //NOP
    }

    public WxeFunction Function
    {
      get { return null; }
    }

    public string BackedUpUserControlState
    {
      get { return null; }
    }

    public string BackedUpUserControl
    {
      get { return null; }
    }

    public string UserControlID
    {
      get { return null; }
    }

    public bool IsReturningPostBack
    {
      get { return false; }
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return Null;
    }

    public WxeStep ExecutingStep
    {
      get { return null; }
    }
  }
}
