using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;
using SecureFlight.Infrastructure.Repositories;
using Xunit;

namespace SecureFlight.Test
{
    public class AirportTests
    {
        [Fact]
        public async Task Update_Succeeds()
        {
            //Arrange
            var testingContext = new SecureFlightDatabaseTestContext();
            testingContext.CreateDatabase();

            var repository = new BaseRepository<Airport>(testingContext);
            var mockRepository = A.Fake<IRepository<Airport>>(x => x.Strict());

            var updateCall = A.CallTo(() => mockRepository.Update(A<Airport>._));
            updateCall.Returns(new Airport());

            var saveCall = A.CallTo(() => mockRepository.SaveChangesAsync());
            saveCall.Returns(1);

            //Act
            var action = async () => {
                mockRepository.Update(new Airport() { Name = "Test" });
                await mockRepository.SaveChangesAsync();
            };

            //Assert
            await action.Should().NotThrowAsync();
            updateCall.MustHaveHappenedOnceExactly();

            saveCall.MustHaveHappenedOnceExactly();
            testingContext.DisposeDatabase();
        }
    }
}
