using System.Data;
using Application.Common.SQL.ResponseModels;
using Application.Common.SQL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;

public class SqlManager
{
    private readonly string _connectionString;
    private readonly DataComparer _dataComparer;

    public SqlManager(string connectionString)
    {
        _connectionString = connectionString;
        _dataComparer = new DataComparer();
    }

    public async Task<SqlValidateResponse> ValidateSqlSyntaxAsync(string sql, bool safeValidate = false)
    {
        try
        {
            if (safeValidate)
            {
                string wrappedSql = $"SET NOEXEC ON;\n{sql}\nSET NOEXEC OFF;";
                await ExecuteQueryAsync(wrappedSql, suppressResults: true);
            }
            else
            {
                await ExecuteQueryAsync(sql);
            }

            return new SqlValidateResponse { IsValid = true };
        }
        catch (SqlException ex)
        {
            return new SqlValidateResponse { Error = ex.Message, IsValid = false };
        }
    }


    public async Task<CompareResponse> CompareSqlResultsAsync(string sql1, string sql2)
    {
        bool sql1HasTrigger = sql1.IndexOf("CREATE TRIGGER", StringComparison.OrdinalIgnoreCase) >= 0;
        bool sql2HasTrigger = sql2.IndexOf("CREATE TRIGGER", StringComparison.OrdinalIgnoreCase) >= 0;
        
        List<Dictionary<string, object>> task1;
        List<Dictionary<string, object>> task2;
        if (sql1HasTrigger)
        {
            task1 = await ExecuteQueryWithTriggerMessagesAndRollbackAsync(sql1);
        }
        else
        {
            task1 = await ExecuteQueryAsync(sql1);
        }

        if (sql2HasTrigger)
        {
            task2 = await ExecuteQueryWithTriggerMessagesAndRollbackAsync(sql2);
        }
        else
        {
            task2 = await ExecuteQueryAsync(sql2);
        }
        

        bool isCompare = _dataComparer.CompareResults(task1, task2);

        return new CompareResponse
        {
            DataIsEqual = isCompare,
            QueryData1 = task1,
            QueryData2 = task2
        };
    }

    public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql, bool suppressResults = false)
    {
        var sqlCommands = sql.Split(new[] { "\nGO", "\r\nGO" }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(command => command.Trim())
                             .Where(command => !string.IsNullOrWhiteSpace(command))
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
                    if (suppressResults)
                    {
                        using var cmd = new SqlCommand(command, connection, (SqlTransaction)transaction);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        var queryResult = await connection.QueryAsync(command, transaction: (SqlTransaction)transaction);
                        results.AddRange(queryResult
                            .Select(row => (IDictionary<string, object>)row)
                            .Select(dict => dict.ToDictionary(kv => kv.Key, kv => kv.Value)));
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                await transaction.RollbackAsync(); // гарантированный откат
            }
        }

        return results;
    }

    public async Task<List<Dictionary<string, object>>> ExecuteQueryWithTriggerMessagesAndRollbackAsync(string sql)
    {
        var messagesWithTypes = new List<Dictionary<string, object>>();

        var sqlCommands = sql.Split(new[] { "\nGO", "\r\nGO" }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(command => command.Trim())
                             .Where(command => !string.IsNullOrWhiteSpace(command))
                             .ToList();

        using (var connection = new SqlConnection(_connectionString))
        {
            var collectedMessages = new List<(string Message, string QueryType)>();

            connection.InfoMessage += (sender, e) =>
            {
                foreach (SqlError info in e.Errors)
                {
                    collectedMessages.Add((info.Message, null));
                }
            };

            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                foreach (var command in sqlCommands)
                {
                    string queryType = GetQueryTypeWithTable(command);
                    collectedMessages.Clear();

                    using var cmd = new SqlCommand(command, connection, transaction);

                    await cmd.ExecuteNonQueryAsync();

                    foreach (var (message, _) in collectedMessages)
                    {
                        messagesWithTypes.Add(new Dictionary<string, object>
                        {
                            [message] = new {
                                QueryType = queryType,
                                Message = message
                            }
                        });
                    }
                }

                await transaction.RollbackAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Ошибка выполнения SQL команды: " + ex.Message, ex);
            }
        }

        return messagesWithTypes;
    }

    private string GetQueryTypeWithTable(string sqlCommand)
    {
        if (string.IsNullOrWhiteSpace(sqlCommand))
            return "UNKNOWN";

        var words = sqlCommand.Split(new[] { ' ', '\t', '\r', '\n', '(', ';' }, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
            return "UNKNOWN";

        string type = words[0].ToUpperInvariant();

        if (type == "INSERT")
        {
            if (words.Length > 2 && words[1].Equals("INTO", StringComparison.OrdinalIgnoreCase))
            {
                string tableName = words[2];
                return $"{type}_{tableName}";
            }
        }
        else if (type == "UPDATE")
        {
            if (words.Length > 1)
            {
                string tableName = words[1];
                return $"{type}_{tableName}";
            }
        }
        else if (type == "DELETE")
        {
            if (words.Length > 2 && words[1].Equals("FROM", StringComparison.OrdinalIgnoreCase))
            {
                string tableName = words[2];
                return $"{type}_{tableName}";
            }
        }
        else if (type == "CREATE")
        {
            if (words.Length > 2 && words[1].Equals("TABLE", StringComparison.OrdinalIgnoreCase))
            {
                string tableName = words[2];
                return $"{type}_TABLE_{tableName}";
            }
            if (words.Length > 2 && words[1].Equals("TRIGGER", StringComparison.OrdinalIgnoreCase))
            {
                string triggerName = words[2];
                return $"{type}_TRIGGER_{triggerName}";
            }
        }
        else if (type == "ALTER")
        {
            if (words.Length > 2 && words[1].Equals("TABLE", StringComparison.OrdinalIgnoreCase))
            {
                string tableName = words[2];
                return $"{type}_TABLE_{tableName}";
            }
        }

        return type;
    }

    
}
