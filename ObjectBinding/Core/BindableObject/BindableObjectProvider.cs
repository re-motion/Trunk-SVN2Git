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
using System.Collections;
using Remotion.Collections;
using Remotion.Utilities;
using TypeUtility=Remotion.Mixins.TypeUtility;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The implementation for the <see cref="IBusinessObjectProvider"/> interface for the reflection based business object layer.
  /// </summary>
  public class BindableObjectProvider : BusinessObjectProvider
  {
    [Obsolete ("Use BusinessObjectProvider.GetProvider instead. (Version 1.9.1.0)", true)]
    public static BindableObjectProvider Current
    {
      get { throw new NotImplementedException ("Obsolete. Use BusinessObjectProvider.GetProvider instead. (Version 1.9.1.0)"); }
    }

    [Obsolete ("Use BusinessObjectProvider.SetProvider instead. (Version 1.9.1.0)", true)]
    public static void SetCurrent (BindableObjectProvider provider)
    {
      throw new NotImplementedException ("Obsolete. Use BusinessObjectProvider.GetProvider instead. (Version 1.9.1.0)");
    }

    /// <summary>
    /// Use this method as a shortcut to retrieve the <see cref="BindableObjectProvider"/> for a <see cref="Type"/> 
    /// that has the <see cref="BindableObjectMixinBase{T}"/> applied without first retrieving the matching provider.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to retrieve the <see cref="BindableObjectProvider"/> for.</param>
    /// <returns>Returns the <see cref="BindableObjectProvider"/> for the <paramref name="type"/>.</returns>
    public static BindableObjectProvider GetProviderForBindableObjectType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Type concreteType = TypeUtility.GetConcreteMixedType (type);
      BusinessObjectProviderAttribute attribute = AttributeUtility.GetCustomAttribute<BusinessObjectProviderAttribute> (concreteType, true);

      if (attribute == null)
      {
        throw new ArgumentException (
            string.Format ("The type '{0}' does not have the '{1}' applied.", type.FullName, typeof (BusinessObjectProviderAttribute).FullName),
            "type");
      }

      if (!ReflectionUtility.CanAscribe (attribute.BusinessObjectProviderType, typeof (BindableObjectProvider)))
      {
        throw new ArgumentException (
            string.Format (
                "The business object provider associated with the type '{0}' is not of type '{1}'.",
                type.FullName,
                typeof (BindableObjectProvider).FullName),
            "type");
      }

      return (BindableObjectProvider) GetProvider (attribute.GetType());
    }

    /// <summary>
    /// Use this method as a shortcut to retrieve the <see cref="BindableObjectClass"/> for a <see cref="Type"/> 
    /// that has the <see cref="BindableObjectMixinBase{T}"/> applied without first retrieving the matching provider.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to retrieve the <see cref="BindableObjectClass"/> for.</param>
    /// <returns>Returns the <see cref="BindableObjectClass"/> for the <paramref name="type"/>.</returns>
    public static BindableObjectClass GetBindableObjectClassFromProvider (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      BindableObjectProvider provider = GetProviderForBindableObjectType (type);
      Assertion.IsNotNull (provider, "No BindableObjectProvider associated with type '{0}'.", type.FullName);

      return provider.GetBindableObjectClass (type);
    }

    private readonly InterlockedDataStore<Type, BindableObjectClass> _businessObjectClassStore = new InterlockedDataStore<Type, BindableObjectClass>();
    private readonly InterlockedDataStore<Type, IBusinessObjectService> _serviceStore = new InterlockedDataStore<Type, IBusinessObjectService>();
    private readonly IMetadataFactory _metadataFactory;

    public BindableObjectProvider ()
      : this (BindableObjectMetadataFactory.Create(), BindableObjectServiceFactory.Create ())
    {
    }

    protected BindableObjectProvider (IMetadataFactory metadataFactory)
      : this (metadataFactory, BindableObjectServiceFactory.Create())
    {
    }

    public BindableObjectProvider (IMetadataFactory metadataFactory, IBusinessObjectServiceFactory serviceFactory)
      : base (serviceFactory)
    {
      ArgumentUtility.CheckNotNull ("metadataFactory", metadataFactory);
      ArgumentUtility.CheckNotNull ("serviceFactory", serviceFactory);

      _metadataFactory = metadataFactory;
    }

    /// <summary>
    /// Gets the <see cref="BindableObjectClass"/> for the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to get a <see cref="BindableObjectClass"/> for. This type must have a mixin derived from
    /// <see cref="BindableObjectMixinBase{TBindableObject}"/> configured, and it is recommended to specify the simple target type rather then the
    /// generated mixed type.</param>
    /// <returns>A <see cref="BindableObjectClass"/> for the given <paramref name="type"/>.</returns>
    public BindableObjectClass GetBindableObjectClass (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _businessObjectClassStore.GetOrCreateValue (type, CreateBindableObjectClass);
    }

    /// <summary>Gets the MetadataFactory for this <see cref="BindableObjectProvider"/></summary>
    public IMetadataFactory MetadataFactory
    {
      get { return _metadataFactory; }
    }

    /// <summary> The <see cref="IDictionary"/> used to store the references to the registered servies. </summary>
    protected override IDataStore<Type, IBusinessObjectService> ServiceStore
    {
      get { return _serviceStore; }
    }

    private BindableObjectClass CreateBindableObjectClass (Type type)
    {
      IClassReflector classReflector = _metadataFactory.CreateClassReflector (type, this);
      Assertion.IsNotNull (classReflector, "The IMetadataFactory.CreateClassReflector method evaluated and returned null.");

      return classReflector.GetMetadata();
    }
  }
}
