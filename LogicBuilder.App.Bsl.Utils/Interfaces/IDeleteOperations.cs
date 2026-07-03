using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.Forms.Parameters.Expressions;

namespace LogicBuilder.App.Bsl.Utils.Interfaces
{
    public interface IDeleteOperations
    {
        bool Delete<TModel, TData>(FilterLambdaOperatorParameters filterExpression) where TModel : BaseModel where TData : BaseData;
    }
}
