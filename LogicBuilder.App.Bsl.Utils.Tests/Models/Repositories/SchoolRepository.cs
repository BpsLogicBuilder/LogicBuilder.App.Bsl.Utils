using AutoMapper;
using LogicBuilder.App.Bsl.Utils.Tests.Data.Stores;
using LogicBuilder.EntityFrameworkCore.Repositories;

namespace LogicBuilder.App.Bsl.Utils.Tests.Models.Repositories
{
    public class SchoolRepository(ISchoolStore store, IMapper mapper) : ContextRepositoryBase(store, mapper), ISchoolRepository
    {
    }
}
