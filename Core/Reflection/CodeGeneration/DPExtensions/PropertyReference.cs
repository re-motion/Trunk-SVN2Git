using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Reflection;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class PropertyReference : TypeReference
  {
    private readonly PropertyInfo _property;

    public PropertyReference (PropertyInfo property)
        : base (SelfReference.Self, property.PropertyType)
    {
      _property = property;
    }

    public PropertyReference(Reference owner, PropertyInfo property)
        : base (owner, property.PropertyType)
    {
      _property = property;
    }

    public PropertyInfo Reference
    {
      get { return _property; }
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      throw new NotSupportedException ("A property's address cannot be loaded.");
    }

    public override void LoadReference (ILGenerator gen)
    {
      MethodInfo getMethod = Reference.GetGetMethod(true);
      if (getMethod == null)
      {
        string message = string.Format("The property {0}.{1} cannot be loaded, it has no getter.", Reference.DeclaringType.FullName, Reference.Name);
        throw new InvalidOperationException (message);
      }
      if (getMethod.IsStatic)
      {
        gen.EmitCall (OpCodes.Call, getMethod, null);
      }
      else
      {
        gen.EmitCall (OpCodes.Callvirt, getMethod, null);
      }
    }

    public override void StoreReference (ILGenerator gen)
    {
      MethodInfo setMethod = Reference.GetSetMethod (true);
      if (setMethod == null)
      {
        string message = string.Format ("The property {0}.{1} cannot be stored, it has no setter.", Reference.DeclaringType.FullName, Reference.Name);
        throw new InvalidOperationException (message);
      }
      if (setMethod.IsStatic)
      {
        gen.EmitCall (OpCodes.Call, setMethod, null);
      }
      else
      {
        gen.EmitCall (OpCodes.Callvirt, setMethod, null);
      }
    }
  }
}
