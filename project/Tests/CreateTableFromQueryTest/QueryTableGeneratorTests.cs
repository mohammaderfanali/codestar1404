// using System;
// using System.Data;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using project.DataBase.CreateTableQuery;
// using project.Helpers.ColumnNameMaker;
// using project.Helpers.ColumnNameMaker.Abstraction;
// using Xunit;
//
// namespace Tests.CreateTableFromQueryTest
// {
//     public class QueryTableGeneratorTests
//     {
//         [Fact]
//         public void GenerateCreateTableSql_ShouldGenerateCorrectSql_WithResolvedColumnNames()
//         {
//             // Arrange
//             var logger = Substitute.For<ILogger<QueryTableGenerator>>();
//             var resolver = Substitute.For<IColumnNameResolver>();
//             var generator = new QueryTableGenerator(logger, resolver);
//
//             var tableName = "TestTable";
//             var table = new DataTable();
//             table.Columns.Add("Name", typeof(string));
//             table.Columns.Add("Name1", typeof(string)); // ❌ DataTable won't allow this, so we simulate
//
//             // Simulate resolved names
//             resolver.ResolveUniqueNames(table.Columns).Returns(new List<string> { "Name", "Name1" });
//
//             // Act
//             var sql = generator.GenerateCreateTableSql(tableName, table.Columns);
//
//             // Assert
//             var expectedSql =
//                 @"DROP TABLE IF EXISTS ""TestTable"";
// CREATE TABLE ""TestTable"" (
//     ""Name"" TEXT,
//     ""Name1"" TEXT
// );";
//
//             Assert.Equal(expectedSql, sql.Trim());
//
//             // Verify logging
//             logger.Received().LogInformation("Generating CREATE TABLE SQL for table: {TableName}", tableName);
//             logger.Received().LogInformation(Arg.Is<string>(s => s.Contains("Generated SQL:")));
//             logger.Received().LogDebug("Column: {OriginalName} mapped to {FinalName} with type {Type}",
//                 "Name", "Name", "TEXT");
//             logger.Received().LogDebug("Column: {OriginalName} mapped to {FinalName} with type {Type}",
//                 "Name", "Name1", "TEXT");
//         }
//     }
// }