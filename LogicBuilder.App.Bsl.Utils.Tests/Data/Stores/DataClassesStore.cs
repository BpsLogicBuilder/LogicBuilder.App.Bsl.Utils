using LogicBuilder.EntityFrameworkCore.Crud.DataStores;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data.Stores
{
    public class DataClassesStore(DataClassesContext context) : StoreBase(context), IDataClassesStore
    {
    }
}
