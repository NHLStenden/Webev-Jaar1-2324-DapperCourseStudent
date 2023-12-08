namespace DapperCourse;

public class Parameters
{
    //try to create a sql injection
    
    
    //parameters 
    
    //parameters optional (NULL), filtering, sorting, etc.
    
    //In Parameters
    
    //working towards pagination
    
    
    //SqlBuilder
    void SqlBuilderExample()
    {
//         var builder = new SqlBuilder();
//         builder.Select("id_something");
//         builder.Select("MyCol");
//         builder.Select("OtherCol");
//         DynamicParameters parameters = new DynamicParameters();
//         parameters.Add("@MyParam", 3, DbType.Int32, ParameterDirection.Input);
//         builder.Where("id_something < @MyParam", parameters);
// // builder.Where("id_something < @MyParam", new { MyParam =3}); //this is other option for params.
//         builder.InnerJoin("OtherTable on OtherTable.id=MyTable.id");
// //The /**something**/ are placeholders,
//         var builderTemplate = builder.AddTemplate("Select /**select**/ from MyTable /**innerjoin**/ /**where**/ ");
//         var result = connection.Query<MyClass>(builderTemplate.RawSql, builderTemplate.Parameters);
    }
}