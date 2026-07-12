using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.App.Bsl.Utils.Tests.AutoMapperProfiles;
using LogicBuilder.App.Bsl.Utils.Tests.Data;
using LogicBuilder.App.Bsl.Utils.Tests.Data.Stores;
using LogicBuilder.App.Bsl.Utils.Tests.Models;
using LogicBuilder.App.Bsl.Utils.Tests.Models.Repositories;
using LogicBuilder.App.Utils.Interfaces;
using LogicBuilder.EntityFrameworkCore.Mapping;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Expressions.Utils.ExpansionDescriptors;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using LogicBuilder.Expressions.Utils.Strutures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Tests
{
    public class RequestHelperTest : IClassFixture<DatabaseFixture>
    {
        static RequestHelperTest()
        {
            InitializeMapperConfiguration();
        }

        public RequestHelperTest(DatabaseFixture databaseFixture)
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
        public async Task Select_Departments_In_Ascending_Order_As_LookUpsModel_Type()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<LookUpsModel>>
            (
                GetDepartmentsBodyForLookupModelType(),
                "q"
            );
            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var result = await requestHelper.GetList<DepartmentModel, Department, IEnumerable<LookUpsModel>, IEnumerable<LookUps>>
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor
                }
            );

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.Name).Select(d => new LookUpsModel() {NumericValue = Convert(d.DepartmentID), Text = d.Name}))");
            Assert.Equal(4, result.List.Count());
        }

        [Fact]
        public async Task Select_Courses_In_Ascending_Order_As_CourseModel_Type()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<CourseModel>, IEnumerable<CourseModel>>
            (
                GetCoursesBodyForCourseModelType(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var result = await requestHelper.GetList
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor,
                    ModelType = typeof(CourseModel).AssemblyQualifiedName,
                    DataType = typeof(Course).AssemblyQualifiedName,
                    ModelReturnType = typeof(IEnumerable<CourseModel>).AssemblyQualifiedName,
                    DataReturnType = typeof(IEnumerable<Course>).AssemblyQualifiedName
                }
            );

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.Title))");
            Assert.Equal(7, result.List.Count());
        }

        [Fact]
        public async Task Select_Students_In_Ascending_Order_As_StudentModel_Type()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<StudentModel>, IEnumerable<StudentModel>>
            (
                GetStudentsBodyForStudentModelType(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var result = await requestHelper.GetList
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor,
                    ModelType = typeof(StudentModel).AssemblyQualifiedName,
                    DataType = typeof(Student).AssemblyQualifiedName,
                    ModelReturnType = typeof(IEnumerable<StudentModel>).AssemblyQualifiedName,
                    DataReturnType = typeof(IEnumerable<Student>).AssemblyQualifiedName
                }
            );

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.FullName).Take(2))");
            Assert.Equal(2, result.List.Count());
        }

        [Fact]
        public async Task Get_Departments_ById_And_Courses_WithGenericHelper()
        {
            //arrange
            var filterLambdaOperatorDescriptor = GetFilterExpressionDescriptor<DepartmentModel>
            (
                GetDepartmentByNameFilterBody("Engineering"),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(filterLambdaOperatorDescriptor).Build();
            var selectAndExpand = new SelectExpandDefinitionDescriptor
            (
                [],
                [
                    new SelectExpandItemDescriptor("Courses")
                ]
            );

            var entity = (DepartmentModel)(await requestHelper.GetEntity<DepartmentModel, Department>
            (
                new Business.Requests.GetEntityRequest
                {
                    Filter = filterLambdaOperatorDescriptor,
                    SelectExpandDefinition = selectAndExpand
                }
            )).Entity!;

            //assert
            AssertFilterStringIsCorrect(expression, "q => (q.Name == \"Engineering\")");
            Assert.Single(entity.Courses!);
        }

        [Fact]
        public async Task Get_Departments_ById_And_Courses_WithoutGenericHelper()
        {
            //arrange
            var filterLambdaOperatorDescriptor = GetFilterExpressionDescriptor<DepartmentModel>
            (
                GetDepartmentByIdFilterBody(2),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(filterLambdaOperatorDescriptor).Build();
            var selectAndExpand = new SelectExpandDefinitionDescriptor
            (
                [],
                [
                    new SelectExpandItemDescriptor("Courses")
                ]
            );

            var entity = (DepartmentModel)(await requestHelper.GetEntity
            (
                new Business.Requests.GetEntityRequest
                {
                    Filter = filterLambdaOperatorDescriptor,
                    SelectExpandDefinition = selectAndExpand,
                    ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                    DataType = typeof(Department).AssemblyQualifiedName
                }
            )).Entity!;

            //assert
            AssertFilterStringIsCorrect(expression, "q => (q.DepartmentID == 2)");
            Assert.Equal(2, entity.Courses!.Count);
        }

        [Fact]
        public async Task Select_Departments_In_Ascending_Order_As_DepartmentModel_Type()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<DepartmentModel>>
            (
                GetDepartmentsBodyForDepartmentModelType(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var list = (await requestHelper.GetList<DepartmentModel, Department, IEnumerable<DepartmentModel>, IEnumerable<Department>>
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor
                }
            )).List;

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.Name).Select(d => new DepartmentModel() {DepartmentID = d.DepartmentID, Name = d.Name}))");
            Assert.Equal(4, list.Count());
        }

        [Fact]
        public async Task Select_Departments_In_Ascending_Order_As_DepartmentModel_Type_With_Courses()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IQueryable<DepartmentModel>>
            (
                GetDepartmentsBodyOrderByName(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var selectAndExpand = new SelectExpandDefinitionDescriptor
            (
                [],
                [
                    new SelectExpandItemDescriptor("Courses")
                ]
            );
            var list = (await requestHelper.GetList<DepartmentModel, Department, IQueryable<DepartmentModel>, IQueryable<Department>>
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor,
                    SelectExpandDefinition = selectAndExpand
                }
            )).List.Cast<DepartmentModel>().ToList();

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.Name))");
            Assert.Equal(4, list.Count);
            Assert.True(list.All(d => d.Courses!.Count != 0));
        }

        [Fact]
        public async Task Select_Instructors_In_Ascending_Order_As_InstructorModel_Type()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<InstructorModel>, IEnumerable<InstructorModel>>
            (
                GetInstructorsBody(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var list = (await requestHelper.GetList<InstructorModel, Instructor, IEnumerable<InstructorModel>, IEnumerable<Instructor>>
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor
                }
            )).List;

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.FullName).Select(d => new InstructorModel() {ID = d.ID, FirstName = d.FirstName, LastName = d.LastName, FullName = d.FullName}))");
            Assert.Equal(5, list.Count());
            Assert.Equal("Candace Kapoor", ((InstructorModel)list.First()).FullName);
        }

        [Fact]
        public async Task Select_Credits_From_Lookups_Table_In_Descending_Order_From_DropDownListRequest_As_LookUpsModel()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<LookUpsModel>, IEnumerable<LookUpsModel>>
            (
                GetBodyForLookupsModel(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var list = (await requestHelper.GetList
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor,
                    ModelType = typeof(LookUpsModel).AssemblyQualifiedName,
                    DataType = typeof(LookUps).AssemblyQualifiedName,
                    ModelReturnType = typeof(IEnumerable<LookUpsModel>).AssemblyQualifiedName,
                    DataReturnType = typeof(IEnumerable<LookUps>).AssemblyQualifiedName
                }
            )).List;

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.Where(l => (l.ListName == \"Credits\")).OrderByDescending(l => l.NumericValue).Select(l => new LookUpsModel() {NumericValue = l.NumericValue, Text = l.Text}))");
            Assert.Equal(5, list.Count());
        }

        [Fact]
        public async Task Select_Credits_From_Lookups_Table_In_Descending_Order_As_LookUpsModel_Using_Object_ReturnType()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<LookUpsModel>, IEnumerable<LookUpsModel>>
            (
                GetBodyForLookupsModel(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var response = await requestHelper.GetList<LookUpsModel, LookUps, IEnumerable<LookUpsModel>, IEnumerable<LookUps>>
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor
                }
            );

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.Where(l => (l.ListName == \"Credits\")).OrderByDescending(l => l.NumericValue).Select(l => new LookUpsModel() {NumericValue = l.NumericValue, Text = l.Text}))");
            Assert.Equal(5, response.List.Count());
        }

        [Fact]
        public async Task Select_Group_Students_By_EnrollmentDate_Return_EnrollmentDate_With_Count()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<StudentModel>, IQueryable<LookUpsModel>>
            (
                GetAboutBody(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var list = (await requestHelper.GetList
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor,
                    ModelType = typeof(StudentModel).AssemblyQualifiedName,
                    DataType = typeof(Student).AssemblyQualifiedName,
                    ModelReturnType = typeof(IQueryable<LookUpsModel>).AssemblyQualifiedName,
                    DataReturnType = typeof(IQueryable<LookUps>).AssemblyQualifiedName
                }
            )).List.ToList();

            //assert
            AssertFilterStringIsCorrect(expression, "q => q.GroupBy(item => item.EnrollmentDate).OrderByDescending(group => group.Key).Select(sel => new LookUpsModel() {DateTimeValue = sel.Key, NumericValue = Convert(sel.AsQueryable().Count())})");
            Assert.Equal(6, list.Count);
        }

        #region Helpers
        private static SelectDescriptor GetAboutBody()
            => new
            (
                new OrderByDescriptor
                (
                    new GroupByDescriptor
                    (
                        new ParameterDescriptor("q"),
                        new MemberSelectorDescriptor
                        (
                            "EnrollmentDate",
                            new ParameterDescriptor("item")
                        ),
                        "item"
                    ),
                    new MemberSelectorDescriptor
                    (
                        "Key",
                        new ParameterDescriptor("group")
                    ),
                    ListSortDirection.Descending,
                    "group"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["DateTimeValue"] = new MemberSelectorDescriptor
                        (
                            "Key",
                            new ParameterDescriptor("sel")
                        ),
                        ["NumericValue"] = new ConvertDescriptor
                        (
                            new CountDescriptor
                            (
                                new AsQueryableDescriptor
                                (
                                    new ParameterDescriptor("sel")
                                )
                            ),
                            typeof(double?).AssemblyQualifiedName!
                        )
                    },
                    typeof(LookUpsModel).AssemblyQualifiedName
                ),
                "sel"
            );

        private static SelectDescriptor GetInstructorsBody()
            => new
            (
                new OrderByDescriptor
                (
                    new ParameterDescriptor("q"),
                    new MemberSelectorDescriptor
                    (
                        "FullName",
                        new ParameterDescriptor("d")
                    ),
                    Expressions.Utils.Strutures.ListSortDirection.Ascending,
                    "d"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["ID"] = new MemberSelectorDescriptor
                        (
                            "ID",
                            new ParameterDescriptor("d")
                        ),
                        ["FirstName"] = new MemberSelectorDescriptor
                        (
                            "FirstName",
                            new ParameterDescriptor("d")
                        ),
                        ["LastName"] = new MemberSelectorDescriptor
                        (
                            "LastName",
                            new ParameterDescriptor("d")
                        ),
                        ["FullName"] = new MemberSelectorDescriptor
                        (
                            "FullName",
                            new ParameterDescriptor("d")
                        )
                    },
                    typeof(InstructorModel).AssemblyQualifiedName
                ),
                "d"
            );

        private static EqualsBinaryDescriptor GetDepartmentByIdFilterBody(int id)
            => new
            (
                new MemberSelectorDescriptor
                (
                    "DepartmentID",
                    new ParameterDescriptor("q")
                ),
                new ConstantDescriptor(id, typeof(int).AssemblyQualifiedName)
            );

        private static EqualsBinaryDescriptor GetDepartmentByNameFilterBody(string name)
            => new
            (
                new MemberSelectorDescriptor
                (
                    "Name",
                    new ParameterDescriptor("q")
                ),
                new ConstantDescriptor(name, typeof(string).AssemblyQualifiedName)
            );

        private static OrderByDescriptor GetCoursesBodyForCourseModelType()
            => new
            (
                new ParameterDescriptor("q"),
                new MemberSelectorDescriptor
                (
                    "Title",
                    new ParameterDescriptor("d")
                ),
                Expressions.Utils.Strutures.ListSortDirection.Ascending,
                "d"
            );

        private static TakeDescriptor GetStudentsBodyForStudentModelType()
            => new
            (
                new OrderByDescriptor
                (
                    new ParameterDescriptor("q"),
                    new MemberSelectorDescriptor
                    (
                        "FullName",
                        new ParameterDescriptor("d")
                    ),
                    Expressions.Utils.Strutures.ListSortDirection.Ascending,
                    "d"
                ),
                2
            );

        private static OrderByDescriptor GetDepartmentsBodyOrderByName()
            => new
            (
                new ParameterDescriptor("q"),
                new MemberSelectorDescriptor
                (
                    "Name",
                    new ParameterDescriptor("d")
                ),
                Expressions.Utils.Strutures.ListSortDirection.Ascending,
                "d"
            );

        private static SelectDescriptor GetDepartmentsBodyForDepartmentModelType()
            => new
            (
                new OrderByDescriptor
                (
                    new ParameterDescriptor("q"),
                    new MemberSelectorDescriptor
                    (
                        "Name",
                        new ParameterDescriptor("d")
                    ),
                    Expressions.Utils.Strutures.ListSortDirection.Ascending,
                    "d"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["DepartmentID"] = new MemberSelectorDescriptor
                        (
                            "DepartmentID",
                            new ParameterDescriptor("d")
                        ),
                        ["Name"] = new MemberSelectorDescriptor
                        (
                            "Name",
                            new ParameterDescriptor("d")
                        )
                    },
                    typeof(DepartmentModel).AssemblyQualifiedName
                ),
                "d"
            );

        private static SelectDescriptor GetDepartmentsBodyForLookupModelType()
            => new
            (
                new OrderByDescriptor
                (
                    new ParameterDescriptor("q"),
                    new MemberSelectorDescriptor
                    (
                        "Name",
                        new ParameterDescriptor("d")
                    ),
                    Expressions.Utils.Strutures.ListSortDirection.Ascending,
                    "d"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["NumericValue"] = new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "DepartmentID",
                                new ParameterDescriptor("d")
                            ),
                            typeof(double?).AssemblyQualifiedName!
                        ),
                        ["Text"] = new MemberSelectorDescriptor
                        (
                            "Name",
                            new ParameterDescriptor("d")
                        )
                    },
                    typeof(LookUpsModel).AssemblyQualifiedName
                ),
                "d"
            );

        private static SelectDescriptor GetBodyForLookupsModel()
            => new
            (
                new OrderByDescriptor
                (
                    new WhereDescriptor
                    (
                        new ParameterDescriptor("q"),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "ListName",
                                new ParameterDescriptor("l")
                            ),
                            new ConstantDescriptor
                            (
                                "Credits",
                                typeof(string).AssemblyQualifiedName
                            )
                        ),
                        "l"
                    ),
                    new MemberSelectorDescriptor
                    (
                        "NumericValue",
                        new ParameterDescriptor("l")
                    ),
                    Expressions.Utils.Strutures.ListSortDirection.Descending,
                    "l"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, DescriptorBase>
                    {
                        ["NumericValue"] = new MemberSelectorDescriptor
                        (
                            "NumericValue",
                            new ParameterDescriptor("l")
                        ),
                        ["Text"] = new MemberSelectorDescriptor
                        (
                            "Text",
                            new ParameterDescriptor("l")
                        )
                    },
                   typeof(LookUpsModel).AssemblyQualifiedName
                ),
                "l"
            );

        [Fact]
        public async Task GetAnonymousList_Generic_Returns_Dynamic_List()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<dynamic>>
            (
                GetDepartmentsBodyOrderByName(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var result = await requestHelper.GetAnonymousList<DepartmentModel, Department>
            (
                new Business.Requests.GetObjectListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor
                }
            );

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.Name))");
            Assert.True(result.Success);
            Assert.Equal(4, result.List.Count());
        }

        [Fact]
        public async Task GetAnonymousList_NonGeneric_Returns_Dynamic_List()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<dynamic>>
            (
                GetDepartmentsBodyOrderByName(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var result = await requestHelper.GetAnonymousList
            (
                new Business.Requests.GetObjectListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor,
                    ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                    DataType = typeof(Department).AssemblyQualifiedName
                }
            );

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.Name))");
            Assert.True(result.Success);
            Assert.Equal(4, result.List.Count());
        }

        [Fact]
        public async Task GetAnonymousList_With_Null_Selector_Throws_InvalidOperationException()
        {
            //arrange
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetAnonymousList<DepartmentModel, Department>
                (
                    new Business.Requests.GetObjectListRequest
                    {
                        Selector = null
                    }
                )
            );

            Assert.Equal("Selector is required.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetAnonymousList_NonGeneric_With_Invalid_ModelType_Throws_InvalidOperationException(string? modelType)
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<dynamic>>
            (
                GetDepartmentsBodyOrderByName(),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetAnonymousList
                (
                    new Business.Requests.GetObjectListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = modelType,
                        DataType = typeof(Department).AssemblyQualifiedName
                    }
                )
            );

            Assert.Contains("Model type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetAnonymousList_NonGeneric_With_Invalid_DataType_Throws_InvalidOperationException(string? dataType)
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<dynamic>>
            (
                GetDepartmentsBodyOrderByName(),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetAnonymousList
                (
                    new Business.Requests.GetObjectListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = dataType
                    }
                )
            );

            Assert.Contains("Data type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Fact]
        public async Task GetEntity_Generic_With_Null_SelectExpandDefinition_Returns_Entity()
        {
            //arrange
            var filterLambdaOperatorDescriptor = GetFilterExpressionDescriptor<DepartmentModel>
            (
                GetDepartmentByIdFilterBody(1),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(filterLambdaOperatorDescriptor).Build();
            var entity = (DepartmentModel)(await requestHelper.GetEntity<DepartmentModel, Department>
            (
                new Business.Requests.GetEntityRequest
                {
                    Filter = filterLambdaOperatorDescriptor,
                    SelectExpandDefinition = null
                }
            )).Entity!;

            //assert
            AssertFilterStringIsCorrect(expression, "q => (q.DepartmentID == 1)");
            Assert.NotNull(entity);
            Assert.Equal(1, entity.DepartmentID);
        }

        [Fact]
        public async Task GetEntity_With_Null_Filter_Throws_InvalidOperationException()
        {
            //arrange
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetEntity<DepartmentModel, Department>
                (
                    new Business.Requests.GetEntityRequest
                    {
                        Filter = null
                    }
                )
            );

            Assert.Equal("Filter is required.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetEntity_NonGeneric_With_Invalid_ModelType_Throws_InvalidOperationException(string? modelType)
        {
            //arrange
            var filterLambdaOperatorDescriptor = GetFilterExpressionDescriptor<DepartmentModel>
            (
                GetDepartmentByIdFilterBody(1),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetEntity
                (
                    new Business.Requests.GetEntityRequest
                    {
                        Filter = filterLambdaOperatorDescriptor,
                        ModelType = modelType,
                        DataType = typeof(Department).AssemblyQualifiedName
                    }
                )
            );

            Assert.Contains("Model type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetEntity_NonGeneric_With_Invalid_DataType_Throws_InvalidOperationException(string? dataType)
        {
            //arrange
            var filterLambdaOperatorDescriptor = GetFilterExpressionDescriptor<DepartmentModel>
            (
                GetDepartmentByIdFilterBody(1),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetEntity
                (
                    new Business.Requests.GetEntityRequest
                    {
                        Filter = filterLambdaOperatorDescriptor,
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = dataType
                    }
                )
            );

            Assert.Contains("Data type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Fact]
        public async Task GetList_Generic_With_Null_SelectExpandDefinition_Returns_List()
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<DepartmentModel>>
            (
                GetDepartmentsBodyForDepartmentModelType(),
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(selectorLambdaOperatorDescriptor).Build();
            var list = (await requestHelper.GetList<DepartmentModel, Department, IEnumerable<DepartmentModel>, IEnumerable<Department>>
            (
                new Business.Requests.GetTypedListRequest
                {
                    Selector = selectorLambdaOperatorDescriptor,
                    SelectExpandDefinition = null
                }
            )).List;

            //assert
            AssertFilterStringIsCorrect(expression, "q => Convert(q.OrderBy(d => d.Name).Select(d => new DepartmentModel() {DepartmentID = d.DepartmentID, Name = d.Name}))");
            Assert.Equal(4, list.Count());
        }

        [Fact]
        public async Task GetList_With_Null_Selector_Throws_InvalidOperationException()
        {
            //arrange
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetList<DepartmentModel, Department, IEnumerable<DepartmentModel>, IEnumerable<Department>>
                (
                    new Business.Requests.GetTypedListRequest
                    {
                        Selector = null
                    }
                )
            );

            Assert.Equal("Selector is required.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetList_NonGeneric_With_Invalid_ModelType_Throws_InvalidOperationException(string? modelType)
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<DepartmentModel>>
            (
                GetDepartmentsBodyForDepartmentModelType(),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetList
                (
                    new Business.Requests.GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = modelType,
                        DataType = typeof(Department).AssemblyQualifiedName,
                        ModelReturnType = typeof(IEnumerable<DepartmentModel>).AssemblyQualifiedName,
                        DataReturnType = typeof(IEnumerable<Department>).AssemblyQualifiedName
                    }
                )
            );

            Assert.Contains("Model type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetList_NonGeneric_With_Invalid_DataType_Throws_InvalidOperationException(string? dataType)
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<DepartmentModel>>
            (
                GetDepartmentsBodyForDepartmentModelType(),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetList
                (
                    new Business.Requests.GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = dataType,
                        ModelReturnType = typeof(IEnumerable<DepartmentModel>).AssemblyQualifiedName,
                        DataReturnType = typeof(IEnumerable<Department>).AssemblyQualifiedName
                    }
                )
            );

            Assert.Contains("Data type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetList_NonGeneric_With_Invalid_ModelReturnType_Throws_InvalidOperationException(string? modelReturnType)
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<DepartmentModel>>
            (
                GetDepartmentsBodyForDepartmentModelType(),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetList
                (
                    new Business.Requests.GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = typeof(Department).AssemblyQualifiedName,
                        ModelReturnType = modelReturnType,
                        DataReturnType = typeof(IEnumerable<Department>).AssemblyQualifiedName
                    }
                )
            );

            Assert.Contains("Model return type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task GetList_NonGeneric_With_Invalid_DataReturnType_Throws_InvalidOperationException(string? dataReturnType)
        {
            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<DepartmentModel>, IEnumerable<DepartmentModel>>
            (
                GetDepartmentsBodyForDepartmentModelType(),
                "q"
            );

            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await requestHelper.GetList
                (
                    new Business.Requests.GetTypedListRequest
                    {
                        Selector = selectorLambdaOperatorDescriptor,
                        ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                        DataType = typeof(Department).AssemblyQualifiedName,
                        ModelReturnType = typeof(IEnumerable<DepartmentModel>).AssemblyQualifiedName,
                        DataReturnType = dataReturnType
                    }
                )
            );

            Assert.Contains("Data return type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Fact]
        public async Task GetEntity_Returns_Null_When_Entity_Not_Found()
        {
            //arrange
            var filterLambdaOperatorDescriptor = GetFilterExpressionDescriptor<DepartmentModel>
            (
                GetDepartmentByIdFilterBody(999), // Non-existent ID
                "q"
            );

            IMappingOperations mappingOperationOptions = serviceProvider!.GetRequiredService<IMappingOperations>();
            IRequestHelper requestHelper = serviceProvider!.GetRequiredService<IRequestHelper>();

            //act
            var expression = mappingOperationOptions.MapToOperator(filterLambdaOperatorDescriptor).Build();
            var response = await requestHelper.GetEntity<DepartmentModel, Department>
            (
                new Business.Requests.GetEntityRequest
                {
                    Filter = filterLambdaOperatorDescriptor
                }
            );

            //assert
            AssertFilterStringIsCorrect(expression, "q => (q.DepartmentID == 999)");
            Assert.True(response.Success);
            Assert.Null(response.Entity);
        }

        private static SelectorLambdaDescriptor GetExpressionDescriptor<T, TResult>(DescriptorBase selectorBody, string parameterName = "$it")
            => new(selectorBody, typeof(T).AssemblyQualifiedName!, parameterName, typeof(TResult).AssemblyQualifiedName!);

        private static FilterLambdaDescriptor GetFilterExpressionDescriptor<T>(DescriptorBase filterBody, string parameterName = "$it")
            => new(filterBody, typeof(T).AssemblyQualifiedName!, parameterName);

        private static void AssertFilterStringIsCorrect(Expression expression, string expected)
        {
            AssertStringIsCorrect(ExpressionStringBuilder.ToString(expression));

            void AssertStringIsCorrect(string resultExpression)
                => Assert.True
                (
                    expected == resultExpression,
                    $"Expected expression '{expected}' but the deserializer produced '{resultExpression}'"
                );
        }

        [MemberNotNull(nameof(MapperConfiguration))]
        private static void InitializeMapperConfiguration()
        {
            MapperConfiguration ??= ConfigurationHelper.GetMapperConfiguration(cfg =>
            {
                cfg.AddExpressionMapping();

                cfg.AddProfile<ExpressionOperatorsMappingProfile>();
                cfg.AddProfile<SchoolProfile>();
                cfg.AddProfile<ExpansionDescriptorToOperatorMappingProfile>();
            });
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
                .AddTransient<IRequestHelper, RequestHelper>()
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
