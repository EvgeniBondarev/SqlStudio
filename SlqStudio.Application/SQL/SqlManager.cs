using Application.Common.SQL.ResponseModels;
using Application.Common.SQL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Application.Common.SQL
{
    public class SqlManager
    {
        private readonly string _connectionString;
        private readonly DataComparer _dataComparer;

        public SqlManager(string connectionString)
        {
            _connectionString = connectionString;
            _dataComparer = new DataComparer();
        }

        public async Task<SqlValidateResponse> ValidateSqlSyntaxAsync(string sql)
        {
            try
            {
                await ExecuteQueryAsync(sql);
                return new SqlValidateResponse { IsValid = true };
            }
            catch (SqlException ex)
            {
                return new SqlValidateResponse { Error = ex.Message, IsValid = false };
            }
        }

        public async Task<CompareResponse> CompareSqlResultsAsync(string sql1, string sql2)
        {

            var task1 = ExecuteQueryAsync(sql1);
            var task2 = ExecuteQueryAsync(sql2);

            await Task.WhenAll(task1, task2);

            bool isCompare = _dataComparer.CompareResults(task1.Result, task2.Result);

            return new CompareResponse
            {
                DataIsEqual = isCompare,
                QueryData1 = task1.Result,
                QueryData2 = task2.Result
            };
        }
        

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql)
        {
            var sqlCommands = sql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(command => command.Trim())
                                 .Where(command => !string.IsNullOrEmpty(command))
                                 .ToList();

            var results = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();

                try
                {
                    foreach (var command in sqlCommands)
                    {
                        var result = await connection.QueryAsync(command, transaction: transaction);
                        results.AddRange(result.Select(row => (IDictionary<string, object>)row)
                                               .Select(dict => dict.ToDictionary(kv => kv.Key, kv => kv.Value)));
                    }
                }
                catch (SqlException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка выполнения SQL команды: " + ex.Message, ex);
                }
                finally
                {
                    await transaction.RollbackAsync();
                }
            }

            return results;
        }


    }
}

