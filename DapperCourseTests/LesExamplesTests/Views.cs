using DapperCourse.LesExamples;
using FluentAssertions;

namespace DapperCourseTests.LesExamplesTests;

public class Views
{
    [Test]
    public void BierenViews()
    {
        List<BierExample.BrouwerMetAantalBieren> bieren = BierExample.GetBrouwersMetAantalBieren();
        bieren.Should().HaveCount(8);
    }
}