using System;
using System.Collections.Generic;
using System.Text;

using Remotion.Utilities;
using System.Web.Compilation;
using System.CodeDom;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Web.UI.Globalization;

namespace Remotion.Web.Compilation
{
  //TODO: Check if this is realy the optimal solution
  [ExpressionPrefix ("res")]
  public class ResourceExpressionBuilder : ExpressionBuilder
  {
    // constants

    // types

    // static members

    public static object GetResourceString (Control parent, string resourceID)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNullOrEmpty ("resourceID", resourceID);

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (parent, true);
      if (resourceManager == null)
        throw new InvalidOperationException ("Remotion.Web.Compilation.ResourceExpressionBuilder can only be used on controls embedded within a parent implementing IObjectWithResources.");
      return resourceManager.GetString (resourceID);
    }

    // member fields

    // construction and disposing

    public ResourceExpressionBuilder ()
    {
    }

    // methods and properties

    public override CodeExpression GetCodeExpression (BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
    {
      Tuple<string, Type> entryTuple = (Tuple<string, Type>) parsedData;
      CodeMethodInvokeExpression expression = new CodeMethodInvokeExpression ();
      expression.Method.TargetObject = new CodeTypeReferenceExpression (base.GetType ());
      expression.Method.MethodName = "GetResourceString";
      expression.Parameters.Add (new CodeThisReferenceExpression ());
      expression.Parameters.Add (new CodePrimitiveExpression (entryTuple.A));

      return expression;
    }
 
    public override object ParseExpression (string expression, Type propertyType, ExpressionBuilderContext context)
    {
      return new Tuple<string, Type> (expression, propertyType);
    }
  }
}