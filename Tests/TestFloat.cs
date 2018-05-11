using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestFloat
    {
        private class Test
        {
            [Binary]
            public float Field;

            [Binary]
            public float Property { get; set; }

            [Binary(Reverse = true)]
            public float ReverseTest { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x00, 0x00, 0xc8, 0x42, //field
            0x00, 0x00, 0x00, 0x3f, //Property
            0xbb, 0xa7, 0x1d, 0xe7 //ReverseTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x00, 0x00, 0x01
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x00, 0x00, 0xc8, 0x42,
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
            0x42, 0xc8, 0x00, 0x00, //field
            0x3f, 0x00, 0x00, 0x00, //Property
            0xe7, 0x1d, 0xa7, 0xbb //ReverseTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x40, 0x00, 0x00
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x42, 0xc8, 0x00, 0x00,
            0x42
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
                    Assert.Equal(32, test.Property, 6);
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
            byte[] expected = Enumerable.Repeat<byte>(0, 12).ToArray();
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
                0x00, 0x00, 0xc8, 0x42, //field
                0x00, 0x00, 0x00, 0x3f, //Property
                0xbb, 0xa7, 0x1d, 0xe7 //ReverseTest
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
                0x42, 0xc8, 0x00, 0x00, //field
                0x3f, 0x00, 0x00, 0x00, //Property
                0xe7, 0x1d, 0xa7, 0xbb //ReverseTest
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }
    }
}