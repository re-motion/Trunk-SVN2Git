// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  public interface IClientTransactionWithID
  {
    Guid ID { get; }
    void Commit ();
    void Rollback ();
    ClientTransaction AsClientTransaction { get; }
  }

  public class ClientTransactionWithIDMixin : Mixin<ClientTransaction>, IClientTransactionWithID
  {
    private readonly Guid _id;

    public ClientTransactionWithIDMixin ()
    {
      _id = Guid.NewGuid ();
    }

    public Guid ID
    {
      get { return _id; }
    }

    public void Commit ()
    {
      This.Commit ();
    }

    public void Rollback ()
    {
      This.Rollback ();
    }

    public ClientTransaction AsClientTransaction
    {
      get { return This; }
    }

    [OverrideTarget]
    public new string ToString ()
    {
      return ID.ToString ();
    }
  }
}
