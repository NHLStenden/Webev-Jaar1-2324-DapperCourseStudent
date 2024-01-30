using Dapper;
using MySqlConnector;

namespace DapperCourseTests.LesExamples;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool Completed { get; set; } = false;
}

public class TodoDemo
{
    private static readonly string ConnectionString;
    static TodoDemo()
    {
        ConnectionString = ConnectionStrings.GetConnectionStringMovies();
    }
    
    public static List<Todo> Get()
    {
        using var connection = new MySqlConnection(ConnectionString);
        return connection.Query<Todo>("SELECT Id, Name, Completed FROM Todo")
                        .ToList();
    }
    
    public static Todo? Get(int id)
    {
        string sql = "SELECT Id, Name, Completed FROM Todo WHERE Id = @Id";
        using var connection = new MySqlConnection(ConnectionString);
        return connection.QuerySingleOrDefault<Todo>(sql, new { Id = id });
    }
    
    public static int NumberOfTodos()
    {
        using var connection = new MySqlConnection(ConnectionString);
        return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Todo");
    }
    
    public static int Create(Todo todo)
    {
        using var connection = new MySqlConnection(ConnectionString);
        var sql = "INSERT INTO Todo (Name, Completed) VALUES (@Name, @Completed); " +
                  "SELECT LAST_INSERT_ID();";
        var id = connection.ExecuteScalar<int>(sql, todo);
        return id;
    }
    
    public static void Update(Todo todo)
    {
        var sql = "UPDATE Todo SET Name = @Name, Completed = @Completed WHERE Id = @Id";
        using var connection = new MySqlConnection(ConnectionString);
        connection.Execute(sql, todo);
    }
    
    public static Todo? UpdateAndSelect(Todo todo)
    {
        var sql = "UPDATE Todo SET Name = @Name, Completed = @Completed WHERE Id = @Id; "
                  +"SELECT Id, Name, Completed FROM Todo WHERE Id = @Id";
        
        using var connection = new MySqlConnection(ConnectionString);
        // var updatedTodo = TodoDemo.Get(todo.Id);
        var updatedTodo = connection.QuerySingle<Todo>(sql);
        return updatedTodo;
    }
    
    public static void Delete(int id)
    {
        using var connection = new MySqlConnection(ConnectionString);
        var sql = "DELETE FROM Todo WHERE Id = @Id";
        connection.Execute(sql, new { Id = id });
    }
}