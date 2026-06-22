using AutoMapper;
using LogicBuilder.App.Bsl.Utils.Tests.Data.Stores;
using LogicBuilder.EntityFrameworkCore.Repositories;

namespace LogicBuilder.App.Bsl.Utils.Tests.Models.Repositories
{
    public class DataClassesRepository(IDataClassesStore store, IMapper mapper) : ContextRepositoryBase(store, mapper), IDataClassesRepository
    {
    }
}
