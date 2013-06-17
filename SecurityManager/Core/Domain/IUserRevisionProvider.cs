// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Context;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  [ConcreteImplementation (typeof (UserRevisionProvider), Lifetime = LifetimeKind.Singleton)]
  public interface IRevisionProvider<TRevisionValue, TRevisionKey>
      where TRevisionValue : IRevisionValue
      where TRevisionKey : IRevisionKey
  {
    TRevisionValue GetRevision (TRevisionKey key);
    void InvalidateRevsion (TRevisionKey key);
  }

  public abstract class RevisionProviderBase<TRevisionValue, TRevisionKey> : IRevisionProvider<TRevisionValue, TRevisionKey>
      where TRevisionValue : IRevisionValue
      where TRevisionKey : IRevisionKey
  {
    private readonly string _revisionProviderKey;

    protected RevisionProviderBase ()
    {
      _revisionProviderKey = SafeContextKeys.SecurityManagerRevision + "_" + Guid.NewGuid().ToString();
    }
    
    protected abstract Int32RevisionValue CreateRevisionValue (object rawValue);

    public TRevisionValue GetRevision (TRevisionKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      var revisions = GetCachedRevisions();
      return revisions.GetOrCreateValue (key, GetRevisionFromDatabase);
    }

    public void InvalidateRevsion (TRevisionKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      var revisions = GetCachedRevisions();
      revisions.Remove (key);
    }

    private SimpleDataStore<TRevisionKey, TRevisionValue> GetCachedRevisions ()
    {
      var revisions = (SimpleDataStore<TRevisionKey, TRevisionValue>) SafeContext.Instance.GetData (_revisionProviderKey);
      if (revisions == null)
      {
        revisions = new SimpleDataStore<TRevisionKey, TRevisionValue>();
        SafeContext.Instance.SetData (_revisionProviderKey, revisions);
      }
      return revisions;
    }

    private TRevisionValue GetRevisionFromDatabase (TRevisionKey key)
    {
      return default (TRevisionValue);
    }
  }

  //RM-5640: Rewrite with tests
  public class UserRevisionProvider : RevisionProviderBase<Int32RevisionValue, UserRevisionKey>
  {
    protected override Int32RevisionValue CreateRevisionValue (object rawValue)
    {
      if (rawValue == null)
        return new Int32RevisionValue (0);

      return new Int32RevisionValue ((int) rawValue);
    }
  }


  public sealed class Int32RevisionValue : IRevisionValue
  {
    private readonly DateTime _timestamp;
    private readonly int _revision;

    public Int32RevisionValue (int revision)
    {
      _timestamp = DateTime.UtcNow;
      _revision = revision;
    }

    public bool IsCurrent (IRevisionValue reference)
    {
      var referenceRevision = reference as Int32RevisionValue;
      if (referenceRevision == null)
        return false;

      if (_revision == referenceRevision._revision)
        return true;
      if (_timestamp > referenceRevision._timestamp)
        return true;
      return false;
    }
  }

  public interface IRevisionValue
  {
    bool IsCurrent (IRevisionValue reference);
  }

  public interface IRevisionKey
  {
    Guid GlobalKey { get; }
    string LocalKey { get; }
  }


  [PermanentGuid (c_permanentGuid)]
  public sealed class RevisionKey : IRevisionKey
  {
    private const string c_permanentGuid = "{446DF534-DBEA-420E-9AC1-0B19D51B0ED3}";
    private static readonly Guid s_globalKey = new Guid (c_permanentGuid);

    public RevisionKey ()
    {
    }

    public Guid GlobalKey
    {
      get { return s_globalKey; }
    }

    public string LocalKey
    {
      get { return null; }
    }
  }

  [PermanentGuid (c_permanentGuid)]
  public sealed class UserRevisionKey : IRevisionKey
  {
    private const string c_permanentGuid = "{7ABCDBE8-B3F8-41FB-826B-990DC3D4CB51}";
    private static readonly Guid s_globalKey = new Guid (c_permanentGuid);
    private readonly string _localKey;

    public UserRevisionKey (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      _localKey = userName;
    }

    public Guid GlobalKey
    {
      get { return s_globalKey; }
    }

    public string LocalKey
    {
      get { return _localKey; }
    }

    public override bool Equals (object obj)
    {
      var otherRevisionKey = obj as UserRevisionKey;
      if (otherRevisionKey == null)
        return false;
      return string.Equals (_localKey, otherRevisionKey._localKey);
    }

    public override int GetHashCode ()
    {
      return _localKey.GetHashCode();
    }
  }
}