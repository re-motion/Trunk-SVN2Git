using System;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public static class AttributeReplicator
  {
    public static void ReplicateAttribute (IAttributableEmitter target, CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CustomAttributeBuilder builder = ReflectionEmitUtility.CreateAttributeBuilderFromData (attributeData);
      target.AddCustomAttribute (builder);
    }
  }
}
