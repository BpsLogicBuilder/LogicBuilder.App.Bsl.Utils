using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.Attributes;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.Forms.Parameters.Expressions;

namespace LogicBuilder.App.Bsl.Utils
{
    public static class DeleteOperationUtils<TModel, TData> where TModel : BaseModel where TData : BaseData
    {
        [AlsoKnownAs("Delete")]
        [FunctionGroup(FunctionGroup.Standard)]
        public static bool Delete(IDeleteOperations deleteOperations, FilterLambdaOperatorParameters filterExpression)
            => deleteOperations.Delete<TModel, TData>(filterExpression);
    }
}
