using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestShort
    {
        private class Test
        {
            [Binary]
            public short Field;

            [Binary]
            public short Property { get; set; }

            [Binary(Count = 1)]
            public short CountTest { get; set; }

            [Binary(Reverse = true)]
            public short ReverseTest { get; set; }

            [Binary(Reverse = true, Count = 1)]
            public short ReverseCountTest { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0xf6, 0xff, //field
            0x10, 0x27, //Property
            0xc8,  //CountTest
            0x27, 0x10, //ReverseTest
            0xff //ReverseCountTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0xf6
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0xf6, 0xff,
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
                    Assert.Equal(-10, test.Field);
                    Assert.Equal(10000, test.Property);
                    Assert.Equal(200, test.CountTest);
                    Assert.Equal(10000, test.ReverseTest);
                    Assert.Equal(255, test.ReverseCountTest);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(246, test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal(-10, test.Field);
                    Assert.Equal(16, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                default:
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
            }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0xff, 0xf6, //field
            0x27, 0x10, //Property
            0xc8,  //CountTest
            0x10, 0x27, //ReverseTest
            0x10 //ReverseCountTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x7f
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0xff, 0xf6,
            0x10
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void ReadReverse(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, true);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(-10, test.Field);
                    Assert.Equal(10000, test.Property);
                    Assert.Equal(200, test.CountTest);
                    Assert.Equal(10000, test.ReverseTest);
                    Assert.Equal(16, test.ReverseCountTest);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(0x7f00, test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal(-10, test.Field);
                    Assert.Equal(0x1000, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                default:
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Write()
        {
            Test test = new Test { Field = -10, Property = 10000, CountTest = 200, ReverseTest = 10000, ReverseCountTest = 10000 };
            byte[] expected =
            {
                0xf6, 0xff, //field
                0x10, 0x27, //Property
                0xc8,  //CountTest
                0x27, 0x10, //ReverseTest
                0x10 //ReverseCountTest
            };
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void WriteReverse()
        {
            Test test = new Test { Field = -10, Property = 10000, CountTest = 200, ReverseTest = 10000, ReverseCountTest = 10000 };
            byte[] expected =
            {
                0xff, 0xf6, //field
                0x27, 0x10, //Property
                0xc8,  //CountTest
                0x10, 0x27, //ReverseTest
                0x10 //ReverseCountTest
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }
    }
}