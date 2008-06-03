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

namespace Remotion.Security.UnitTests.TestDomain
{
  [PermanentGuid ("00000000-0000-0000-0001-000000000000")]
  public class File : ISecurableObject
  {
    private Confidentiality _confidentiality;
    private SomeEnum _someEnum;
	
    private string _id;

    public File ()
    {
    }

    [PermanentGuid ("00000000-0000-0000-0001-000000000001")]
    public Confidentiality Confidentiality
    {
      get { return _confidentiality; }
      set { _confidentiality = value; }
    }

    public SomeEnum SimpleEnum
    {
      get { return _someEnum; }
      set { _someEnum = value; }
    }

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    [DemandMethodPermission (DomainAccessTypes.Journalize)]
    public void Journalize ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Type GetSecurableType ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
