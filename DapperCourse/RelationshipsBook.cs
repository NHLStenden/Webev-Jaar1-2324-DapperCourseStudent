using Dapper;
using Dapper.Mapper;
using MySql.Data.MySqlClient;

namespace DapperCourse;

public class RelationshipsBook
{

    
    public string GetConnectionStringForBooks()
    {
        return "Server=localhost;port=3306;Database=Books;Uid=root;Pwd=Test@1234!";
    }


    
    public List<Book> GetBookWithAuthor()
    {
        using var connection = new MySqlConnection(GetConnectionStringForBooks());
        
        string sql = "SELECT *  FROM Authors A JOIN Books B on A.Id = B.AuthorId";
        var books = connection.Query<Book, Author, Book>(sql, 
            (book, author) =>
            {
                book.Author = author;
                // book.Author = author;
                return book;
            }, splitOn: "AuthorId");
        return books.ToList();
    }

    public List<Book> SmartMapper()
    {
        string sql = "SELECT *  FROM Books JOIN Authors ON Authors.Id = Books.AuthorId";
        using var connection = new MySqlConnection(GetConnectionStringForBooks());
        var books = connection.Query<Book, Author>(sql);
        var r = books.ToList();
        return r;
    }
    

    
    public List<Author> GetAuthorsWithBooks()
    {
        using var connection = new MySqlConnection(GetConnectionStringForBooks());
         
        var authorDictionary = new Dictionary<int, Author>();
        
        string sql = "SELECT *  FROM Authors A JOIN Books B on A.Id = B.AuthorId";
        var duplicatedAuthors = connection.Query<Author, Book, Author>(sql, 
            (author, book) =>
            {
                if (!authorDictionary.ContainsKey(author.Id))
                {
                    author.Books ??= new List<Book>();
                    authorDictionary.Add(author.Id, author);
                }
                author = authorDictionary[author.Id];
                author.Books.Add(book);
                return author;
            }, splitOn: "BookId");

        var authors = duplicatedAuthors.DistinctBy(x => x.Id).ToList();
        
        return authors;
    }

}