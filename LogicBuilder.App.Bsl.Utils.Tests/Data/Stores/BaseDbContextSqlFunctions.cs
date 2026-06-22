using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data.Stores
{
    public static class BaseDbContextSqlFunctions
    {
#pragma warning disable IDE0060 //unused field is required for database function mapping
        public static string FormatDateTime(DateTime value, string format, string culture) => value.ToString(format);
        public static string FormatDecimal(decimal value, string format, string culture) => value.ToString(format);
#pragma warning restore IDE0060 //unused field is required for database function mapping

        public static void Register(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction
            (
                typeof(BaseDbContextSqlFunctions).GetMethod(nameof(FormatDateTime), [typeof(DateTime), typeof(string), typeof(string)])!
            )
            .HasTranslation(Translate);

            modelBuilder.HasDbFunction
            (
                typeof(BaseDbContextSqlFunctions).GetMethod(nameof(FormatDecimal), [typeof(decimal), typeof(string), typeof(string)])!
            )
            .HasTranslation(Translate);
        }

        private static SqlExpression Translate(IReadOnlyCollection<SqlExpression> args)
            => new SqlFunctionExpression
            (
                "FORMAT",
                args,
                false,
                [true, true, true],
                typeof(string),
                null
            );
    }
}
