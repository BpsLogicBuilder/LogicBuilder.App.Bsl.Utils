using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.App.Common.Utils.Interfaces;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Forms.Parameters.Expressions;
using System;
using System.Linq.Expressions;

namespace LogicBuilder.App.Bsl.Utils
{
    public class DeleteOperations(IContextRepository contextRepository, IMappingOperations mappingOperations) : IDeleteOperations
    {
        private readonly IContextRepository _contextRepository = contextRepository;
        private readonly IMappingOperations _mappingOperations = mappingOperations;

        public bool Delete<TModel, TData>(FilterLambdaOperatorParameters filterExpression)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _contextRepository.DeleteAsync<TModel, TData>
            (
                GetFilter<TModel>(_mappingOperations.MapToOperator(filterExpression))
            ).GetAwaiter().GetResult();
        }

        private static Expression<Func<TModel, bool>> GetFilter<TModel>(IExpressionPart filterExpression)
            => (Expression<Func<TModel, bool>>)filterExpression.Build();
    }
}
