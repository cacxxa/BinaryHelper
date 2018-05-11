using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestReadWrite
    {
        private class Test
        {
            [Binary]
            public byte Field;

            [Binary(Skip = 2)]
            public byte SkipTest { get; set; }

            [Binary(Order = -1)]
            public byte OrderTest { get; set; }

            [Binary]
            public byte ReadOnly { get; } = 70;

            [Binary(IgnoreOnWrite = true)]
            public byte IgnoreWriteTest { get; set; }

            [Binary(IgnoreOnRead = true)]
            public byte IgnoreReadTest { get; set; } = 100;

            [Binary]
            public short Property { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x14, // OrderTest
            0x05, // Field
            0xfa, 0x12, 0x0a, //SkipTest
            0x46, // ReadOnly
            0x32, // IgnoreWriteTest
            0x78, 0x7d // Property
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x14
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x14, // OrderTest
            0x05, // Field
            0xfa, 0x12, 0x0a, //SkipTest
            0x46, // ReadOnly
            0x32, // IgnoreWriteTest
            0x78
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void Read(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(5, test.Field);
                    Assert.Equal(10, test.SkipTest);
                    Assert.Equal(20, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(50, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(32120, test.Property);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.SkipTest);
                    Assert.Equal(20, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(0, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(0, test.Property);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal(5, test.Field);
                    Assert.Equal(10, test.SkipTest);
                    Assert.Equal(20, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(50, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(120, test.Property);
                    break;
                default:
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.SkipTest);
                    Assert.Equal(0, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(0, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(0, test.Property);
                    break;
            }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x14, // OrderTest
            0x05, // Field
            0xfa, 0x12, 0x0a, //SkipTest
            0x46, // ReadOnly
            0x32, // IgnoreWriteTest
            0x7d, 0x78 // Property
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x14
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x14, // OrderTest
            0x05, // Field
            0xfa, 0x12, 0x0a, //SkipTest
            0x46, // ReadOnly
            0x32, // IgnoreWriteTest
            0x78
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void ReadReverse(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, true);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(5, test.Field);
                    Assert.Equal(10, test.SkipTest);
                    Assert.Equal(20, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(50, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(32120, test.Property);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.SkipTest);
                    Assert.Equal(20, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(0, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(0, test.Property);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal(5, test.Field);
                    Assert.Equal(10, test.SkipTest);
                    Assert.Equal(20, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(50, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(30720, test.Property);
                    break;
                default:
                    Assert.Equal(0, test.Field);
                    Assert.Equal(0, test.SkipTest);
                    Assert.Equal(0, test.OrderTest);
                    Assert.Equal(70, test.ReadOnly);
                    Assert.Equal(0, test.IgnoreWriteTest);
                    Assert.Equal(100, test.IgnoreReadTest);
                    Assert.Equal(0, test.Property);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = { 0x00, 0x00, 0x00, 0x46, 0x64, 0x00, 0x00 };
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Write()
        {
            Test test = new Test { Field = 5, SkipTest = 10, OrderTest = 20, IgnoreWriteTest = 50, IgnoreReadTest = 100, Property = 32120};
            byte[] expected =
            {
                0x14, // OrderTest
                0x05, // Field
                0x0a, //SkipTest
                0x46, // ReadOnly
                0x64, // IgnoreReadTest
                0x78, 0x7d // Property
            };

            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void WriteReverse()
        {
            Test test = new Test { Field = 5, SkipTest = 10, OrderTest = 20, IgnoreWriteTest = 50, IgnoreReadTest = 100, Property = 32120 };
            byte[] expected =
            {
                0x14, // OrderTest
                0x05, // Field
                0x0a, //SkipTest
                0x46, // ReadOnly
                0x64, // IgnoreReadTest
                0x7d, 0x78 // Property
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }
    }
}