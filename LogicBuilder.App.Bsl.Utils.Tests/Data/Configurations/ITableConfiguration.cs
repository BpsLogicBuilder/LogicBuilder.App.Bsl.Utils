using Microsoft.EntityFrameworkCore;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data.Configurations
{
    interface ITableConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
