using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.SharedKernel;

public sealed class PaginationTests
{
    [Fact]
    public void Create_calculates_offset_from_page_and_page_size()
    {
        var result = Pagination.Create(page: 3, pageSize: 25);

        Assert.True(result.IsSuccess);
        Assert.Equal(50, result.Value.Offset);
    }

    [Theory]
    [InlineData(0, 25)]
    [InlineData(1, 0)]
    [InlineData(1, Pagination.MaxPageSize + 1)]
    public void Create_rejects_invalid_values(int page, int pageSize)
    {
        var result = Pagination.Create(page, pageSize);

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.pagination.invalid", result.Error.Code);
    }
}
