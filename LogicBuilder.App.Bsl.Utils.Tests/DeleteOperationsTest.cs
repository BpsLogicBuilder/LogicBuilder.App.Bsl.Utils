using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.App.Bsl.Utils.Tests.AutoMapperProfiles;
using LogicBuilder.App.Bsl.Utils.Tests.Data;
using LogicBuilder.App.Bsl.Utils.Tests.Data.Stores;
using LogicBuilder.App.Bsl.Utils.Tests.Models;
using LogicBuilder.App.Bsl.Utils.Tests.Models.Repositories;
using LogicBuilder.App.Common.Utils;
using LogicBuilder.App.Common.Utils.Interfaces;
using LogicBuilder.EntityFrameworkCore.Mapping;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Forms.Parameters.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Tests
{
    public class DeleteOperationsTest : IClassFixture<DatabaseFixture>
    {
        static DeleteOperationsTest()
        {
            InitializeMapperConfiguration();
        }

        public DeleteOperationsTest(DatabaseFixture databaseFixture)
        {
            this.databaseFixture = databaseFixture;
            Initialize();
        }

        [Fact]
        public async Task Delete_A_Single_Entity()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            IDeleteOperations deleteOperations = serviceProvider?.GetRequiredService<IDeleteOperations>()!;
            string fullName = "Carson Alexander";
            var filter = new FilterLambdaOperatorParameters
            (
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("FullName", new ParameterOperatorParameters("f")),
                    new ConstantOperatorParameters(fullName)
                ),
                typeof(StudentModel),
                "f"
            );
            var before = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == fullName)).SingleOrDefault();

            //act
            DeleteOperations<StudentModel, Student>.Delete(deleteOperations, filter);
            var after = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == fullName)).SingleOrDefault();

            //assert
            Assert.NotNull(before);
            Assert.Null(after);
        }

        #region Fields
        private readonly DatabaseFixture databaseFixture;
        private static MapperConfiguration MapperConfiguration;
        private static IServiceProvider? serviceProvider;
        #endregion Fields

        #region Helpers
        [MemberNotNull(nameof(MapperConfiguration))]
        private static void InitializeMapperConfiguration()
        {
            MapperConfiguration ??= ConfigurationHelper.GetMapperConfiguration(cfg =>
            {
                cfg.AddExpressionMapping();

                cfg.AddProfile<ExpressionOperatorsMappingProfile>();
                cfg.AddProfile<ExpressionParameterToDescriptorMappingProfile>();
                cfg.AddProfile<SchoolProfile>();
                cfg.AddProfile<ExpansionParameterToDescriptorMappingProfile>();
                cfg.AddProfile<ExpansionDescriptorToOperatorMappingProfile>();
            });
            MapperConfiguration.AssertConfigurationIsValid();
        }

        [MemberNotNull(nameof(serviceProvider))]
        private void Initialize()
        {
            serviceProvider ??= new ServiceCollection()
                .AddDbContext<SchoolContext>
                (
                    options => options.UseSqlServer
                    (
                        databaseFixture.GetConnectionString(GetType().Name),
                        options => options.EnableRetryOnFailure()
                    ),
                    ServiceLifetime.Transient
                )
                .AddAppCommonUtilsServices()
                .AddBslUtilsServices()
                .AddTransient<ISchoolStore, SchoolStore>()
                .AddTransient<IContextRepository, SchoolRepository>()
                .AddTransient<ISchoolRepository, SchoolRepository>()
                .AddSingleton<AutoMapper.IConfigurationProvider>
                (
                    MapperConfiguration
                )
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService))
                .BuildServiceProvider();

            ReCreateDataBase(serviceProvider.GetRequiredService<SchoolContext>()).GetAwaiter().GetResult();
            DatabaseSeeder.Seed_Database(serviceProvider.GetRequiredService<ISchoolRepository>()).GetAwaiter().GetResult();
        }

        private static async Task ReCreateDataBase(SchoolContext context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        #endregion Helpers
    }
}
