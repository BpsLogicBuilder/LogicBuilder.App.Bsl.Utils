using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.App.Utils.Interfaces;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils
{
    public class ProjectionOperations(IContextRepository contextRepository, IMappingOperations mappingOperations) : IProjectionOperations
    {
        private readonly IContextRepository _contextRepository = contextRepository;
        private readonly IMappingOperations _mappingOperations = mappingOperations;

        public async Task<TModel?> Get<TModel, TData>(IExpressionParameter filterExpression, IExpressionParameter? queryFunc = null, SelectExpandDefinitionParameters? expansion = null)
            where TModel : BaseModel
            where TData : BaseData
            => (await GetItems<TModel, TData>(filterExpression, queryFunc, expansion)).SingleOrDefault();

        public Task<ICollection<TModel>> GetItems<TModel, TData>(IExpressionParameter? filterExpression = null, IExpressionParameter? queryFunc = null, SelectExpandDefinitionParameters? expansion = null)
            where TModel : BaseModel
            where TData : BaseData
            => _contextRepository.GetAsync<TModel, TData>
            (
                filterExpression == null ? null : GetFilter<TModel>(_mappingOperations.MapToOperator(filterExpression)),
                queryFunc == null ? null : GetQueryFunc<TModel>(_mappingOperations.MapToOperator(queryFunc)),
                expansion == null ? null : _mappingOperations.MapExpansion(expansion)
            );

        private static Expression<Func<TModel, bool>> GetFilter<TModel>(IExpressionPart filterExpression)
            => (Expression<Func<TModel, bool>>)filterExpression.Build();

        private static Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> GetQueryFunc<TModel>(IExpressionPart selectorExpression)
            => (Expression<Func<IQueryable<TModel>, IQueryable<TModel>>>)selectorExpression.Build();
    }
}
