using System;

namespace Remotion.ObjectBinding.BindableObject
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class UseCustomMetadataFactoryAttribute : Attribute
  {
    public abstract IMetadataFactory GetFactoryInstance ();
  }
}