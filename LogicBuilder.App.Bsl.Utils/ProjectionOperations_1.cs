using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.Attributes;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;
using System.Collections.Generic;

namespace LogicBuilder.App.Bsl.Utils
{
    public static class ProjectionOperations<TModel, TData> where TModel : BaseModel where TData : BaseData
    {
        [AlsoKnownAs("GetSingle")]
        [FunctionGroup(FunctionGroup.Standard)]
        public static TModel? Get(IProjectionOperations projectionOperations,
            IExpressionParameter filterExpression,
            IExpressionParameter? queryFunc = null,
            SelectExpandDefinitionParameters? expansion = null)
            => projectionOperations.Get<TModel, TData>
            (
                filterExpression,
                queryFunc,
                expansion
            ).GetAwaiter().GetResult();

        [AlsoKnownAs("GetList")]
        [FunctionGroup(FunctionGroup.Standard)]
        public static ICollection<TModel> GetItems(IProjectionOperations projectionOperations,
            IExpressionParameter? filterExpression = null,
            IExpressionParameter? queryFunc = null,
            SelectExpandDefinitionParameters? expansion = null)
            => projectionOperations.GetItems<TModel, TData>
            (
                filterExpression,
                queryFunc,
                expansion
            ).GetAwaiter().GetResult();
    }
}
