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
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Persistence.Rdbms
{
  public class SqlProviderBaseTest : ClientTransactionBaseTest
  {
    private RdbmsProviderDefinition _providerDefinition;
    private SqlProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _providerDefinition = new RdbmsProviderDefinition (c_testDomainProviderID, typeof (SqlProvider), TestDomainConnectionString);

      _provider = new SqlProvider (_providerDefinition);
    }

    public override void TearDown ()
    {
      _provider.Dispose ();
      base.TearDown ();
    }

    protected RdbmsProviderDefinition ProviderDefinition
    {
      get { return _providerDefinition; }
    }

    protected SqlProvider Provider
    {
      get { return _provider; }
    }
  }
}
