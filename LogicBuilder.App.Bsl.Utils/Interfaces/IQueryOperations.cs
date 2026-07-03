using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Interfaces
{
    public interface IQueryOperations
    {
#pragma warning disable S2436//Generic arguments needed for mapping
        Task<TModelReturn> Query<TModel, TData, TModelReturn, TDataReturn>(IExpressionParameter queryExpression,
            SelectExpandDefinitionParameters? expansion = null) where TModel : LogicBuilder.Domain.BaseModel where TData : LogicBuilder.Data.BaseData;
#pragma warning restore S2436
    }
}
