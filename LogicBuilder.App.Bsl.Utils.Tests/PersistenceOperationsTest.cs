using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using LogicBuilder.App.Bsl.Utils.Tests.AutoMapperProfiles;
using LogicBuilder.App.Bsl.Utils.Tests.Data;
using LogicBuilder.App.Bsl.Utils.Tests.Data.Stores;
using LogicBuilder.App.Bsl.Utils.Tests.Models;
using LogicBuilder.App.Bsl.Utils.Tests.Models.Repositories;
using LogicBuilder.EntityFrameworkCore.Mapping;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Expressions.Utils.Expansions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Tests
{
    public class PersistenceOperationsTest : IClassFixture<DatabaseFixture>
    {
        static PersistenceOperationsTest()
        {
            InitializeMapperConfiguration();
        }

        public PersistenceOperationsTest(DatabaseFixture databaseFixture)
        {
            this.databaseFixture = databaseFixture;
            Initialize();
        }

        #region Fields
        private readonly DatabaseFixture databaseFixture;
        private static MapperConfiguration MapperConfiguration;
        private static IServiceProvider? serviceProvider;
        #endregion Fields

        [Fact]
        public async Task Add_A_Single_Entity_Without_Child_Objects_Using_AddChange()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.AddChange(schoolRepository, student);
            await schoolRepository.SaveChangesAsync();
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Empty(result.Enrollments!);
        }

        [Fact]
        public async Task Add_A_Entity_List_Without_Child_Objects_Using_AddChanges()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.AddChanges(schoolRepository, [student]);
            await schoolRepository.SaveChangesAsync();
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Empty(result.Enrollments!);
        }

        [Fact]
        public async Task Add_A_Single_Entity_With_Child_Objects_Using_AddGraphChange()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.AddGraphChange(schoolRepository, student);
            await schoolRepository.SaveChangesAsync();
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Equal(3, result.Enrollments!.Count);
        }

        [Fact]
        public async Task Add_Entity_List_With_Child_Objects_Using_AddGraphChanges()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.AddGraphChanges(schoolRepository, [student]);
            await schoolRepository.SaveChangesAsync();
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Equal(3, result.Enrollments!.Count);
        }

        [Fact]
        public async Task Delete_A_Single_Entity()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            string fullName = "Carson Alexander";
            var before = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == fullName)).SingleOrDefault();

            //act
            PersistenceOperationsUtils<StudentModel, Student>.Delete(schoolRepository, s => s.FullName == fullName);
            var after = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == fullName)).SingleOrDefault();

            //assert
            Assert.NotNull(before);
            Assert.Null(after);
        }

        [Fact]
        public async Task Add_A_Single_Entity_Without_Child_Objects()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.Save(schoolRepository, student);
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Empty(result.Enrollments!);
        }

        [Fact]
        public async Task Add_A_Entity_List_Without_Child_Objects()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.Save(schoolRepository, [student]);
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Empty(result.Enrollments!);
        }

        [Fact]
        public async Task Add_A_Single_Entity_With_Child_Objects()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.SaveGraph(schoolRepository, student);
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Equal(3, result.Enrollments!.Count);
        }

        [Fact]
        public async Task Add_Entity_List_With_Child_Objects()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider?.GetRequiredService<ISchoolRepository>()!;
            var student = new StudentModel
            {
                EntityState = Domain.EntityStateType.Added,
                EnrollmentDate = new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                Enrollments =
                [
                    new EnrollmentModel { CourseID = 1050, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4022, Grade = Models.Grade.A },
                    new EnrollmentModel { CourseID = 4041, Grade = Models.Grade.A }
                ],
                FirstName = "Roger",
                LastName = "Milla"
            };

            //act
            PersistenceOperationsUtils<StudentModel, Student>.SaveGraphs(schoolRepository, [student]);
            var result = (await schoolRepository.GetAsync<StudentModel, Student>(s => s.FullName == "Roger Milla", null, new SelectExpandDefinition([], [new SelectExpandItem("Enrollments")]))).Single();

            //assert
            Assert.Equal(new DateTime(2021, 2, 8, 0, 0, 0, DateTimeKind.Utc), result.EnrollmentDate);
            Assert.Equal(3, result.Enrollments!.Count);
        }

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
                .AddAppUtilsServices()
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
