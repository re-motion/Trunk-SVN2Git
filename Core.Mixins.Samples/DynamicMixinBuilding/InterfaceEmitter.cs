using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding
{
  internal class InterfaceEmitter : AbstractTypeEmitter
  {
    private static TypeBuilder CreateTypeBuilder (ModuleScope scope, string typeName)
    {
      return scope.ObtainDynamicModule (true).DefineType (typeName, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
    }

    public InterfaceEmitter (ModuleScope scope, string typeName)
        : base (CreateTypeBuilder (ArgumentUtility.CheckNotNull ("scope", scope), ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName)))
    {
    }

    protected override void EnsureBuildersAreInAValidState ()
    {
      // do not call base implementation, we don't want any method bodies or constructors
    }
  }
}