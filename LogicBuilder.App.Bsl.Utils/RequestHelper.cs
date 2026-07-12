using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.App.Utils.Interfaces;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Expressions.Utils.Expansions;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils
{
    public class RequestHelper(IContextRepository contextRepository, IMappingOperations mappingOperations) : IRequestHelper
    {
        private readonly IContextRepository _contextRepository = contextRepository;
        private readonly IMappingOperations _mappingOperations = mappingOperations;

        public async Task<GetObjectListResponse> GetAnonymousList(GetObjectListRequest request)
        {
            MethodInfo methodInfo = GetMethodInfo(nameof(GetAnonymousList)).MakeGenericMethod
            (
                Type.GetType(request.ModelType ?? "") ?? throw new InvalidOperationException($"Model type {request.ModelType} is invalid. Provide a valid asembly qualified type name."),
                Type.GetType(request.DataType ?? "") ?? throw new InvalidOperationException($"Data type {request.DataType} is invalid. Provide a valid asembly qualified type name.")
            );

            return await (Task<GetObjectListResponse>)(methodInfo.Invoke(this, [request])!);//Generic GetAnonymousList never returns null
        }

        public async Task<GetObjectListResponse> GetAnonymousList<TModel, TData>(GetObjectListRequest request)
            where TModel : BaseModel
            where TData : BaseData 
            => new()
            {
                List = await Query<TModel, TData, IEnumerable<dynamic>, IEnumerable<dynamic>>
                (
                    _mappingOperations.MapToOperator(request.Selector ?? throw new InvalidOperationException($"Selector is required."))
                ),
                Success = true
            };

        public async Task<GetEntityResponse> GetEntity(GetEntityRequest request)
        {
            MethodInfo methodInfo = GetMethodInfo(nameof(GetEntity)).MakeGenericMethod
            (
                Type.GetType(request.ModelType ?? "") ?? throw new InvalidOperationException($"Model type {request.ModelType} is invalid. Provide a valid asembly qualified type name."),
                Type.GetType(request.DataType ?? "") ?? throw new InvalidOperationException($"Data type {request.DataType} is invalid. Provide a valid asembly qualified type name.")
            );

            return await (Task<GetEntityResponse>)(methodInfo.Invoke(this, [request])!);//Generic GetEntity never returns null
        }

        public async Task<GetEntityResponse> GetEntity<TModel, TData>(GetEntityRequest request)
            where TModel : BaseModel
            where TData : BaseData 
            => new()
            {
                Entity = await QueryEntity<TModel, TData>
                (
                    _mappingOperations.MapToOperator(request.Filter ?? throw new InvalidOperationException($"Filter is required.")),
                    request.SelectExpandDefinition == null ? null : _mappingOperations.MapExpansion(request.SelectExpandDefinition)
                ),
                Success = true
            };

        public async Task<GetListResponse> GetList(GetTypedListRequest request)
        {
            MethodInfo methodInfo = GetMethodInfo(nameof(GetList)).MakeGenericMethod
            (
                Type.GetType(request.ModelType ?? "") ?? throw new InvalidOperationException($"Model type {request.ModelType} is invalid. Provide a valid asembly qualified type name."),
                Type.GetType(request.DataType ?? "") ?? throw new InvalidOperationException($"Data type {request.DataType} is invalid. Provide a valid asembly qualified type name."),
                Type.GetType(request.ModelReturnType ?? "") ?? throw new InvalidOperationException($"Model return type {request.ModelReturnType} is invalid. Provide a valid asembly qualified type name."),
                Type.GetType(request.DataReturnType ?? "") ?? throw new InvalidOperationException($"Data return type {request.DataReturnType} is invalid. Provide a valid asembly qualified type name.")
            );

            return await (Task<GetListResponse>)(methodInfo.Invoke(this, [request])!);//Generic GetList never returns null
        }

#pragma warning disable S2436
        public async Task<GetListResponse> GetList<TModel, TData, TModelReturn, TDataReturn>(GetTypedListRequest request) //NOSSONAR needed for mapping
            where TModel : BaseModel
            where TData : BaseData
#pragma warning restore S2436
        {
            return new GetListResponse
            {
                List = (IEnumerable<BaseModel>)(await Query<TModel, TData, TModelReturn, TDataReturn>
                (
                    _mappingOperations.MapToOperator(request.Selector ?? throw new InvalidOperationException($"Selector is required.")),
                    request.SelectExpandDefinition == null ? null : _mappingOperations.MapExpansion(request.SelectExpandDefinition)
                ))!,//List query does not return null
                Success = true
            };
        }

        private static MethodInfo GetMethodInfo(string methodName)
           => typeof(RequestHelper).GetMethods().Single(m => m.Name == methodName && m.IsGenericMethod);

#pragma warning disable S2436
        private Task<TModelReturn> Query<TModel, TData, TModelReturn, TDataReturn>(//NOSSONAR needed for mapping
#pragma warning restore S2436
            IExpressionPart queryExpression,
            SelectExpandDefinition? selectExpandDefinition = null)
            where TModel : BaseModel
            where TData : BaseData
            => _contextRepository.QueryAsync<TModel, TData, TModelReturn, TDataReturn>
            (
                (Expression<Func<IQueryable<TModel>, TModelReturn>>)queryExpression.Build(),
                selectExpandDefinition
            );

        private async Task<TModel?> QueryEntity<TModel, TData>(IExpressionPart filterExpression, SelectExpandDefinition? selectExpandDefinition = null)
            where TModel : BaseModel
            where TData : BaseData
            => (
                    await _contextRepository.GetAsync<TModel, TData>
                    (
                        (Expression<Func<TModel, bool>>)filterExpression.Build(),
                        null,
                        selectExpandDefinition
                    )
               ).FirstOrDefault();
    }
}
