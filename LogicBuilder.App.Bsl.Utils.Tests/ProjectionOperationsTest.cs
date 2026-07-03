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
using LogicBuilder.Expressions.Utils.Strutures;
using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Tests
{
    public class ProjectionOperationsTest : IClassFixture<DatabaseFixture>
    {
        static ProjectionOperationsTest()
        {
            InitializeMapperConfiguration();
        }

        public ProjectionOperationsTest(DatabaseFixture databaseFixture)
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
        public void Get_students_with_filter_with_expansion()
        {
            ICollection<StudentModel> students = ProjectionOperations<StudentModel, Student>.GetItems
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>(),
                new FilterLambdaOperatorParameters
                (
                    new GreaterThanBinaryOperatorParameters
                    (
                        new CountOperatorParameters
                        (
                            new MemberSelectorOperatorParameters("Enrollments", new ParameterOperatorParameters("f"))
                        ),
                        new ConstantOperatorParameters(0)
                    ),
                    typeof(StudentModel),
                    "f"
                ),
                null,
                new SelectExpandDefinitionParameters
                (
                    [],
                    [
                        new SelectExpandItemParameters("Enrollments")
                    ]
                )
            );

            Assert.True(students.First().Enrollments!.Count > 0);
        }

        [Fact]
        public void Get_students_with_filter_and_no_expansion()
        {
            ICollection<StudentModel> students = ProjectionOperations<StudentModel, Student>.GetItems
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>(),
                new FilterLambdaOperatorParameters
                (
                    new GreaterThanBinaryOperatorParameters
                    (
                        new CountOperatorParameters
                        (
                            new MemberSelectorOperatorParameters("Enrollments", new ParameterOperatorParameters("f"))
                        ),
                        new ConstantOperatorParameters(0)
                    ),
                    typeof(StudentModel),
                    "f"
                )
            );

            Assert.Null(students.First().Enrollments);
        }

        [Fact]
        public void Get_students_no_filter_and_no_expansion()
        {
            ICollection<StudentModel> students = ProjectionOperations<StudentModel, Student>.GetItems
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>()
            );

            Assert.Null(students.First().Enrollments);
            Assert.Equal(11, students.Count);
        }

        [Fact]
        public void Get_students_sorted_with_filter_and_no_expansion()
        {
            ICollection<StudentModel> students = ProjectionOperations<StudentModel, Student>.GetItems
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>(),
                new FilterLambdaOperatorParameters
                (
                    new GreaterThanBinaryOperatorParameters
                    (
                        new CountOperatorParameters
                        (
                            new MemberSelectorOperatorParameters("Enrollments", new ParameterOperatorParameters("f"))
                        ),
                        new ConstantOperatorParameters(0)
                    ),
                    typeof(StudentModel),
                    "f"
                ),
                new SelectorLambdaOperatorParameters
                (
                    new OrderByOperatorParameters
                    (
                        new ParameterOperatorParameters("q"),
                        new MemberSelectorOperatorParameters
                        (
                            "FullName",
                            new ParameterOperatorParameters("s")
                        ),
                        ListSortDirection.Ascending,
                        "s"
                    ),
                    typeof(IQueryable<StudentModel>),
                    "q",
                    typeof(IQueryable<StudentModel>)
                )
            );

            Assert.Equal("Arturo Anand", students.First().FullName);
        }

        [Fact]
        public void Get_students_with_filter_and_filtered_expansion()
        {
            ICollection<StudentModel> students = ProjectionOperations<StudentModel, Student>.GetItems
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>(),
                new FilterLambdaOperatorParameters
                (
                    new GreaterThanBinaryOperatorParameters
                    (
                        new CountOperatorParameters
                        (
                            new MemberSelectorOperatorParameters("Enrollments", new ParameterOperatorParameters("f"))
                        ),
                        new ConstantOperatorParameters(0)
                    ),
                    typeof(StudentModel),
                    "f"
                ),
                null,
                new SelectExpandDefinitionParameters
                (
                    [],
                    [
                        new SelectExpandItemParameters
                        (
                            "enrollments",
                            new SelectExpandItemFilterParameters
                            (
                                new FilterLambdaOperatorParameters
                                (
                                    new EqualsBinaryOperatorParameters
                                    (
                                        new MemberSelectorOperatorParameters("enrollmentID", new ParameterOperatorParameters("a")),
                                        new ConstantOperatorParameters(-1)
                                    ),
                                    typeof(EnrollmentModel),
                                    "a"
                                )
                            )
                        )
                    ]
                )
            );

            Assert.Empty(students.First().Enrollments!);
        }

        [Fact]
        public void Get_students_with_filter_and_sorted_expansion()
        {
            ICollection<StudentModel> students = ProjectionOperations<StudentModel, Student>.GetItems
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>(),
                new FilterLambdaOperatorParameters
                (
                    new GreaterThanBinaryOperatorParameters
                    (
                        new CountOperatorParameters
                        (
                            new MemberSelectorOperatorParameters("Enrollments", new ParameterOperatorParameters("f"))
                        ),
                        new ConstantOperatorParameters(0)
                    ),
                    typeof(StudentModel),
                    "f"
                ),
                null,
                new SelectExpandDefinitionParameters
                (
                    [],
                    [
                        new SelectExpandItemParameters
                        (
                            "enrollments",//,
                            new SelectExpandItemFilterParameters
                            (
                                new FilterLambdaOperatorParameters
                                (
                                    new GreaterThanBinaryOperatorParameters
                                    (
                                        new MemberSelectorOperatorParameters("enrollmentID", new ParameterOperatorParameters("a")),
                                        new ConstantOperatorParameters(0)
                                    ),
                                    typeof(EnrollmentModel),
                                    "a"
                                )
                            ),
                            new SelectExpandItemQueryFunctionParameters
                            (
                                new SortCollectionParameters
                                (
                                    [
                                        new SortDescriptionParameters("Grade", Expressions.Utils.Strutures.ListSortDirection.Ascending)
                                    ],
                                    null,
                                    null
                                )
                            ),
                            null,
                            null
                        )
                    ]
                )
            );

            Assert.True(students.First().Enrollments!.Count > 0);
            Assert.True
            (
                string.Compare
                (
                    students.First().Enrollments!.First().GradeLetter,
                    students.Skip(1).First().Enrollments!.First().GradeLetter
                ) <= 0
            );
        }

        [Fact]
        public void Get_single_student_filtered_expansion_with_sort_skip_and_take()
        {
            StudentModel? student = ProjectionOperations<StudentModel, Student>.Get
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>(),
                new FilterLambdaOperatorParameters
                (
                    new AndBinaryOperatorParameters
                    (
                        new EqualsBinaryOperatorParameters
                        (
                            new MemberSelectorOperatorParameters("FirstName", new ParameterOperatorParameters("f")),
                            new ConstantOperatorParameters("Carson")
                        ),
                        new EqualsBinaryOperatorParameters
                        (
                            new MemberSelectorOperatorParameters("LastName", new ParameterOperatorParameters("f")),
                            new ConstantOperatorParameters("Alexander")
                        )
                    ),
                    typeof(StudentModel),
                    "f"
                ),
                null,
                new SelectExpandDefinitionParameters
                (
                    [],
                    [
                        new SelectExpandItemParameters
                        (
                            "enrollments",
                            new SelectExpandItemFilterParameters
                            (
                                new FilterLambdaOperatorParameters
                                (
                                    new GreaterThanBinaryOperatorParameters
                                    (
                                        new MemberSelectorOperatorParameters("enrollmentID", new ParameterOperatorParameters("a")),
                                        new ConstantOperatorParameters(0)
                                    ),
                                    typeof(EnrollmentModel),
                                    "a"
                                )
                            ),
                            new SelectExpandItemQueryFunctionParameters
                            (
                                new SortCollectionParameters
                                (
                                    [
                                        new SortDescriptionParameters("Grade", Expressions.Utils.Strutures.ListSortDirection.Descending)
                                    ],
                                    1,
                                    2
                                )
                            ),
                            null,
                            null
                        )
                    ]
                )
            );

            Assert.NotNull(student);
            Assert.Equal(2, student.Enrollments!.Count);
            Assert.Equal("A", student.Enrollments!.Last().GradeLetter);
        }

        [Fact]
        public void Get_enrollments_filtered_by_grade_letter()
        {
            ICollection<EnrollmentModel> enrollments = ProjectionOperations<EnrollmentModel, Enrollment>.GetItems
            (
                serviceProvider!.GetRequiredService<IProjectionOperations>(),
                new FilterLambdaOperatorParameters
                (
                    new EqualsBinaryOperatorParameters
                    (
                        new MemberSelectorOperatorParameters("GradeLetter", new ParameterOperatorParameters("f")),
                        new ConstantOperatorParameters("A")
                    ),
                    typeof(EnrollmentModel),
                    "f"
                )
            );

            Assert.Single(enrollments);
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
                .AddTransient<IMappingOperations, MappingOperations>()
                .AddBslUtilservices()
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
