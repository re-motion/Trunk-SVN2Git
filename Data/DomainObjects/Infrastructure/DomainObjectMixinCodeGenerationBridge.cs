using System;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Reflection.CodeGeneration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Forms a bridge between domain objects and mixins by supporting generation and deserialization of mixed domain objects.
  /// </summary>
  /// <remarks>All of the methods in this class are tolerant agains non-mixed types, i.e. they will perform default/no-op actions if a non-mixed
  /// type (or object) is passed to them rather than throwing exceptions.</remarks>
  public static class DomainObjectMixinCodeGenerationBridge
  {
    internal class DummyObjectReference : IObjectReference
    {
      private readonly object _realObject;

      public DummyObjectReference (Type concreteDeserializedType, SerializationInfo info, StreamingContext context)
      {
        try
        {
          _realObject = Activator.CreateInstance (
              concreteDeserializedType,
              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
              null,
              new object[] { info, context },
              null);
        }
        catch (MissingMethodException ex)
        {
          throw new MissingMethodException ("No deserialization constructor was found on type " + concreteDeserializedType.FullName + ".", ex);
        }
      }

      public object GetRealObject (StreamingContext context)
      {
        return _realObject;
      }
    }

    public static Type GetConcreteType (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      return TypeFactory.GetConcreteType (domainObjectType);
    }

    public static void PrepareUnconstructedInstance (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      
      IMixinTarget instanceAsMixinTarget = instance as IMixinTarget;
      if (instanceAsMixinTarget != null)
        TypeFactory.InitializeUnconstructedInstance (instanceAsMixinTarget);
    }

    public static void SerializeMetadata (DomainObject instance, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddValue ("IsMixed", instance is IMixinTarget);
    }

    public static IObjectReference BeginDeserialization (Type publicDomainObjectType, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("baseType", publicDomainObjectType);
      ArgumentUtility.CheckNotNull ("info", info);

      IObjectReference objectReference;
      bool containsMixins = info.GetBoolean ("IsMixed");
      if (!containsMixins)
      {
        Type concreteType = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetConcreteDomainObjectType (publicDomainObjectType);
        objectReference = new DummyObjectReference (concreteType, info, context);
      }
      else
      {
        ClassDefinition baseTypeClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (publicDomainObjectType);
        Func<Type, Type> transformer = delegate (Type mixedType)
        {
          return DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetConcreteDomainObjectType (baseTypeClassDefinition, mixedType);
        };

        objectReference = Mixins.CodeGeneration.ConcreteTypeBuilder.Current.BeginDeserialization (transformer, info, context);
      }

      Assertion.IsNotNull (objectReference);
      Assertion.IsTrue (publicDomainObjectType.IsAssignableFrom (objectReference.GetRealObject(context).GetType ()));
      return objectReference;
    }

    public static void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);
      if (! (objectReference is DummyObjectReference))
        Mixins.CodeGeneration.ConcreteTypeBuilder.Current.FinishDeserialization (objectReference);
    }

    public static void OnDomainObjectCreated (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      NotifyDomainObjectMixins (instance, delegate (IDomainObjectMixin mixin) { mixin.OnDomainObjectCreated (); });
    }

    public static void OnDomainObjectLoaded (DomainObject instance, LoadMode loadMode)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      NotifyDomainObjectMixins (instance, delegate (IDomainObjectMixin mixin) { mixin.OnDomainObjectLoaded (loadMode); });
    }

    private static void NotifyDomainObjectMixins (DomainObject instance, Proc<IDomainObjectMixin> notifier)
    {
      IMixinTarget instanceAsMixinTarget = instance as IMixinTarget;
      if (instanceAsMixinTarget != null)
      {
        foreach (object mixin in instanceAsMixinTarget.Mixins)
        {
          IDomainObjectMixin mixinAsDomainObjectMixin = mixin as IDomainObjectMixin;
          if (mixinAsDomainObjectMixin != null)
            notifier (mixinAsDomainObjectMixin);
        }
      }
    }

    public static void RaiseOnDeserializing (DomainObject instance, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
       
      // Only raise event if the mixin infrastructure hasn't already done it
      if (!(instance is IMixinTarget))
        SerializationImplementer.RaiseOnDeserializing (instance, context);
    }

    public static void RaiseOnDeserialized (DomainObject instance, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      // Only raise event if the mixin infrastructure hasn't already done it
      if (!(instance is IMixinTarget))
        SerializationImplementer.RaiseOnDeserialized (instance, context);
    }

    public static void RaiseOnDeserialization (DomainObject instance, object sender)
    {
      // Only raise event if the mixin infrastructure hasn't already done it
      if (!(instance is IMixinTarget))
        SerializationImplementer.RaiseOnDeserialization (instance, sender);
    }
  }
}