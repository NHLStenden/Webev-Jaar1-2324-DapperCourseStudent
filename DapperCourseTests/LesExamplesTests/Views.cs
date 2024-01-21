using DapperCourse.LesExamples;
using FluentAssertions;

namespace DapperCourseTests.LesExamplesTests;

public class Views
{
    [Test]
    public void BierenViews()
    {
        var bieren = BierExample.GetBrouwersMetAantalBieren();
        bieren.Should().HaveCount(8);
    }
}