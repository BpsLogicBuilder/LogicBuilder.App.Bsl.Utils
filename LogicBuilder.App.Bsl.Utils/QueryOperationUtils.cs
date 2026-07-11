using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.Attributes;
using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;

namespace LogicBuilder.App.Bsl.Utils
{
#pragma warning disable S2436//Generic arguments needed for mapping
    public static class QueryOperationUtils<TModel, TData, TModelReturn, TDataReturn> where TModel : LogicBuilder.Domain.BaseModel where TData : LogicBuilder.Data.BaseData
#pragma warning restore S2436
    {
        [AlsoKnownAs("Query")]
        [FunctionGroup(FunctionGroup.Standard)]
        public static TModelReturn Query(IQueryOperations queryOperations,
            IExpressionParameter queryExpression,
            SelectExpandDefinitionParameters? expansion = null)
            => queryOperations.Query<TModel, TData, TModelReturn, TDataReturn>
            (
                queryExpression,
                expansion
            ).GetAwaiter().GetResult();
    }
}
