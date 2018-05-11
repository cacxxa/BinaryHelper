using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestULong
    {
        private class Test
        {
            [Binary]
            public ulong Field;

            [Binary]
            public ulong Property { get; set; }

            [Binary(Count = 1)]
            public ulong CountTest1 { get; set; }

            [Binary(Count = 2)]
            public ulong CountTest2 { get; set; }

            [Binary(Count = 3)]
            public ulong CountTest3 { get; set; }

            [Binary(Reverse = true)]
            public ulong ReverseTest { get; set; }

            [Binary(Reverse = true, Count = 1)]
            public ulong ReverseCountTest1 { get; set; }

            [Binary(Reverse = true, Count = 2)]
            public ulong ReverseCountTest2 { get; set; }

            [Binary(Reverse = true, Count = 3)]
            public ulong ReverseCountTest3 { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, //field
            0xa0, 0x86, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, //Property
            0xc8, //CountTest1
            0x50, 0xc3, //CountTest2
            0xff, 0xff, 0xff, //CountTest3
            0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x86, 0xa0, //ReverseTest1
            0x64, //ReverseCountTest1
            0x00, 0x0c, //ReverseCountTest2
            0x98, 0x96, 0x80 //ReverseCountTest3
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x01, 0x00, 0x00
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0xa0, 0x86, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
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
                    Assert.Equal(18_446_744_073_709_551_615, test.Field);
                    Assert.Equal((ulong)100000, test.Property);
                    Assert.Equal((ulong)200, test.CountTest1);
                    Assert.Equal((ulong)50000, test.CountTest2);
                    Assert.Equal((ulong)16_777_215, test.CountTest3);
                    Assert.Equal((ulong)100000, test.ReverseTest);
                    Assert.Equal((ulong)100, test.ReverseCountTest1);
                    Assert.Equal((ulong)12, test.ReverseCountTest2);
                    Assert.Equal((ulong)10_000_000, test.ReverseCountTest3);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal((ulong)1, test.Field);
                    Assert.Equal((ulong)0, test.Property);
                    Assert.Equal((ulong)0, test.CountTest1);
                    Assert.Equal((ulong)0, test.CountTest2);
                    Assert.Equal((ulong)0, test.CountTest3);
                    Assert.Equal((ulong)0, test.ReverseTest);
                    Assert.Equal((ulong)0, test.ReverseCountTest1);
                    Assert.Equal((ulong)0, test.ReverseCountTest2);
                    Assert.Equal((ulong)0, test.ReverseCountTest3);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal((ulong)100000, test.Field);
                    Assert.Equal((ulong)16, test.Property);
                    Assert.Equal((ulong)0, test.CountTest1);
                    Assert.Equal((ulong)0, test.CountTest2);
                    Assert.Equal((ulong)0, test.CountTest3);
                    Assert.Equal((ulong)0, test.ReverseTest);
                    Assert.Equal((ulong)0, test.ReverseCountTest1);
                    Assert.Equal((ulong)0, test.ReverseCountTest2);
                    Assert.Equal((ulong)0, test.ReverseCountTest3);
                    break;
                default:
                    Assert.Equal((ulong)0, test.Field);
                    Assert.Equal((ulong)0, test.Property);
                    Assert.Equal((ulong)0, test.CountTest1);
                    Assert.Equal((ulong)0, test.CountTest2);
                    Assert.Equal((ulong)0, test.CountTest3);
                    Assert.Equal((ulong)0, test.ReverseTest);
                    Assert.Equal((ulong)0, test.ReverseCountTest1);
                    Assert.Equal((ulong)0, test.ReverseCountTest2);
                    Assert.Equal((ulong)0, test.ReverseCountTest3);
                    break;
            }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, //field
            0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x86, 0xa0, //Property
            0xc8, //CountTest1
            0xc3, 0x50, //CountTest2
            0xff, 0xff, 0xff, //CountTest3
            0xa0, 0x86, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, //ReverseTest1
            0x64, //ReverseCountTest1
            0x0c, 0x00, //ReverseCountTest2
            0x80, 0x96, 0x98 //ReverseCountTest3
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x00, 0x00, 0x00, 0x01
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x86, 0xa0,
            0x00, 0x00, 0x00, 0x10
        }, 3)]
        [InlineData(new byte[0], 4)]
        [InlineData(null, 5)]
        public void ReadReverse(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, true);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal((ulong)9_223_372_036_854_775_809, test.Field);
                    Assert.Equal((ulong)100000, test.Property);
                    Assert.Equal((ulong)200, test.CountTest1);
                    Assert.Equal((ulong)50000, test.CountTest2);
                    Assert.Equal((ulong)16_777_215, test.CountTest3);
                    Assert.Equal((ulong)100000, test.ReverseTest);
                    Assert.Equal((ulong)100, test.ReverseCountTest1);
                    Assert.Equal((ulong)12, test.ReverseCountTest2);
                    Assert.Equal((ulong)10_000_000, test.ReverseCountTest3);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal((ulong)0x0100000000, test.Field);
                    Assert.Equal((ulong)0, test.Property);
                    Assert.Equal((ulong)0, test.CountTest1);
                    Assert.Equal((ulong)0, test.CountTest2);
                    Assert.Equal((ulong)0, test.CountTest3);
                    Assert.Equal((ulong)0, test.ReverseTest);
                    Assert.Equal((ulong)0, test.ReverseCountTest1);
                    Assert.Equal((ulong)0, test.ReverseCountTest2);
                    Assert.Equal((ulong)0, test.ReverseCountTest3);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal((ulong)100000, test.Field);
                    Assert.Equal((ulong)0x1000000000, test.Property);
                    Assert.Equal((ulong)0, test.CountTest1);
                    Assert.Equal((ulong)0, test.CountTest2);
                    Assert.Equal((ulong)0, test.CountTest3);
                    Assert.Equal((ulong)0, test.ReverseTest);
                    Assert.Equal((ulong)0, test.ReverseCountTest1);
                    Assert.Equal((ulong)0, test.ReverseCountTest2);
                    Assert.Equal((ulong)0, test.ReverseCountTest3);
                    break;
                default:
                    Assert.Equal((ulong)0, test.Field);
                    Assert.Equal((ulong)0, test.Property);
                    Assert.Equal((ulong)0, test.CountTest1);
                    Assert.Equal((ulong)0, test.CountTest2);
                    Assert.Equal((ulong)0, test.CountTest3);
                    Assert.Equal((ulong)0, test.ReverseTest);
                    Assert.Equal((ulong)0, test.ReverseCountTest1);
                    Assert.Equal((ulong)0, test.ReverseCountTest2);
                    Assert.Equal((ulong)0, test.ReverseCountTest3);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = Enumerable.Repeat<byte>(0, 36).ToArray();
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Write()
        {
            Test test = new Test
            {
                Field = 18_446_744_073_709_551_615, Property = 100000, CountTest1 = 200, CountTest2 = 50000, CountTest3 = 16_777_215,
                ReverseTest = 100000, ReverseCountTest1 = 100, ReverseCountTest2 = 12, ReverseCountTest3 = 10_000_000
            };
            byte[] expected =
            {
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, //field
                0xa0, 0x86, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, //Property
                0xc8, //CountTest1
                0x50, 0xc3, //CountTest2
                0xff, 0xff, 0xff, //CountTest3
                0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x86, 0xa0, //ReverseTest1
                0x64, //ReverseCountTest1
                0x00, 0x0c, //ReverseCountTest2
                0x98, 0x96, 0x80 //ReverseCountTest3
            };
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void WriteReverse()
        {
            Test test = new Test
            {
                Field = 9_223_372_036_854_775_809, Property = 100000, CountTest1 = 200, CountTest2 = 50000, CountTest3 = 16_777_215,
                ReverseTest = 100000, ReverseCountTest1 = 100, ReverseCountTest2 = 12, ReverseCountTest3 = 10_000_000
            };
            byte[] expected =
            {
                0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, //field
                0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x86, 0xa0, //Property
                0xc8, //CountTest1
                0xc3, 0x50, //CountTest2
                0xff, 0xff, 0xff, //CountTest3
                0xa0, 0x86, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, //ReverseTest1
                0x64, //ReverseCountTest1
                0x0c, 0x00, //ReverseCountTest2
                0x80, 0x96, 0x98 //ReverseCountTest3
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }
    }
}