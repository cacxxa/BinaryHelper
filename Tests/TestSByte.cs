using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestSByte
    {
        private class Test
        {
            [Binary]
            public sbyte Field;

            [Binary]
            public sbyte Property { get; set; }
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x80 }, 1)] // корректный
        [InlineData(new byte[] { 0xff }, 2)] // недостаточно байтов
        [InlineData(new byte[0], 3)]
        [InlineData(null, 4)]
        public void Read(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(1, test.Field);
                    Assert.Equal(-128, test.Property);
                    break;
                case 2: // недостаточно байтов
                    Assert.Equal(-1, test.Field);
                    Assert.Equal(0, test.Property);
                    break;
                default:
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.Property);
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
            Test test = new Test {Field = 1, Property = 2};
            byte[] expected = {0x01, 0x02};
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }
    }
}