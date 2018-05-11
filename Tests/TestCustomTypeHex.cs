using System;
using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestCustomTypeHex
    {
        private class Test
        {
            [Binary(CustomType = CustomType.Hex, Count = 3)]
            public string Field;

            [Binary(CustomType = CustomType.Hex, Count = 1)]
            public string Property { get; set; }

            [Binary(CustomType = CustomType.Hex, Count = 3)]
            public string LessTest { get; set; }

            [Binary(CustomType = CustomType.Hex, Count = 3, Reverse = true)]
            public string ReverseTest { get; set; }

            [Binary(CustomType = CustomType.Hex, Count = 3, Reverse = true)]
            public string LessReverseTest { get; set; }

            [Binary(CustomType = CustomType.Hex)]
            public string Infinite { get; set; }
        }

        private class Test2
        {
            [Binary(CustomType = CustomType.Hex)]
            public int Error { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x01, 0x02, 0x03, // Field
            0x04, // Property
            0x00, 0x00, 0x0a, // LessTest
            0x09, 0x08, 0x07, // ReverseTest
            0x0a, 0x00, 0x00, // LessReverseTest
            0x12, 0x34, 0x56, 0x78, 0x90 // Infinite
        }, 1, false)]
        [InlineData(new byte[] // корректный, проверка реверса
        {
            0x01, 0x02, 0x03, // Field
            0x04, // Property
            0x00, 0x00, 0x0a, // LessTest
            0x09, 0x08, 0x07, // ReverseTest
            0x0a, 0x00, 0x00, // LessReverseTest
            0x12, 0x34, 0x56, 0x78, 0x90 // Infinite
        }, 1, true)]
        [InlineData(new byte[] // недостаточно байтов
        {
            0x01, 0x02
        }, 2, false)]
        [InlineData(new byte[0], 3, false)]
        [InlineData(null, 4, false)]
        public void Read(byte[] bytes, byte variant, bool reverse)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, reverse);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal("010203", test.Field);
                    Assert.Equal("04", test.Property);                    
                    Assert.Equal("00000a", test.LessTest, true);
                    Assert.Equal("070809", test.ReverseTest);
                    Assert.Equal("00000a", test.LessReverseTest, true);
                    Assert.Equal("1234567890", test.Infinite);
                    break;
                case 2: // недостаточно байтов
                    Assert.Equal("010200", test.Field);
                    Assert.Equal("00", test.Property);
                    Assert.Equal("000000", test.LessTest);
                    Assert.Equal("000000", test.ReverseTest);
                    Assert.Equal("000000", test.LessReverseTest);
                    Assert.Equal(string.Empty, test.Infinite);
                    break;                
                case 3:
                    Assert.Equal("000000", test.Field);
                    Assert.Equal("00", test.Property);
                    Assert.Equal("000000", test.LessTest);
                    Assert.Equal("000000", test.ReverseTest);
                    Assert.Equal("000000", test.LessReverseTest);
                    Assert.Equal(string.Empty, test.Infinite);
                    break;
                default:
                    Assert.Null(test.Field);
                    Assert.Null(test.Property);
                    Assert.Null(test.LessTest);
                    Assert.Null(test.ReverseTest);
                    Assert.Null(test.LessReverseTest);
                    Assert.Null(test.Infinite);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = Enumerable.Repeat<byte>(0, 13).ToArray();
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Write(bool reverse)
        {
            Test test = new Test { Field = "010203", Property = "040506", LessTest = "0a", ReverseTest = "070809", LessReverseTest = "0a", Infinite = "123456789"};
            byte[] expected =
            {
                0x01, 0x02, 0x03, // Field
                0x04, // Property
                0x00, 0x00, 0x0a, // LessTest
                0x09, 0x08, 0x07, // ReverseTest
                0x0a, 0x00, 0x00, // LessReverseTest
                0x01, 0x23, 0x45, 0x67, 0x89 // Infinite
            };
            byte[] bytes = BinaryConverter.ToBytes(test, reverse);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Exception()
        {
            Test2 test = new Test2 {Error = 123};
            Assert.Equal(123, test.Error);
            Assert.Throws<ArgumentException>(() => BinaryConverter.ToBytes(test));
        }
    }
}