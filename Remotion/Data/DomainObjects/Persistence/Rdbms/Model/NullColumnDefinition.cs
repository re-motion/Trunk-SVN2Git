// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Represents a non-existing column.
  /// </summary>
  public class NullColumnDefinition : IColumnDefinition
  {
    string IStoragePropertyDefinition.Name
    {
      get { throw new NotImplementedException(); }
    }

    public void Accept (IColumnDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitNullColumnDefinition (this);
    }

    public bool IsNull
    {
      get { return true; }
    }

    public bool Equals (IColumnDefinition other)
    {
      return other != null && other.GetType () == GetType ();
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as IColumnDefinition);
    }

    public override int GetHashCode ()
    {
      return 0;
    }
    
  }
}