using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestDouble
    {
        private class Test
        {
            [Binary]
            public double Field;

            [Binary]
            public double Property { get; set; }

            [Binary(Reverse = true)]
            public double ReverseTest { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x59, 0x40, //field
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xe0, 0x3f, //Property
            0xbf, 0x74, 0xe3, 0xbc, 0xe0, 0x00, 0x00, 0x00  //ReverseTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x00, 0x00, 0x01
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x59, 0x40,
            0x10
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void Read(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(100, test.Field);
                    Assert.Equal(0.5, test.Property, 6);
                    Assert.Equal(-5.1e-3, test.ReverseTest, 6);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(0, test.Field, 6);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.ReverseTest);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal(100, test.Field);
                    Assert.Equal(0, test.Property, 6);
                    Assert.Equal(0, test.ReverseTest);
                    break;
                default:
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.ReverseTest);
                    break;
            }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x40, 0x59, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //field
            0x3f, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //Property
            0x00, 0x00, 0x00, 0xe0, 0xbc, 0xe3, 0x74, 0xbf  //ReverseTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x40, 0x00, 0x00
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x40, 0x59, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x40, 0x41, 0x40
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void ReadReverse(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, true);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(100, test.Field);
                    Assert.Equal(0.5, test.Property, 6);
                    Assert.Equal(-5.1e-3, test.ReverseTest, 6);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(2, test.Field, 6);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.ReverseTest);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal(100, test.Field, 6);
                    Assert.Equal(34.5, test.Property, 6);
                    Assert.Equal(0, test.ReverseTest);
                    break;
                default:
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.ReverseTest);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = Enumerable.Repeat<byte>(0, 24).ToArray();
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Write()
        {
            Test test = new Test
            {
                Field = 100, Property = 0.5f, ReverseTest = -5.1e-3f
            };
            byte[] expected =
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x59, 0x40, //field
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xe0, 0x3f, //Property
                0xbf, 0x74, 0xe3, 0xbc, 0xe0, 0x00, 0x00, 0x00  //ReverseTest
            };

            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void WriteReverse()
        {
            Test test = new Test
            {
                Field = 100, Property = 0.5f, ReverseTest = -5.1e-3f
            };
            byte[] expected =
            {
                0x40, 0x59, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //field
                0x3f, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //Property
                0x00, 0x00, 0x00, 0xe0, 0xbc, 0xe3, 0x74, 0xbf  //ReverseTest
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }
    }
}