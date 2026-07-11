using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.App.Utils.Interfaces;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils
{
    public class QueryOperations(IContextRepository contextRepository, IMappingOperations mappingOperations) : IQueryOperations
    {
        private readonly IContextRepository _contextRepository = contextRepository;
        private readonly IMappingOperations _mappingOperations = mappingOperations;

#pragma warning disable S2436//Generic arguments needed for mapping
        public Task<TModelReturn> Query<TModel, TData, TModelReturn, TDataReturn>(IExpressionParameter queryExpression, SelectExpandDefinitionParameters? expansion = null) where TModel : LogicBuilder.Domain.BaseModel where TData : LogicBuilder.Data.BaseData
#pragma warning restore S2436
        => _contextRepository.QueryAsync<TModel, TData, TModelReturn, TDataReturn>
        (
            GetQueryFunc<TModel, TModelReturn>(_mappingOperations.MapToOperator(queryExpression)),
            expansion == null ? null : _mappingOperations.MapExpansion(expansion)
        );

        private static Expression<Func<IQueryable<TModel>, TModelReturn>> GetQueryFunc<TModel, TModelReturn>(IExpressionPart selectorExpression)
           => (Expression<Func<IQueryable<TModel>, TModelReturn>>)selectorExpression.Build();
    }
}
