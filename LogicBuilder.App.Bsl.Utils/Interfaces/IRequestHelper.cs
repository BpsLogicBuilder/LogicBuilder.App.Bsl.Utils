using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Interfaces
{
    public interface IRequestHelper
    {
        Task<GetObjectListResponse> GetAnonymousList(GetObjectListRequest request);
        Task<GetObjectListResponse> GetAnonymousList<TModel, TData>(GetObjectListRequest request) where TModel : BaseModel where TData : BaseData;
        Task<GetEntityResponse> GetEntity(GetEntityRequest request);
        Task<GetEntityResponse> GetEntity<TModel, TData>(GetEntityRequest request) where TModel : BaseModel where TData : BaseData;
        Task<GetListResponse> GetList(GetTypedListRequest request);
#pragma warning disable S2436
        Task<GetListResponse> GetList<TModel, TData, TModelReturn, TDataReturn>(GetTypedListRequest request) where TModel : BaseModel where TData : BaseData; //NOSSONAR needed for mapping
#pragma warning restore S2436
    }
}
