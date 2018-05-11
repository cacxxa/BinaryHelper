using System;
using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestString
    {
        private class Test
        {
            [Binary(Count = 5, Encoding = TextEncoding.Ascii)]
            public string Field;

            [Binary(Count = 6, Encoding = TextEncoding.Utf8)]
            public string Property { get; set; }

            [Binary(Count = 3, Encoding = TextEncoding.Utf8)]
            public string CountTest1 { get; set; }

            [Binary(Count = 5, Encoding = TextEncoding.Utf8)]
            public string CountTest2 { get; set; }

            [Binary(Count = 3, Encoding = TextEncoding.Unicode)]
            public string CountTest3 { get; set; }

            [Binary(Count = 3, Encoding = TextEncoding.DosRus, Reverse = true)]
            public string ReverseTest1 { get; set; }

            [Binary(Count = 4, Encoding = TextEncoding.Utf8, Reverse = true)]
            public string ReverseTest2 { get; set; }

            [Binary(Encoding = TextEncoding.WinRus)]
            public string Infinite { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x61, 0x62, 0x3f, 0x3f, 0x3f, // Field
            0x41, 0x62, 0xd0, 0x93, 0xd0, 0xb3, // Property
            0x44, 0xd0, 0xb4, // CountTest1
            0xd0, 0x96, 0x20, 0x20, 0x20, // CountTest2
            0x22, 0x04, 0x35, // CountTest3
            0x20, 0xaa, 0x8e, // ReverseTest1
            0x8b, 0xd1, 0x93, 0xd0, // ReverseTest2
            0xcf, 0xf0, 0xe8, 0xe2, 0xe5, 0xf2, 0x21 // Infinite
        }, 1, false)]
        [InlineData(new byte[] // корректный, проверка реверса
        {
            0x61, 0x62, 0x3f, 0x3f, 0x3f, // Field
            0x41, 0x62, 0xd0, 0x93, 0xd0, 0xb3, // Property
            0x44, 0xd0, 0xb4, // CountTest1
            0xd0, 0x96, 0x20, 0x20, 0x20, // CountTest2
            0x22, 0x04, 0x35, // CountTest3
            0x20, 0xaa, 0x8e, // ReverseTest1
            0x8b, 0xd1, 0x93, 0xd0, // ReverseTest2
            0xcf, 0xf0, 0xe8, 0xe2, 0xe5, 0xf2, 0x21 // Infinite
        }, 1, true)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x61, 0x62
        }, 2, false)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x61, 0x62, 0x3f, 0x3f, 0x3f, // Field
            0x41, 0x62, 0xd0
        }, 3, false)]
        [InlineData(new byte[0], 4, false)]
        [InlineData(null, 5, false)]
        public void Read(byte[] bytes, byte variant, bool reverse)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, reverse);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal("ab???", test.Field);
                    Assert.Equal("AbГг", test.Property);
                    Assert.Equal("Dд", test.CountTest1);
                    Assert.Equal("Ж   ", test.CountTest2);
                    Assert.Equal("Т\xfffd", test.CountTest3);
                    Assert.Equal("Ок ", test.ReverseTest1);
                    Assert.Equal("Гы", test.ReverseTest2);
                    Assert.Equal("Привет!", test.Infinite);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal("ab", test.Field);
                    Assert.Equal(string.Empty, test.Property);
                    Assert.Equal(string.Empty, test.CountTest1);
                    Assert.Equal(string.Empty, test.CountTest2);
                    Assert.Equal(string.Empty, test.CountTest3);
                    Assert.Equal(string.Empty, test.ReverseTest1);
                    Assert.Equal(string.Empty, test.ReverseTest2);
                    Assert.Equal(string.Empty, test.Infinite);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal("ab???", test.Field);
                    Assert.Equal("Ab\xfffd", test.Property);
                    Assert.Equal(string.Empty, test.CountTest1);
                    Assert.Equal(string.Empty, test.CountTest2);
                    Assert.Equal(string.Empty, test.CountTest3);
                    Assert.Equal(string.Empty, test.ReverseTest1);
                    Assert.Equal(string.Empty, test.ReverseTest2);
                    Assert.Equal(string.Empty, test.Infinite);
                    break;
                case 4:
                    Assert.Equal(string.Empty, test.Field);
                    Assert.Equal(string.Empty, test.Property);
                    Assert.Equal(string.Empty, test.CountTest1);
                    Assert.Equal(string.Empty, test.CountTest2);
                    Assert.Equal(string.Empty, test.CountTest3);
                    Assert.Equal(string.Empty, test.ReverseTest1);
                    Assert.Equal(string.Empty, test.ReverseTest2);
                    Assert.Equal(string.Empty, test.Infinite);
                    break;
                default:
                    Assert.Null(test.Field);
                    Assert.Null(test.Property);
                    Assert.Null(test.CountTest1);
                    Assert.Null(test.CountTest2);
                    Assert.Null(test.CountTest3);
                    Assert.Null(test.ReverseTest1);
                    Assert.Null(test.ReverseTest2);
                    Assert.Null(test.Infinite);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = Enumerable.Repeat<byte>(0x20, 29).ToArray();
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Write(bool reverse)
        {
            Test test = new Test { Field = "abвгд", Property = "AbГг", CountTest1 = "Dд", CountTest2 = "Ж", CountTest3 = "Тест", ReverseTest1 = "Ок", ReverseTest2 = "Гы", Infinite = "Привет!" };
            byte[] expected =
            {
                0x61, 0x62, 0x3f, 0x3f, 0x3f, // Field
                0x41, 0x62, 0xd0, 0x93, 0xd0, 0xb3, // Property
                0x44, 0xd0, 0xb4, // CountTest1
                0xd0, 0x96, 0x20, 0x20, 0x20, // CountTest2
                0x22, 0x04, 0x35, // CountTest3
                0x20, 0xaa, 0x8e, // ReverseTest1
                0x8b, 0xd1, 0x93, 0xd0, // ReverseTest2
                0xcf, 0xf0, 0xe8, 0xe2, 0xe5, 0xf2, 0x21 // Infinite
            };
            byte[] bytes = BinaryConverter.ToBytes(test, reverse);
            Assert.Equal(expected, bytes);
        }
    }
}