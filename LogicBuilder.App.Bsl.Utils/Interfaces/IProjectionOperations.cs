using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Interfaces
{
    public interface IProjectionOperations
    {
        Task<TModel?> Get<TModel, TData>(
            IExpressionParameter filterExpression,
            IExpressionParameter? queryFunc = null,
            SelectExpandDefinitionParameters? expansion = null) where TModel : BaseModel where TData : BaseData;

        Task<ICollection<TModel>> GetItems<TModel, TData>(
            IExpressionParameter? filterExpression = null,
            IExpressionParameter? queryFunc = null,
            SelectExpandDefinitionParameters? expansion = null) where TModel : BaseModel where TData : BaseData;
    }
}
