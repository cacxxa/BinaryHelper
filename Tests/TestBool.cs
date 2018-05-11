using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestBool
    {
        private class Test
        {
            [Binary]
            public bool Field;

            [Binary]
            public bool Property { get; set; }
        }

        [Theory]
        [InlineData(new byte[] {0x00, 0x01}, 1)] // корректный
        [InlineData(new byte[] {0x01}, 2)] // недостаточно байтов
        [InlineData(new byte[0], 3)]
        [InlineData(null, 4)]
        public void Read(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes);
            switch (variant)
            {
                case 1: //корректный
                    Assert.False(test.Field);
                    Assert.True(test.Property);
                    break;
                case 2: // недостаточно байтов
                    Assert.True(test.Field);
                    Assert.False(test.Property);
                    break;
                default:
                    Assert.False(test.Field);
                    Assert.False(test.Property);
                    break;
            }            
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = {0x00, 0x00};
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Write()
        {
            Test test = new Test {Field = true, Property = true};
            byte[] expected = {0x01, 0x01};
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }
    }
}