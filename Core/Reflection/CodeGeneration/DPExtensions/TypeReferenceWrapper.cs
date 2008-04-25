using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class TypeReferenceWrapper : TypeReference
  {
    private readonly Reference _referenceToWrap;
    private readonly Type _referenceType;

    public TypeReferenceWrapper (Reference referenceToWrap, Type referenceType)
      : base (referenceToWrap.OwnerReference, referenceType)
    {
      ArgumentUtility.CheckNotNull ("referenceToWrap", referenceToWrap);
      ArgumentUtility.CheckNotNull ("referenceType", referenceType);

      _referenceToWrap = referenceToWrap;
      _referenceType = referenceType;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      _referenceToWrap.LoadAddressOfReference (gen);
    }

    public override void LoadReference (ILGenerator gen)
    {
      _referenceToWrap.LoadReference (gen);
    }

    public override void StoreReference (ILGenerator gen)
    {
      _referenceToWrap.StoreReference (gen);
    }
  }
}
