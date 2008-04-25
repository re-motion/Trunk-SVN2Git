using System;
using System.ComponentModel;

namespace Remotion.ObjectBinding.Design
{
  internal class EditorControlBaseClassProvider : TypeDescriptionProvider
  {
    public EditorControlBaseClassProvider ()
        : base (TypeDescriptor.GetProvider (typeof (EditorControlBase)))
    {
    }

    public override Type GetReflectionType (Type objectType, object instance)
    {
      if (objectType == typeof (EditorControlBase))
        return typeof (ConcreteEditorControlBase);

      return base.GetReflectionType (objectType, instance);
    }

    public override object CreateInstance (IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
    {
      if (objectType == typeof (EditorControlBase))
        objectType = typeof (ConcreteEditorControlBase);

      return base.CreateInstance (provider, objectType, argTypes, args);
    }
  }
}