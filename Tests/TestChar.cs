using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestChar
    {
        private class Test
        {
            [Binary]
            public char Field;

            [Binary]
            public char Property { get; set; }

            [Binary(Count = 1)]
            public char CountTest { get; set; }

            [Binary(Reverse = true)]
            public char ReverseTest { get; set; }

            [Binary(Reverse = true, Count = 1)]
            public char ReverseCountTest { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x64, 0x00, //field
            0x34, 0x04, //Property
            0x68,  //CountTest
            0x04, 0x39, //ReverseTest
            0x34 //ReverseCountTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x64
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x64, 0x00,
            0x34
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void Read(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal('d', test.Field);
                    Assert.Equal('д', test.Property);
                    Assert.Equal('h', test.CountTest);
                    Assert.Equal('й', test.ReverseTest);
                    Assert.Equal('4', test.ReverseCountTest);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal('d', test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal('d', test.Field);
                    Assert.Equal('4', test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                default:
                    Assert.Equal('\0', test.Field);
                    Assert.Equal('\0', test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
            }            
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x00, 0x64, //field
            0x04, 0x34, //Property
            0x04,  //CountTest
            0x39, 0x04, //ReverseTest
            0x68 //ReverseCountTest
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x64
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x00, 0x64,
            0x04, 0x34,
            0x68
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void ReadReverse(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, true);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal('d', test.Field);
                    Assert.Equal('д', test.Property);
                    Assert.Equal(4, test.CountTest);
                    Assert.Equal('й', test.ReverseTest);
                    Assert.Equal('h', test.ReverseCountTest);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(0x6400, test.Field);
                    Assert.Equal(0, test.Property);
                    Assert.Equal(0, test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal('d', test.Field);
                    Assert.Equal('д', test.Property);
                    Assert.Equal('h', test.CountTest);
                    Assert.Equal(0, test.ReverseTest);
                    Assert.Equal(0, test.ReverseCountTest);
                    break;
                default:
                    Assert.Equal('\0', test.Field);
                    Assert.Equal('\0', test.Property);
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
            Test test = new Test { Field = 'd', Property = 'д', CountTest = 'ж', ReverseTest = 'й', ReverseCountTest = 'л' };
            byte[] expected =
            {
                0x64, 0x00, //field
                0x34, 0x04, //Property
                0x36,  //CountTest
                0x04, 0x39, //ReverseTest
                0x3b //ReverseCountTest
            };
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void WriteReverse()
        {
            Test test = new Test { Field = 'd', Property = 'д', CountTest = 'ж', ReverseTest = 'й', ReverseCountTest = 'л' };
            byte[] expected =
            {
                0x00, 0x64, //field
                0x04, 0x34, //Property
                0x36,  //CountTest
                0x39, 0x04, //ReverseTest
                0x3b //ReverseCountTest
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }
    }
}