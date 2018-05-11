using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestUInt
    {
        private class Test
        {
            [Binary]
            public uint Field;

            [Binary]
            public uint Property { get; set; }

            [Binary(Count = 1)]
            public uint CountTest1 { get; set; }
            
            [Binary(Count = 2)]
            public uint CountTest2 { get; set; }

            [Binary(Count = 3)]
            public uint CountTest3 { get; set; }

            [Binary(Reverse = true)]
            public uint ReverseTest { get; set; }

            [Binary(Reverse = true, Count = 1)]
            public uint ReverseCountTest1 { get; set; }

            [Binary(Reverse = true, Count = 2)]
            public uint ReverseCountTest2 { get; set; }

            [Binary(Reverse = true, Count = 3)]
            public uint ReverseCountTest3 { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0xff, 0xff, 0xff, 0xff, //field
            0xa0, 0x86, 0x01, 0x00, //Property
            0xc8, //CountTest1
            0x50, 0xc3, //CountTest2
            0xff, 0xff, 0xff, //CountTest3
            0x00, 0x01, 0x86, 0xa0, //ReverseTest1
            0x85, //ReverseCountTest1
            0x00, 0x0c, //ReverseCountTest2
            0x98, 0x96, 0x80 //ReverseCountTest3
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x01, 0x00, 0x00
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0xa0, 0x86, 0x01, 0x00,
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
                    Assert.Equal(4_294_967_295, test.Field);
                    Assert.Equal((uint)100000, test.Property);
                    Assert.Equal((uint)200, test.CountTest1);
                    Assert.Equal((uint)50000, test.CountTest2);
                    Assert.Equal((uint)16_777_215, test.CountTest3);
                    Assert.Equal((uint)100000, test.ReverseTest);
                    Assert.Equal((uint)133, test.ReverseCountTest1);
                    Assert.Equal((uint)12, test.ReverseCountTest2);
                    Assert.Equal((uint)10_000_000, test.ReverseCountTest3);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal((uint)1, test.Field);
                    Assert.Equal((uint)0, test.Property);
                    Assert.Equal((uint)0, test.CountTest1);
                    Assert.Equal((uint)0, test.CountTest2);
                    Assert.Equal((uint)0, test.CountTest3);
                    Assert.Equal((uint)0, test.ReverseTest);
                    Assert.Equal((uint)0, test.ReverseCountTest1);
                    Assert.Equal((uint)0, test.ReverseCountTest2);
                    Assert.Equal((uint)0, test.ReverseCountTest3);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal((uint)100000, test.Field);
                    Assert.Equal((uint)16, test.Property);
                    Assert.Equal((uint)0, test.CountTest1);
                    Assert.Equal((uint)0, test.CountTest2);
                    Assert.Equal((uint)0, test.CountTest3);
                    Assert.Equal((uint)0, test.ReverseTest);
                    Assert.Equal((uint)0, test.ReverseCountTest1);
                    Assert.Equal((uint)0, test.ReverseCountTest2);
                    Assert.Equal((uint)0, test.ReverseCountTest3);
                    break;
                default:
                    Assert.Equal((uint)0, test.Field);
                    Assert.Equal((uint)0, test.Property);
                    Assert.Equal((uint)0, test.CountTest1);
                    Assert.Equal((uint)0, test.CountTest2);
                    Assert.Equal((uint)0, test.CountTest3);
                    Assert.Equal((uint)0, test.ReverseTest);
                    Assert.Equal((uint)0, test.ReverseCountTest1);
                    Assert.Equal((uint)0, test.ReverseCountTest2);
                    Assert.Equal((uint)0, test.ReverseCountTest3);
                    break;
            }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0xff, 0xff, 0xff, 0xff, //field
            0x00, 0x01, 0x86, 0xa0, //Property
            0xc8, //CountTest1
            0xc3, 0x50, //CountTest2
            0xff, 0xff, 0xff, //CountTest3
            0xa0, 0x86, 0x01, 0x00, //ReverseTest1
            0x85, //ReverseCountTest1
            0x0c, 0x00, //ReverseCountTest2
            0x80, 0x96, 0x98 //ReverseCountTest3
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x00, 0x00, 0x01
        }, 2)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x00, 0x01, 0x86, 0xa0,
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
                    Assert.Equal(4_294_967_295, test.Field);
                    Assert.Equal((uint)100000, test.Property);
                    Assert.Equal((uint)200, test.CountTest1);
                    Assert.Equal((uint)50000, test.CountTest2);
                    Assert.Equal((uint)16_777_215, test.CountTest3);
                    Assert.Equal((uint)100000, test.ReverseTest);
                    Assert.Equal((uint)133, test.ReverseCountTest1);
                    Assert.Equal((uint)12, test.ReverseCountTest2);
                    Assert.Equal((uint)10_000_000, test.ReverseCountTest3);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal((uint)256, test.Field);
                    Assert.Equal((uint)0, test.Property);
                    Assert.Equal((uint)0, test.CountTest1);
                    Assert.Equal((uint)0, test.CountTest2);
                    Assert.Equal((uint)0, test.CountTest3);
                    Assert.Equal((uint)0, test.ReverseTest);
                    Assert.Equal((uint)0, test.ReverseCountTest1);
                    Assert.Equal((uint)0, test.ReverseCountTest2);
                    Assert.Equal((uint)0, test.ReverseCountTest3);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal((uint)100000, test.Field);
                    Assert.Equal((uint)0x10000000, test.Property);
                    Assert.Equal((uint)0, test.CountTest1);
                    Assert.Equal((uint)0, test.CountTest2);
                    Assert.Equal((uint)0, test.CountTest3);
                    Assert.Equal((uint)0, test.ReverseTest);
                    Assert.Equal((uint)0, test.ReverseCountTest1);
                    Assert.Equal((uint)0, test.ReverseCountTest2);
                    Assert.Equal((uint)0, test.ReverseCountTest3);
                    break;
                default:
                    Assert.Equal((uint)0, test.Field);
                    Assert.Equal((uint)0, test.Property);
                    Assert.Equal((uint)0, test.CountTest1);
                    Assert.Equal((uint)0, test.CountTest2);
                    Assert.Equal((uint)0, test.CountTest3);
                    Assert.Equal((uint)0, test.ReverseTest);
                    Assert.Equal((uint)0, test.ReverseCountTest1);
                    Assert.Equal((uint)0, test.ReverseCountTest2);
                    Assert.Equal((uint)0, test.ReverseCountTest3);
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
                Field = 4_294_967_295, Property = 100000, CountTest1 = 200, CountTest2 = 50000, CountTest3 = 16_777_215,
                ReverseTest = 100000, ReverseCountTest1 = 133, ReverseCountTest2 = 12, ReverseCountTest3 = 10_000_000
            };
            byte[] expected =
            {
                0xff, 0xff, 0xff, 0xff, //field
                0xa0, 0x86, 0x01, 0x00, //Property
                0xc8, //CountTest1
                0x50, 0xc3, //CountTest2
                0xff, 0xff, 0xff, //CountTest3
                0x00, 0x01, 0x86, 0xa0, //ReverseTest1
                0x85, //ReverseCountTest1
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
                Field = 4_294_967_295, Property = 100000, CountTest1 = 200, CountTest2 = 50000, CountTest3 = 16_777_215,
                ReverseTest = 100000, ReverseCountTest1 = 133, ReverseCountTest2 = 12, ReverseCountTest3 = 10_000_000
            };
            byte[] expected =
            {
                0xff, 0xff, 0xff, 0xff, //field
                0x00, 0x01, 0x86, 0xa0, //Property
                0xc8, //CountTest1
                0xc3, 0x50, //CountTest2
                0xff, 0xff, 0xff, //CountTest3
                0xa0, 0x86, 0x01, 0x00, //ReverseTest1
                0x85, //ReverseCountTest1
                0x0c, 0x00, //ReverseCountTest2
                0x80, 0x96, 0x98 //ReverseCountTest3
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }
    }
}