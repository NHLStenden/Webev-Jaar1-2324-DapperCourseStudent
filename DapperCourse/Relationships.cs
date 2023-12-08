namespace DapperCourse;

// https://www.infoworld.com/article/3705055/how-to-map-object-relationships-using-dapper-in-aspnet-core.html


public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public List<Book> Books { get; set; }
}

public class Book
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; }
    
    public Author Author { get; set; }
}

