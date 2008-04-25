using System;
using System.Collections;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectProvider : BusinessObjectProvider
  {
    private static readonly DoubleCheckedLockingContainer<BindableObjectProvider> s_current =
        new DoubleCheckedLockingContainer<BindableObjectProvider> (CreateBindableObjectProvider);

    public static BindableObjectProvider Current
    {
      get { return s_current.Value; }
    }

    public static void SetCurrent (BindableObjectProvider provider)
    {
      s_current.Value = provider;
    }

    private static BindableObjectProvider CreateBindableObjectProvider ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();
      provider.AddService (typeof (IBindableObjectGlobalizationService), new BindableObjectGlobalizationService());
      provider.AddService (typeof (IBusinessObjectStringFormatterService), new BusinessObjectStringFormatterService());

      return provider;
    }

    private readonly InterlockedCache<Type, BindableObjectClass> _businessObjectClassCache = new InterlockedCache<Type, BindableObjectClass>();
    private readonly InterlockedDataStore<Type, IBusinessObjectService> _serviceStore = new InterlockedDataStore<Type, IBusinessObjectService>();

    public BindableObjectProvider ()
    {
    }

    /// <summary>
    /// Gets the <see cref="BindableObjectClass"/> for the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to get a <see cref="BindableObjectClass"/> for. This type must have a mixin derived from
    /// <see cref="BindableObjectMixinBase{TBindableObject}"/> configured, and it is recommended to specify the simple target type rather then the
    /// generated mixed type.</param>
    /// <returns>A <see cref="BindableObjectClass"/> for the given <paramref name="type"/>.</returns>
    public virtual BindableObjectClass GetBindableObjectClass (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _businessObjectClassCache.GetOrCreateValue (type, delegate (Type classType) { return CreateBindableObjectClass (classType); });
    }

    private BindableObjectClass CreateBindableObjectClass (Type type)
    {
      ClassReflector classReflector = new ClassReflector (type, this, GetMetadataFactoryForType (type));
      return classReflector.GetMetadata ();
    }

    /// <summary>
    /// Gets the <see cref="IMetadataFactory"/> for the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type for which an <see cref="IMetadataFactory"/> is needed..</param>
    /// <returns><see cref="DefaultMetadataFactory.Instance"/> by default, unless the <paramref name="type"/> is configured to use a specific
    /// <see cref="IMetadataFactory"/> implementation (via a subclass of <see cref="UseCustomMetadataFactoryAttribute"/>), in which case an instance of
    /// that implementation is returned.</returns>
    public virtual IMetadataFactory GetMetadataFactoryForType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      Type concreteType = Mixins.TypeUtility.GetConcreteMixedType (type);
      UseCustomMetadataFactoryAttribute attribute =
          (UseCustomMetadataFactoryAttribute) AttributeUtility.GetCustomAttribute (concreteType, typeof (UseCustomMetadataFactoryAttribute), true);
      if (attribute != null)
        return attribute.GetFactoryInstance ();
      else
        return DefaultMetadataFactory.Instance;
    }

    /// <summary> The <see cref="IDictionary"/> used to store the references to the registered servies. </summary>
    protected override IDataStore<Type, IBusinessObjectService> ServiceStore
    {
      get { return _serviceStore; }
    }
  }
}