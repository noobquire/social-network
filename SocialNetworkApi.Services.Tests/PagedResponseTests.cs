using FluentAssertions;
using NUnit.Framework;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using System.Linq;

namespace SocialNetworkApi.Services.Tests
{
    [TestFixture]
    public class PagedResponseTests
    {
        [Test]
        public void CreatePagedResponse_EmptyCollection_ReturnsExpectedResponse()
        {
            var filter = new PaginationFilter(1, 1);

            var result = PagedResponse<object>
                .CreatePagedResponse(Enumerable.Empty<object>(), filter);

            result.PageSize.Should().Be(1);
            result.TotalRecords.Should().Be(0);
            result.PageNumber.Should().Be(0);
            result.TotalPages.Should().Be(0);
            result.Data.Should().BeEmpty();
        }

        [Test]
        public void CreatePagedResponse_FilterPageLargerThanTotalPages_ResetsPageNumberToMax()
        {
            var filter = new PaginationFilter(11, 1);

            var result = PagedResponse<int>
                .CreatePagedResponse(Enumerable.Range(1, 10), filter);

            result.PageNumber.Should().Be(10);
            result.TotalPages.Should().Be(10);
        }
    }
}
