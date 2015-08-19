using Xunit;

namespace Hgaard.Nemlogin.Signing.Tests
{
    public class SigningTest
    {
        [Fact]
        public void VerifySimpleSigning()
        {
            // Arrange
            var request = Signer.BuildRequest("myFirstId", "Hello signer", "http://a-dummy-url"); 


            // Act 

            //Assert
            Assert.Equal()
        }
    }
}
