using LogicBuilder.EntityFrameworkCore.Crud.DataStores;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data.Stores
{
    public class SchoolStore(SchoolContext context) : StoreBase(context), ISchoolStore
    {
    }
}
