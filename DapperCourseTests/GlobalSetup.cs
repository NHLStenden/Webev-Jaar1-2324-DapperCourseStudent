namespace DapperCourseTests;

[SetUpFixture]
public class GlobalSetup
{

    [OneTimeSetUp]
    public void SetupVerifyConfiguration()
    {
        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(projectDirectory, "Snapshots"),
                typeName: type.Name,
                methodName: method.Name));
    }
}