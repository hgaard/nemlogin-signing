using Xunit;

namespace Hgaard.Nemlogin.Signing.Tests
{
    public class SigningTest
    {
        [Fact]
        public void VerifySimpleSigning()
        {
            // Arrange
            // todo

            // Act 
            var request = Signer.BuildRequest("myFirstId", "Hello signer", "http://a-dummy-url");

            //Assert
            Assert.Equal(request.RequestId, "myFirstId");
        }
    }
}
