// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  [TestFixture]
  public class MetadataCacheTest
  {
    // types

    // static members

    // member fields

    private MetadataCache _cache;

    // construction and disposing

    public MetadataCacheTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _cache = new MetadataCache ();
    }

    [Test]
    public void CacheSecurableClassInfos ()
    {
      Type fileType = typeof (File);
      Type paperFileType = typeof (PaperFile);

      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      Assert.IsFalse (_cache.ContainsSecurableClassInfo (fileType));
      Assert.IsNull (_cache.GetSecurableClassInfo (fileType));

      _cache.AddSecurableClassInfo (fileType, fileTypeInfo);
      Assert.AreSame (fileTypeInfo, _cache.GetSecurableClassInfo (fileType));
      Assert.IsFalse (_cache.ContainsSecurableClassInfo (paperFileType));
      Assert.IsNull (_cache.GetSecurableClassInfo (paperFileType));

      _cache.AddSecurableClassInfo (paperFileType, paperFileTypeInfo);
      Assert.AreSame (fileTypeInfo, _cache.GetSecurableClassInfo (fileType));
      Assert.AreSame (paperFileTypeInfo, _cache.GetSecurableClassInfo (paperFileType));
    }

    [Test]
    public void CacheStatePropertyInfos ()
    {
      PropertyInfo fileConfidentialityProperty = typeof (File).GetProperty ("Confidentiality");
      Assert.IsNotNull (fileConfidentialityProperty);

      PropertyInfo paperFileConfidentialityProperty = typeof (PaperFile).GetProperty ("Confidentiality");
      Assert.IsNotNull (paperFileConfidentialityProperty);

      PropertyInfo paperFileStateProperty = typeof (PaperFile).GetProperty ("State");
      Assert.IsNotNull (paperFileStateProperty);

      StatePropertyInfo confidentialityPropertyInfo = new StatePropertyInfo ();
      StatePropertyInfo statePropertyInfo = new StatePropertyInfo ();

      Assert.IsFalse (_cache.ContainsStatePropertyInfo (fileConfidentialityProperty));
      Assert.IsNull (_cache.GetStatePropertyInfo (fileConfidentialityProperty));

      _cache.AddStatePropertyInfo (fileConfidentialityProperty, confidentialityPropertyInfo);
      Assert.AreSame (confidentialityPropertyInfo, _cache.GetStatePropertyInfo (fileConfidentialityProperty));
      Assert.AreSame (_cache.GetStatePropertyInfo (fileConfidentialityProperty), _cache.GetStatePropertyInfo (paperFileConfidentialityProperty));
      Assert.IsFalse (_cache.ContainsStatePropertyInfo (paperFileStateProperty));
      Assert.IsNull (_cache.GetStatePropertyInfo (paperFileStateProperty));

      _cache.AddStatePropertyInfo (paperFileStateProperty, statePropertyInfo);
      Assert.AreSame (confidentialityPropertyInfo, _cache.GetStatePropertyInfo (fileConfidentialityProperty));
      Assert.AreSame (statePropertyInfo, _cache.GetStatePropertyInfo (paperFileStateProperty));
    }

    [Test]
    public void CacheEnumValueInfos ()
    {
      Assert.IsFalse (_cache.ContainsEnumValueInfo (FileState.New));
      Assert.IsNull (_cache.GetEnumValueInfo (FileState.New));

      _cache.AddEnumValueInfo (FileState.New, PropertyStates.FileStateNew);
      Assert.AreSame (PropertyStates.FileStateNew, _cache.GetEnumValueInfo (FileState.New));
      Assert.IsFalse (_cache.ContainsEnumValueInfo (FileState.Normal));
      Assert.IsNull (_cache.GetEnumValueInfo (FileState.Normal));

      _cache.AddEnumValueInfo (FileState.Normal, PropertyStates.FileStateNormal);
      Assert.AreSame (PropertyStates.FileStateNew, _cache.GetEnumValueInfo (FileState.New));
      Assert.AreSame (PropertyStates.FileStateNormal, _cache.GetEnumValueInfo (FileState.Normal));
    }

    [Test]
    public void CacheAccessTypes ()
    {
      Assert.IsFalse (_cache.ContainsAccessType (DomainAccessTypes.Journalize));
      Assert.IsNull (_cache.GetAccessType (DomainAccessTypes.Journalize));

      _cache.AddAccessType (DomainAccessTypes.Journalize, AccessTypes.Journalize);
      Assert.AreSame (AccessTypes.Journalize, _cache.GetAccessType (DomainAccessTypes.Journalize));
      Assert.IsFalse (_cache.ContainsAccessType (DomainAccessTypes.Archive));
      Assert.IsNull (_cache.GetAccessType (DomainAccessTypes.Archive));

      _cache.AddAccessType (DomainAccessTypes.Archive, AccessTypes.Archive);
      Assert.AreSame (AccessTypes.Journalize, _cache.GetAccessType (DomainAccessTypes.Journalize));
      Assert.AreSame (AccessTypes.Archive, _cache.GetAccessType (DomainAccessTypes.Archive));
    }

    [Test]
    public void CacheAbstractRoles ()
    {
      Assert.IsFalse (_cache.ContainsAbstractRole (DomainAbstractRoles.Clerk));
      Assert.IsNull (_cache.GetAbstractRole (DomainAbstractRoles.Secretary));

      _cache.AddAbstractRole (DomainAbstractRoles.Clerk, AbstractRoles.Clerk);
      Assert.AreSame (AbstractRoles.Clerk, _cache.GetAbstractRole (DomainAbstractRoles.Clerk));
      Assert.IsFalse (_cache.ContainsAbstractRole (DomainAbstractRoles.Secretary));
      Assert.IsNull (_cache.GetAbstractRole (DomainAbstractRoles.Secretary));

      _cache.AddAbstractRole (DomainAbstractRoles.Secretary, AbstractRoles.Secretary);
      Assert.AreSame (AbstractRoles.Clerk, _cache.GetAbstractRole (DomainAbstractRoles.Clerk));
      Assert.AreSame (AbstractRoles.Secretary, _cache.GetAbstractRole (DomainAbstractRoles.Secretary));
    }

    [Test]
    public void GetCachedSecurableClassInfos ()
    {
      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      _cache.AddSecurableClassInfo (typeof (File), fileTypeInfo);
      _cache.AddSecurableClassInfo (typeof (PaperFile), paperFileTypeInfo);

      List<SecurableClassInfo> infos = _cache.GetSecurableClassInfos ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (fileTypeInfo, infos);
      Assert.Contains (paperFileTypeInfo, infos);
    }

    [Test]
    public void GetCachedPropertyInfos ()
    {
      PropertyInfo confidentialityProperty = typeof (PaperFile).GetProperty ("Confidentiality");
      Assert.IsNotNull (confidentialityProperty);

      PropertyInfo stateProperty = typeof (PaperFile).GetProperty ("State");
      Assert.IsNotNull (stateProperty);

      StatePropertyInfo confidentialityPropertyInfo = new StatePropertyInfo ();
      StatePropertyInfo statePropertyInfo = new StatePropertyInfo ();

      _cache.AddStatePropertyInfo (confidentialityProperty, confidentialityPropertyInfo);
      _cache.AddStatePropertyInfo (stateProperty, statePropertyInfo);

      List<StatePropertyInfo> infos = _cache.GetStatePropertyInfos ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (confidentialityPropertyInfo, infos);
      Assert.Contains (statePropertyInfo, infos);
    }

    [Test]
    public void GetCachedAccessTypes ()
    {
      _cache.AddAccessType (DomainAccessTypes.Journalize, AccessTypes.Journalize);
      _cache.AddAccessType (DomainAccessTypes.Archive, AccessTypes.Archive);

      List<EnumValueInfo> infos = _cache.GetAccessTypes ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (AccessTypes.Journalize, infos);
      Assert.Contains (AccessTypes.Archive, infos);
    }

    [Test]
    public void GetCachedAbstractRoles ()
    {
      _cache.AddAbstractRole (DomainAbstractRoles.Clerk, AbstractRoles.Clerk);
      _cache.AddAbstractRole (DomainAbstractRoles.Secretary, AbstractRoles.Secretary);

      List<EnumValueInfo> infos = _cache.GetAbstractRoles ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (AbstractRoles.Clerk, infos);
      Assert.Contains (AbstractRoles.Secretary, infos);
    }
  }
}
