using AutoFixture.Kernel;

namespace WebApi.Tests.Helper;

public class ValidDateOnlyBuilder : ISpecimenBuilder
{
    private static readonly Random _random = new();

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(DateOnly))
            return new AutoFixture.Kernel.NoSpecimen();

        var year = _random.Next(2000, 2050);
        var month = _random.Next(1, 13);
        var day = _random.Next(1, DateTime.DaysInMonth(year, month) + 1);

        return new DateOnly(year, month, day);
    }
}
