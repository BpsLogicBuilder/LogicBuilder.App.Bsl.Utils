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
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.Strutures;
using LogicBuilder.Forms.Parameters.Expansions;
using LogicBuilder.Forms.Parameters.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.App.Bsl.Utils.Tests
{
    public class QueryOperationsTest : IClassFixture<DatabaseFixture>
    {
        static QueryOperationsTest()
        {
            InitializeMapperConfiguration();
        }

        public QueryOperationsTest(DatabaseFixture databaseFixture)
        {
            this.databaseFixture = databaseFixture;
            Initialize();
        }

        [Fact]
        public void SelectNewCourseAssignment()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
            (
                new OrderByOperatorParameters
                (
                    new ParameterOperatorParameters("q"),
                    new MemberSelectorOperatorParameters
                    (
                        "Title",
                        new ParameterOperatorParameters("s")
                    ),
                    ListSortDirection.Ascending,
                    "s"
                ),
                new MemberInitOperatorParameters
                (
                    [
                        new MemberBindingItem("CourseID", new MemberSelectorOperatorParameters("CourseID", new ParameterOperatorParameters("a"))),
                        new MemberBindingItem("CourseTitle", new MemberSelectorOperatorParameters("Title", new ParameterOperatorParameters("a"))),
                        new MemberBindingItem
                        (
                            "CourseNumberAndTitle", new ConcatOperatorParameters
                            (
                                new ConcatOperatorParameters
                                (
                                    new ConvertToStringOperatorParameters
                                    (
                                        new MemberSelectorOperatorParameters("CourseID", new ParameterOperatorParameters("a"))
                                    ),
                                    new ConstantOperatorParameters(" ", typeof(string))
                                ),
                                new MemberSelectorOperatorParameters("Title", new ParameterOperatorParameters("a"))
                            )
                        )
                    ],
                    typeof(CourseAssignmentModel)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, IQueryable<CourseAssignmentModel>>(bodyParameter, "q");
            IQueryable<CourseAssignmentModel> returnValue = QueryOperations<CourseModel, Course, IQueryable<CourseAssignmentModel>, IQueryable<CourseAssignment>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<CourseModel>, IQueryable<CourseAssignmentModel>>> selectorExpression = GetSelectorLambdaExpression<CourseModel, IQueryable<CourseAssignmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal("Calculus", returnValue.First().CourseTitle);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => q.OrderBy(s => s.Title).Select(a => new CourseAssignmentModel() {CourseID = a.CourseID, CourseTitle = a.Title, CourseNumberAndTitle = a.CourseID.ToString().Concat(\" \").Concat(a.Title)})"
            );
        }

        [Fact]
        public void SelectInstructorFullNames()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
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
                new MemberSelectorOperatorParameters("FullName", new ParameterOperatorParameters("a")),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<InstructorModel>, IQueryable<string>>(bodyParameter, "q");
            IQueryable<string> returnValue = QueryOperations<InstructorModel, Instructor, IQueryable<string>, IQueryable<string>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<InstructorModel>, IQueryable<string>>> selectorExpression = GetSelectorLambdaExpression<InstructorModel, IQueryable<string>>(selectorExpressionParameter);

            //assert
            Assert.Equal("Candace Kapoor", returnValue.First());
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => q.OrderBy(s => s.FullName).Select(a => a.FullName)"
            );
        }

        [Fact]
        public void SelectNewInstructor()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
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
                new MemberInitOperatorParameters
                (
                    [
                        new MemberBindingItem("ID", new MemberSelectorOperatorParameters("ID", new ParameterOperatorParameters("a"))),
                        new MemberBindingItem("FirstName", new MemberSelectorOperatorParameters("FirstName", new ParameterOperatorParameters("a"))),
                        new MemberBindingItem("LastName", new MemberSelectorOperatorParameters("LastName", new ParameterOperatorParameters("a"))),
                        new MemberBindingItem("FullName", new MemberSelectorOperatorParameters("FullName", new ParameterOperatorParameters("a")))
                    ],
                    typeof(InstructorModel)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<InstructorModel>, IQueryable<InstructorModel>>(bodyParameter, "q");
            IQueryable<InstructorModel> returnValue = QueryOperations<InstructorModel, Instructor, IQueryable<InstructorModel>, IQueryable<Instructor>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<InstructorModel>, IQueryable<InstructorModel>>> selectorExpression = GetSelectorLambdaExpression<InstructorModel, IQueryable<InstructorModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal("Candace Kapoor", returnValue.First().FullName);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => q.OrderBy(s => s.FullName).Select(a => new InstructorModel() {ID = a.ID, FirstName = a.FirstName, LastName = a.LastName, FullName = a.FullName})"
            );
        }

        [Fact]
        public void SelectNewInstructor_FullNameOnly()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
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
                new MemberInitOperatorParameters
                (
                    [
                        new MemberBindingItem("FullName", new MemberSelectorOperatorParameters("FullName", new ParameterOperatorParameters("a")))
                    ],
                    typeof(InstructorModel)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<InstructorModel>, IQueryable<InstructorModel>>(bodyParameter, "q");
            IQueryable<InstructorModel> returnValue = QueryOperations<InstructorModel, Instructor, IQueryable<InstructorModel>, IQueryable<Instructor>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<InstructorModel>, IQueryable<InstructorModel>>> selectorExpression = GetSelectorLambdaExpression<InstructorModel, IQueryable<InstructorModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(" ", returnValue.First().FullName);//No way to create FirstName and LastName in the translated expressions with the FullName binding alone. See the test "SelectNewInstructor() for FullName binding
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => q.OrderBy(s => s.FullName).Select(a => new InstructorModel() {FullName = a.FullName})"
            );
        }

        [Fact]
        public void SelectNewInstructor_FirstNameOnly()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
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
                new MemberInitOperatorParameters
                (
                    [
                        new MemberBindingItem("FirstName", new MemberSelectorOperatorParameters("FirstName", new ParameterOperatorParameters("a")))
                    ],
                    typeof(InstructorModel)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<InstructorModel>, IQueryable<InstructorModel>>(bodyParameter, "q");
            IQueryable<InstructorModel> returnValue = QueryOperations<InstructorModel, Instructor, IQueryable<InstructorModel>, IQueryable<Instructor>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<InstructorModel>, IQueryable<InstructorModel>>> selectorExpression = GetSelectorLambdaExpression<InstructorModel, IQueryable<InstructorModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal("Candace", returnValue.First().FirstName);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => q.OrderBy(s => s.FullName).Select(a => new InstructorModel() {FirstName = a.FirstName})"
            );
        }

        [Fact]
        public void SelectNewAnonymousType_FullNameOnly()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
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
                new MemberInitOperatorParameters
                (
                    [
                        new MemberBindingItem("FullName", new MemberSelectorOperatorParameters("FullName", new ParameterOperatorParameters("a")))
                    ]
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<InstructorModel>, IQueryable<dynamic>>(bodyParameter, "q");
            IQueryable<dynamic> returnValue = QueryOperations<InstructorModel, Instructor, IQueryable<dynamic>, IQueryable<dynamic>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<InstructorModel>, IQueryable<dynamic>>> selectorExpression = GetSelectorLambdaExpression<InstructorModel, IQueryable<dynamic>>(selectorExpressionParameter);

            //assert
            Assert.Equal("Candace Kapoor", returnValue.First().FullName);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => Convert(q.OrderBy(s => s.FullName).Select(a => new AnonymousType() {FullName = a.FullName}))"
            );
            //How come this fails for Instructor type "q => q.OrderBy(s => s.FullName).Select(a => new InstructorModel() {FullName = a.FullName})" ?
            //Probably because we can't map new InstructorModel.FullName back to Instructor.FirstName and Instructor.LastName  in the data query
            //AnonymousType.FullName is itself the data query
        }

        [Fact]
        public void Where_OrderBy_ThenBy_Skip_Take_Average()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new AverageOperatorParameters
            (
                new TakeOperatorParameters
                (
                    new SkipOperatorParameters
                    (
                        new ThenByOperatorParameters
                        (
                            new OrderByOperatorParameters
                            (
                                new WhereOperatorParameters
                                (//q.Where(s => ((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0)))
                                    new ParameterOperatorParameters("q"),//q. the source operand
                                    new AndBinaryOperatorParameters//((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0)
                                    (
                                        new GreaterThanBinaryOperatorParameters
                                        (
                                            new MemberSelectorOperatorParameters("Id", new ParameterOperatorParameters("s")),
                                            new ConstantOperatorParameters(1, typeof(int))
                                        ),
                                        new GreaterThanBinaryOperatorParameters
                                        (
                                            new MemberSelectorOperatorParameters("FirstName", new ParameterOperatorParameters("s")),
                                            new MemberSelectorOperatorParameters("LastName", new ParameterOperatorParameters("s"))
                                        )
                                    ),
                                    "s"//s => (created in Where operator.  The parameter type is based on the source operand underlying type in this case Student.)
                                ),
                                new MemberSelectorOperatorParameters("LastName", new ParameterOperatorParameters("v")),
                                ListSortDirection.Ascending,
                                "v"
                            ),
                            new MemberSelectorOperatorParameters("FirstName", new ParameterOperatorParameters("v")),
                            ListSortDirection.Descending,
                            "v"
                        ),
                        2
                    ),
                    3
                ),
                new MemberSelectorOperatorParameters("Id", new ParameterOperatorParameters("j")),
                "j"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<StudentModel>, double>(bodyParameter, "q");
            double returnValue = QueryOperations<StudentModel, Student, double, double>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<StudentModel>, double>> selectorExpression = GetSelectorLambdaExpression<StudentModel, double>(selectorExpressionParameter);

            //assert
            Assert.True(returnValue > 1);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => q.Where(s => ((s.ID > 1) AndAlso (s.FirstName.Compare(s.LastName) > 0))).OrderBy(v => v.LastName).ThenByDescending(v => v.FirstName).Skip(2).Take(3).Average(j => j.ID)"
            );
        }

        [Fact]
        public void GroupBy_OrderBy_ThenBy_Skip_Take_Average()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
            (
                new OrderByOperatorParameters
                (
                    new GroupByOperatorParameters
                    (
                        new ParameterOperatorParameters("q"),
                        new ConstantOperatorParameters(1, typeof(int)),
                        "a"
                    ),
                    new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("b")),
                    ListSortDirection.Ascending,
                    "b"
                ),
                new MemberInitOperatorParameters
                (
                    [
                        new MemberBindingItem
                        (
                            "Sum_budget",
                            new SumOperatorParameters
                            (
                                new WhereOperatorParameters
                                (
                                    new ParameterOperatorParameters("q"),
                                    new AndBinaryOperatorParameters
                                    (
                                        new NotEqualsBinaryOperatorParameters
                                        (
                                            new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("d")),
                                            new CountOperatorParameters(new ParameterOperatorParameters("q"))
                                        ),
                                        new EqualsBinaryOperatorParameters
                                        (
                                            new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("d")),
                                            new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("c"))
                                        )
                                    ),
                                    "d"
                                ),
                                new MemberSelectorOperatorParameters("Budget", new ParameterOperatorParameters("item")),
                                "item"
                            )
                        )
                    ]
                ),
                "c"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, IQueryable<dynamic>>(bodyParameter, "q");
            IQueryable<dynamic> returnValue = QueryOperations<DepartmentModel, Department, IQueryable<dynamic>, IQueryable<dynamic>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, IQueryable<dynamic>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, IQueryable<dynamic>>(selectorExpressionParameter);

            //assert
            Assert.True(returnValue.First().Sum_budget == 350000);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => Convert(q.GroupBy(a => 1).OrderBy(b => b.Key).Select(c => new AnonymousType() {Sum_budget = q.Where(d => ((d.DepartmentID != q.Count()) AndAlso (d.DepartmentID == c.Key))).Sum(item => item.Budget)}))"
            );
        }

        [Fact]
        public void GroupBy_AsQueryable_OrderBy_Select_FirstOrDefault()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new FirstOrDefaultOperatorParameters
            (
                new SelectOperatorParameters
                (
                    new OrderByOperatorParameters
                    (
                        new AsQueryableOperatorParameters
                        (
                            new GroupByOperatorParameters
                            (
                                new ParameterOperatorParameters("q"),
                                new ConstantOperatorParameters(1, typeof(int)),
                                "item"
                            )
                        ),
                        new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("group")),
                        ListSortDirection.Ascending,
                        "group"
                    ),
                    new MemberInitOperatorParameters
                    (
                        [
                            new MemberBindingItem
                            (
                                "Min_administratorName",
                                new MinOperatorParameters
                                (
                                    new WhereOperatorParameters
                                    (
                                        new ParameterOperatorParameters("q"),
                                        new EqualsBinaryOperatorParameters
                                        (
                                            new ConstantOperatorParameters(1, typeof(int)),
                                            new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("sel"))
                                        ),
                                        "d"
                                    ),
                                    new MemberSelectorOperatorParameters("AdministratorName", new ParameterOperatorParameters("item")),
                                    "item"
                                )
                            ),
                            new MemberBindingItem
                            (
                                "Count",
                                new CountOperatorParameters
                                (
                                    new WhereOperatorParameters
                                    (
                                        new ParameterOperatorParameters("q"),
                                        new EqualsBinaryOperatorParameters
                                        (
                                            new ConstantOperatorParameters(1, typeof(int)),
                                            new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("sel"))
                                        ),
                                        "d"
                                    )
                                )
                            ),
                            new MemberBindingItem
                            (
                                "Sum_budget",
                                new SumOperatorParameters
                                (
                                    new WhereOperatorParameters
                                    (
                                        new ParameterOperatorParameters("q"),
                                        new EqualsBinaryOperatorParameters
                                        (
                                            new ConstantOperatorParameters(1, typeof(int)),
                                            new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("sel"))
                                        ),
                                        "d"
                                    ),
                                    new MemberSelectorOperatorParameters("Budget", new ParameterOperatorParameters("item")),
                                    "item"
                                )
                            ),
                            new MemberBindingItem
                            (
                                "Min_budget",
                                new MinOperatorParameters
                                (
                                    new WhereOperatorParameters
                                    (
                                        new ParameterOperatorParameters("q"),
                                        new EqualsBinaryOperatorParameters
                                        (
                                            new ConstantOperatorParameters(1, typeof(int)),
                                            new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("sel"))
                                        ),
                                        "d"
                                    ),
                                    new MemberSelectorOperatorParameters("Budget", new ParameterOperatorParameters("item")),
                                    "item"
                                )
                            ),
                            new MemberBindingItem
                            (
                                "Min_startDate",
                                new MinOperatorParameters
                                (
                                    new WhereOperatorParameters
                                    (
                                        new ParameterOperatorParameters("q"),
                                        new EqualsBinaryOperatorParameters
                                        (
                                            new ConstantOperatorParameters(1, typeof(int)),
                                            new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("sel"))
                                        ),
                                        "d"
                                    ),
                                    new MemberSelectorOperatorParameters("StartDate", new ParameterOperatorParameters("item")),
                                    "item"
                                )
                            )
                        ]
                    ),
                    "sel"
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, dynamic>(bodyParameter, "q");
            dynamic returnValue = QueryOperations<DepartmentModel, Department, dynamic, object>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, dynamic>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, dynamic>(selectorExpressionParameter);

            //assert
            Assert.True(returnValue.Min_administratorName == "Candace Kapoor");
            Assert.True(returnValue.Count == 4);
            Assert.True(returnValue.Sum_budget == 900000);
            Assert.True(returnValue.Min_budget == 100000);
            Assert.True(returnValue.Min_startDate == DateTime.SpecifyKind(DateTime.Parse("2007-09-01", CultureInfo.InvariantCulture), DateTimeKind.Unspecified));
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "q => Convert(q.GroupBy(item => 1).AsQueryable().OrderBy(group => group.Key).Select(sel => new AnonymousType() {Min_administratorName = q.Where(d => (1 == sel.Key)).Min(item => item.AdministratorName), Count = q.Where(d => (1 == sel.Key)).Count(), Sum_budget = q.Where(d => (1 == sel.Key)).Sum(item => item.Budget), Min_budget = q.Where(d => (1 == sel.Key)).Min(item => item.Budget), Min_startDate = q.Where(d => (1 == sel.Key)).Min(item => item.StartDate)}).FirstOrDefault())"
            );
        }

        [Fact]
        public void All_Filter()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new AllOperatorParameters
            (
                new ParameterOperatorParameters("$it"),
                new OrBinaryOperatorParameters
                (
                    new EqualsBinaryOperatorParameters
                    (
                        new MemberSelectorOperatorParameters("AdministratorName", new ParameterOperatorParameters("a")),
                        new ConstantOperatorParameters("Kim Abercrombie")
                    ),
                    new EqualsBinaryOperatorParameters
                    (
                        new MemberSelectorOperatorParameters("AdministratorName", new ParameterOperatorParameters("a")),
                        new ConstantOperatorParameters("Fadi Fakhouri")
                    )
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, bool>(bodyParameter, "$it");
            dynamic returnValue = QueryOperations<DepartmentModel, Department, bool, bool>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, bool>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, bool>(selectorExpressionParameter);

            //assert
            Assert.False(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.All(a => ((a.AdministratorName == \"Kim Abercrombie\") OrElse (a.AdministratorName == \"Fadi Fakhouri\")))"
            );
        }

        [Fact]
        public void Any_Filter()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new AnyOperatorParameters
            (
                new ParameterOperatorParameters("$it"),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("AdministratorName", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters("Kim Abercrombie")
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, bool>(bodyParameter, "$it");
            dynamic returnValue = QueryOperations<DepartmentModel, Department, bool, bool>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, bool>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, bool>(selectorExpressionParameter);

            //assert
            Assert.True(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Any(a => (a.AdministratorName == \"Kim Abercrombie\"))"
            );
        }

        [Fact]
        public void Any()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new AnyOperatorParameters
            (
                new ParameterOperatorParameters("$it")
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, bool>(bodyParameter, "$it");
            dynamic returnValue = QueryOperations<DepartmentModel, Department, bool, bool>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, bool>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, bool>(selectorExpressionParameter);

            //assert
            Assert.True(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Any()"
            );
        }

        [Fact]
        public void AsQueryable()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new AsQueryableOperatorParameters
            (
                new ParameterOperatorParameters("$it")
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, IQueryable<DepartmentModel>>(bodyParameter, "$it");
            IQueryable<DepartmentModel> returnValue = QueryOperations<DepartmentModel, Department, IQueryable<DepartmentModel>, IQueryable<Department>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, IQueryable<DepartmentModel>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, IQueryable<DepartmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue.Count());
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.AsQueryable()"
            );
        }

        [Fact]
        public void Average_Selector()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new AverageOperatorParameters
            (
                new ParameterOperatorParameters("$it"),
                new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, double>(bodyParameter, "$it");
            double returnValue = QueryOperations<DepartmentModel, Department, double, double>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, double>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, double>(selectorExpressionParameter);

            //assert
            Assert.Equal(2.5, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Average(a => a.DepartmentID)"
            );
        }

        [Fact]
        public void Average()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new AverageOperatorParameters
            (
                new SelectOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    "a"
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, double>(bodyParameter, parameterName);
            double returnValue = QueryOperations<DepartmentModel, Department, double, double>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, double>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, double>(selectorExpressionParameter);

            //assert
            Assert.Equal(2.5, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Select(a => a.DepartmentID).Average()"
            );
        }

        [Fact]
        public void Count_Filter()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new CountOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(1, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Count(a => (a.DepartmentID == 1))"
            );
        }

        [Fact]
        public void Count()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new CountOperatorParameters
            (
                new ParameterOperatorParameters(parameterName)
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Count()"
            );
        }

        [Fact]
        public void Distinct()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new ToListOperatorParameters
            (
                new DistinctOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName)
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, List<DepartmentModel>>(bodyParameter, parameterName);
            List<DepartmentModel> returnValue = QueryOperations<DepartmentModel, Department, List<DepartmentModel>, List<Department>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, List<DepartmentModel>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, List<DepartmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue.Count);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Distinct().ToList()"
            );
        }

        [Fact()]
        public void First_Filter_Throws_Exception()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new FirstOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(-1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);
            //assert
            Assert.Throws<InvalidOperationException>
            (
                () => QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
                (
                    queryOperations,
                    selectorExpressionParameter
                )
            );
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.First(a => (a.DepartmentID == -1))"
            );
        }

        [Fact]
        public void First_Filter_Returns_match()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new FirstOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.Equal(1, returnValue.DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.First(a => (a.DepartmentID == 1))"
            );
        }

        [Fact]
        public void First()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new FirstOperatorParameters
            (
                new ParameterOperatorParameters(parameterName)
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.NotNull(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.First()"
            );
        }

        [Fact]
        public void FirstOrDefault_Filter_Returns_null()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new FirstOrDefaultOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(-1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.Null(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.FirstOrDefault(a => (a.DepartmentID == -1))"
            );
        }

        [Fact]
        public void FirstOrDefault_Filter_Returns_match()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new FirstOrDefaultOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.Equal(1, returnValue.DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.FirstOrDefault(a => (a.DepartmentID == 1))"
            );
        }

        [Fact]
        public void FirstOrDefault()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new FirstOrDefaultOperatorParameters
            (
                new ParameterOperatorParameters(parameterName)
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.NotNull(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.FirstOrDefault()"
            );
        }

        [Fact]
        public void GroupBy_Select()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
            (
                new GroupByOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    "a"
                ),
                new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("b")),
                "b"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, IQueryable<int>>(bodyParameter, parameterName);
            IQueryable<int> returnValue = QueryOperations<CourseModel, Course, IQueryable<int>, IQueryable<int>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<CourseModel>, IQueryable<int>>> selectorExpression = GetSelectorLambdaExpression<CourseModel, IQueryable<int>>(selectorExpressionParameter);

            //assert
            Assert.NotNull(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.GroupBy(a => a.DepartmentID).Select(b => b.Key)"
            );
        }

        [Fact]
        public void GroupBy_SelectCount()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new CountOperatorParameters
            (
                new SelectOperatorParameters
                (
                    new GroupByOperatorParameters
                    (
                        new ParameterOperatorParameters(parameterName),
                        new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                        "a"
                    ),
                    new MemberSelectorOperatorParameters("Key", new ParameterOperatorParameters("b")),
                    "b"
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<CourseModel, Course, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<CourseModel>, int>> selectorExpression = GetSelectorLambdaExpression<CourseModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.GroupBy(a => a.DepartmentID).Select(b => b.Key).Count()"
            );
        }

        [Fact]
        public void Last_Filter_Throws_Exception()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new LastOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(-1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            Assert.Throws<InvalidOperationException>
            (
                () => QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
                (
                    queryOperations,
                    selectorExpressionParameter
                )
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Last(a => (a.DepartmentID == -1))"
            );
        }

        [Fact]
        public void Last_Filter_Returns_match()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new LastOperatorParameters
            (
                new ToListOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName)
                ),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(2)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.Equal(2, returnValue.DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.ToList().Last(a => (a.DepartmentID == 2))"
            );
        }

        [Fact]
        public void Last()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new LastOperatorParameters
            (
                new ToListOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName)
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.NotNull(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.ToList().Last()"
            );
        }

        [Fact]
        public void LastOrDefault_Filter_Returns_null()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new LastOrDefaultOperatorParameters
            (
                new ToListOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName)
                ),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(-1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.Null(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.ToList().LastOrDefault(a => (a.DepartmentID == -1))"
            );
        }

        [Fact]
        public void LastOrDefault_Filter_Returns_match()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new LastOrDefaultOperatorParameters
            (
                new ToListOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName)
                ),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(2)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.Equal(2, returnValue.DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.ToList().LastOrDefault(a => (a.DepartmentID == 2))"
            );
        }

        [Fact]
        public void LastOrDefault()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new LastOrDefaultOperatorParameters
            (
                new ToListOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName)
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.NotNull(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.ToList().LastOrDefault()"
            );
        }

        [Fact]
        public void Max_Selector()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new MaxOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Max(a => a.DepartmentID)"
            );
        }

        [Fact]
        public void Max()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new MaxOperatorParameters
            (
                new SelectOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    "a"
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Select(a => a.DepartmentID).Max()"
            );
        }

        [Fact]
        public void Min_Selector()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new MinOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(1, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Min(a => a.DepartmentID)"
            );
        }

        [Fact]
        public void Min()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new MinOperatorParameters
            (
                new SelectOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    "a"
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(1, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Select(a => a.DepartmentID).Min()"
            );
        }

        [Fact]
        public void OrderBy()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new OrderByOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                ListSortDirection.Ascending,
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, IOrderedQueryable<DepartmentModel>>(bodyParameter, parameterName);
            IOrderedQueryable<DepartmentModel> returnValue = QueryOperations<DepartmentModel, Department, IOrderedQueryable<DepartmentModel>, IOrderedQueryable<Department>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, IOrderedQueryable<DepartmentModel>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, IOrderedQueryable<DepartmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(1, returnValue.First().DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.OrderBy(a => a.DepartmentID)"
            );
        }

        [Fact]
        public void OrderByDescending()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new OrderByOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                ListSortDirection.Descending,
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, IOrderedQueryable<DepartmentModel>>(bodyParameter, parameterName);
            IOrderedQueryable<DepartmentModel> returnValue = QueryOperations<DepartmentModel, Department, IOrderedQueryable<DepartmentModel>, IOrderedQueryable<Department>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, IOrderedQueryable<DepartmentModel>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, IOrderedQueryable<DepartmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue.First().DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.OrderByDescending(a => a.DepartmentID)"
            );
        }

        [Fact]
        public void OrderByThenBy()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new ThenByOperatorParameters
            (
                new OrderByOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("Credits", new ParameterOperatorParameters("a")),
                    ListSortDirection.Ascending,
                    "a"
                ),
                new MemberSelectorOperatorParameters("CourseID", new ParameterOperatorParameters("a")),
                ListSortDirection.Ascending,
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, IOrderedQueryable<CourseModel>>(bodyParameter, parameterName);
            IOrderedQueryable<CourseModel> returnValue = QueryOperations<CourseModel, Course, IOrderedQueryable<CourseModel>, IOrderedQueryable<Course>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<CourseModel>, IOrderedQueryable<CourseModel>>> selectorExpression = GetSelectorLambdaExpression<CourseModel, IOrderedQueryable<CourseModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(1050, returnValue.First().CourseID);
            Assert.Null(returnValue.First().Assignments!);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.OrderBy(a => a.Credits).ThenBy(a => a.CourseID)"
            );
        }

        [Fact]
        public void OrderByThenByWithExpansion()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new ThenByOperatorParameters
            (
                new OrderByOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("Credits", new ParameterOperatorParameters("a")),
                    ListSortDirection.Ascending,
                    "a"
                ),
                new MemberSelectorOperatorParameters("CourseID", new ParameterOperatorParameters("a")),
                ListSortDirection.Ascending,
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, IOrderedQueryable<CourseModel>>(bodyParameter, parameterName);
            IOrderedQueryable<CourseModel> returnValue = QueryOperations<CourseModel, Course, IOrderedQueryable<CourseModel>, IOrderedQueryable<Course>>.Query
            (
                queryOperations,
                selectorExpressionParameter,
                new SelectExpandDefinitionParameters
                (
                    [],
                    [
                        new SelectExpandItemParameters("Assignments")
                    ]
                )
            );
            Expression<Func<IQueryable<CourseModel>, IOrderedQueryable<CourseModel>>> selectorExpression = GetSelectorLambdaExpression<CourseModel, IOrderedQueryable<CourseModel>>(selectorExpressionParameter);

            //assert
            Assert.NotEmpty(returnValue.First().Assignments!);
            Assert.Equal(1050, returnValue.First().CourseID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.OrderBy(a => a.Credits).ThenBy(a => a.CourseID)"
            );
        }

        [Fact]
        public void OrderByThenByDescending()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new ThenByOperatorParameters
            (
                new OrderByOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("Credits", new ParameterOperatorParameters("a")),
                    ListSortDirection.Ascending,
                    "a"
                ),
                new MemberSelectorOperatorParameters("CourseID", new ParameterOperatorParameters("a")),
                ListSortDirection.Descending,
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, IOrderedQueryable<CourseModel>>(bodyParameter, parameterName);
            IOrderedQueryable<CourseModel> returnValue = QueryOperations<CourseModel, Course, IOrderedQueryable<CourseModel>, IOrderedQueryable<Course>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<CourseModel>, IOrderedQueryable<CourseModel>>> selectorExpression = GetSelectorLambdaExpression<CourseModel, IOrderedQueryable<CourseModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(4041, returnValue.First().CourseID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.OrderBy(a => a.Credits).ThenByDescending(a => a.CourseID)"
            );
        }

        [Fact]
        public void Paging()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new TakeOperatorParameters
            (
                new SkipOperatorParameters
                (
                    new ThenByOperatorParameters
                    (
                        new OrderByOperatorParameters
                        (
                            new SelectManyOperatorParameters
                            (
                                new ParameterOperatorParameters(parameterName),
                                new MemberSelectorOperatorParameters("Assignments", new ParameterOperatorParameters("a")),
                                "a"
                            ),
                            new MemberSelectorOperatorParameters("CourseTitle", new ParameterOperatorParameters("a")),
                            ListSortDirection.Ascending,
                            "a"
                        ),
                        new MemberSelectorOperatorParameters("InstructorID", new ParameterOperatorParameters("a")),
                        ListSortDirection.Ascending,
                        "a"
                    ),
                    1
                ),
                2
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, IQueryable<CourseAssignmentModel>>(bodyParameter, parameterName);
            IQueryable<CourseAssignmentModel> returnValue = QueryOperations<CourseModel, Course, IQueryable<CourseAssignmentModel>, IQueryable<CourseAssignment>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<CourseModel>, IQueryable<CourseAssignmentModel>>> selectorExpression = GetSelectorLambdaExpression<CourseModel, IQueryable<CourseAssignmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(2, returnValue.Count());
            Assert.Equal("Chemistry", returnValue.Last().CourseTitle);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.SelectMany(a => a.Assignments).OrderBy(a => a.CourseTitle).ThenBy(a => a.InstructorID).Skip(1).Take(2)"
            );
        }

        [Fact]
        public void Select_New()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectOperatorParameters
            (
                new OrderByOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    ListSortDirection.Descending,
                    "a"
                ),
                new MemberInitOperatorParameters
                (
                    [
                        new MemberBindingItem("ID", new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a"))),
                        new MemberBindingItem("DepartmentName", new MemberSelectorOperatorParameters("Name", new ParameterOperatorParameters("a"))),
                        new MemberBindingItem("Courses", new MemberSelectorOperatorParameters("Courses", new ParameterOperatorParameters("a")))
                    ]
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, IQueryable<dynamic>>(bodyParameter, parameterName);
            IQueryable<dynamic> returnValue = QueryOperations<DepartmentModel, Department, IQueryable<dynamic>, IQueryable<dynamic>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, IQueryable<dynamic>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, IQueryable<dynamic>>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue.First().ID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => Convert($it.OrderByDescending(a => a.DepartmentID).Select(a => new AnonymousType() {ID = a.DepartmentID, DepartmentName = a.Name, Courses = a.Courses}))"
            );
        }

        [Fact]
        public void SelectMany()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SelectManyOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new MemberSelectorOperatorParameters("Assignments", new ParameterOperatorParameters("a")),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<CourseModel>, IQueryable<CourseAssignmentModel>>(bodyParameter, parameterName);
            IQueryable<CourseAssignmentModel> returnValue = QueryOperations<CourseModel, Course, IQueryable<CourseAssignmentModel>, IQueryable<CourseAssignment>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<CourseModel>, IQueryable<CourseAssignmentModel>>> selectorExpression = GetSelectorLambdaExpression<CourseModel, IQueryable<CourseAssignmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(8, returnValue.Count());
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.SelectMany(a => a.Assignments)"
            );
        }

        [Fact]
        public void Single_Filter_Throws_Exception()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SingleOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(-1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            Assert.Throws<InvalidOperationException>
            (
                () => QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
                (
                    queryOperations,
                    selectorExpressionParameter
                )
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Single(a => (a.DepartmentID == -1))"
            );
        }

        [Fact]
        public void Single_Filter_Returns_match()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SingleOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            DepartmentModel returnValue = QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            Assert.Equal(1, returnValue.DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Single(a => (a.DepartmentID == 1))"
            );
        }

        [Fact]
        public void Single_with_multiple_matches_Throws_Exception()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SingleOperatorParameters
            (
                new ParameterOperatorParameters(parameterName)
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, DepartmentModel>(bodyParameter, parameterName);
            Assert.Throws<InvalidOperationException>
            (
                () => QueryOperations<DepartmentModel, Department, DepartmentModel, Department>.Query
                (
                    queryOperations,
                    selectorExpressionParameter
                )
            );
            Expression<Func<IQueryable<DepartmentModel>, DepartmentModel>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, DepartmentModel>(selectorExpressionParameter);

            //assert
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Single()"
            );
        }

        [Fact]
        public void Sum_Selector()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SumOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(10, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Sum(a => a.DepartmentID)"
            );
        }

        [Fact]
        public void Sum()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new SumOperatorParameters
            (
                new SelectOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    "a"
                )
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, int>(bodyParameter, parameterName);
            int returnValue = QueryOperations<DepartmentModel, Department, int, int>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, int>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, int>(selectorExpressionParameter);

            //assert
            Assert.Equal(10, returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Select(a => a.DepartmentID).Sum()"
            );
        }

        [Fact]
        public void ToList()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new ToListOperatorParameters
            (
                new ParameterOperatorParameters(parameterName)
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, List<DepartmentModel>>(bodyParameter, parameterName);
            List<DepartmentModel> returnValue = QueryOperations<DepartmentModel, Department, List<DepartmentModel>, List<Department>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, List<DepartmentModel>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, List<DepartmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(4, returnValue.Count);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.ToList()"
            );
        }

        [Fact]
        public void Where_with_matches()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new WhereOperatorParameters
            (
                new OrderByOperatorParameters
                (
                    new ParameterOperatorParameters(parameterName),
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    ListSortDirection.Descending,
                    "a"
                ),
                new NotEqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, IQueryable<DepartmentModel>>(bodyParameter, parameterName);
            IQueryable<DepartmentModel> returnValue = QueryOperations<DepartmentModel, Department, IQueryable<DepartmentModel>, IQueryable<Department>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, IQueryable<DepartmentModel>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, IQueryable<DepartmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Equal(2, returnValue.Last().DepartmentID);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.OrderByDescending(a => a.DepartmentID).Where(a => (a.DepartmentID != 1))"
            );
        }

        [Fact]
        public void Where_without_matches()
        {
            //arrange
            IQueryOperations queryOperations = serviceProvider!.GetRequiredService<IQueryOperations>();
            var bodyParameter = new WhereOperatorParameters
            (
                new ParameterOperatorParameters(parameterName),
                new EqualsBinaryOperatorParameters
                (
                    new MemberSelectorOperatorParameters("DepartmentID", new ParameterOperatorParameters("a")),
                    new ConstantOperatorParameters(-1)
                ),
                "a"
            );

            //act
            IExpressionParameter selectorExpressionParameter = GetSelectorParameter<IQueryable<DepartmentModel>, IQueryable<DepartmentModel>>(bodyParameter, parameterName);
            IQueryable<DepartmentModel> returnValue = QueryOperations<DepartmentModel, Department, IQueryable<DepartmentModel>, IQueryable<Department>>.Query
            (
                queryOperations,
                selectorExpressionParameter
            );
            Expression<Func<IQueryable<DepartmentModel>, IQueryable<DepartmentModel>>> selectorExpression = GetSelectorLambdaExpression<DepartmentModel, IQueryable<DepartmentModel>>(selectorExpressionParameter);

            //assert
            Assert.Empty(returnValue);
            AssertFilterStringIsCorrect
            (
                selectorExpression,
                "$it => $it.Where(a => (a.DepartmentID == -1))"
            );
        }

        #region Fields
        private const string parameterName = "$it";
        private readonly DatabaseFixture databaseFixture;
        private static MapperConfiguration MapperConfiguration;
        private static IServiceProvider? serviceProvider;
        #endregion Fields

        #region Helpers
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

        private static Expression<Func<IQueryable<TModel>, TModelReturn>> GetSelectorLambdaExpression<TModel, TModelReturn>(IExpressionParameter selectorExpressionParameter)
        {
            IMappingOperations mappingOperations = serviceProvider!.GetRequiredService<IMappingOperations>();
            IExpressionPart selectorExpression = mappingOperations.MapToOperator(selectorExpressionParameter);
            return (Expression<Func<IQueryable<TModel>, TModelReturn>>)selectorExpression.Build();
        }

        private static SelectorLambdaOperatorParameters GetSelectorParameter<T, TResult>(IExpressionParameter selectorBody, string parameterName = "$it")
            => new
            (
                selectorBody,
                typeof(T),
                parameterName,
                typeof(TResult)
            );

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
