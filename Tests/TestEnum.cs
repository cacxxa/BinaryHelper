using System;
using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestEnum
    {
        private enum Enum1
        {
            A,
            B
        }

        private enum Enum2
        {
            A = 5,
            B = 10
        }

        private enum Enum3
        {
            A = 10000,
            B
        }

        private enum Enum4
        {
            A = 1000000,
            B
        }

        private class Test
        {
            [Binary]
            public Enum1 Field;

            [Binary(Count = 1)]
            public Enum2 Property { get; set; }

            [Binary(Count = 1)]
            public Enum2 Count1 { get; set; }

            [Binary(Count = 2)]
            public Enum3 Count2 { get; set; }

            [Binary(Count = 4)]
            public Enum4 Count3 { get; set; }

            [Binary(Count = 4, Reverse = true)]
            public Enum4 Reverse { get; set; }

            [Binary(Count = 1)]
            public Enum3 IncorrectCount { get; set; }
        }

        private class Test2
        {
            [Binary(Count = 3)]
            public Enum1 Error;
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x00, // Field
            0x05, // Property
            0x0a, // Count 1
            0x11, 0x27, // Count 2
            0x41, 0x42, 0x0f, 0x00, // Count 3
            0x00, 0x0f, 0x42, 0x40, // Reverse
            0x10 // IncorrectCount
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов
        {
            0x00, 0x05, 0x0a, 0x11
        }, 2)]
        [InlineData(new byte[0], 3)]
        [InlineData(null, 4)]
        public void Read(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(Enum1.A, test.Field);
                    Assert.Equal(Enum2.A, test.Property);
                    Assert.Equal(Enum2.B, test.Count1);
                    Assert.Equal(Enum3.B, test.Count2);
                    Assert.Equal(Enum4.B, test.Count3);
                    Assert.Equal(Enum4.A, test.Reverse);
                    Assert.Equal(0, (int)test.IncorrectCount);
                    break;
                case 2: // недостаточно байтов
                    Assert.Equal(Enum1.A, test.Field);
                    Assert.Equal(Enum2.A, test.Property);
                    Assert.Equal(Enum2.B, test.Count1);
                    Assert.Equal(0, (int)test.Count2);
                    Assert.Equal(0, (int)test.Count3);
                    Assert.Equal(0, (int)test.Reverse);
                    Assert.Equal(0, (int)test.IncorrectCount);
                    break;
                default:
                    Assert.Equal(Enum1.A, test.Field);
                    Assert.Equal(0, (int)test.Property);
                    Assert.Equal(0, (int)test.Count1);
                    Assert.Equal(0, (int)test.Count2);
                    Assert.Equal(0, (int)test.Count3);
                    Assert.Equal(0, (int)test.Reverse);
                    Assert.Equal(0, (int)test.IncorrectCount);
                    break;
            }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x00, // Field
            0x05, // Property
            0x0a, // Count 1
            0x27, 0x11, // Count 2
            0x00, 0x0f, 0x42, 0x41, // Count 3
            0x40, 0x42, 0x0f, 0x00, // Reverse
            0x10 // IncorrectCountst3
        }, 1)]
        [InlineData(new byte[] // недостаточно байтов
        {
            0x00, 0x05, 0x0a, 0x11
        }, 2)]
        [InlineData(new byte[0], 3)]
        [InlineData(null, 4)]
        public void ReadReverse(byte[] bytes, byte variant)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, true);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(Enum1.A, test.Field);
                    Assert.Equal(Enum2.A, test.Property);
                    Assert.Equal(Enum2.B, test.Count1);
                    Assert.Equal(Enum3.B, test.Count2);
                    Assert.Equal(Enum4.B, test.Count3);
                    Assert.Equal(Enum4.A, test.Reverse);
                    Assert.Equal(0, (int)test.IncorrectCount);
                    break;
                case 2: // недостаточно байтов
                    Assert.Equal(Enum1.A, test.Field);
                    Assert.Equal(Enum2.A, test.Property);
                    Assert.Equal(Enum2.B, test.Count1);
                    Assert.Equal(0, (int)test.Count2);
                    Assert.Equal(0, (int)test.Count3);
                    Assert.Equal(0, (int)test.IncorrectCount);
                    break;               
                default:
                    Assert.Equal(Enum1.A, test.Field);
                    Assert.Equal(0, (int)test.Property);
                    Assert.Equal(0, (int)test.Count1);
                    Assert.Equal(0, (int)test.Count2);
                    Assert.Equal(0, (int)test.Count3);
                    Assert.Equal(0, (int)test.IncorrectCount);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = Enumerable.Repeat<byte>(0, 14).ToArray();
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Write()
        {
            Test test = new Test {Field = Enum1.A, Property = Enum2.A, Count1 = Enum2.B, Count2 = Enum3.B, Count3 = Enum4.B, Reverse = Enum4.A, IncorrectCount = Enum3.A};
            byte[] expected =
            {
                0x00, // Field
                0x05, // Property
                0x0a, // Count 1
                0x11, 0x27, // Count 2
                0x41, 0x42, 0x0f, 0x00, // Count 3
                0x00, 0x0f, 0x42, 0x40, // Reverse
                0x10 // IncorrectCount
            };
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void WriteReverse()
        {
            Test test = new Test { Field = Enum1.A, Property = Enum2.A, Count1 = Enum2.B, Count2 = Enum3.B, Count3 = Enum4.B, Reverse = Enum4.A, IncorrectCount = Enum3.A };
            byte[] expected =
            {
                0x00, // Field
                0x05, // Property
                0x0a, // Count 1
                0x27, 0x11, // Count 2
                0x00, 0x0f, 0x42, 0x41, // Count 3
                0x40, 0x42, 0x0f, 0x00, // Reverse
                0x10 // IncorrectCount
            };
            byte[] bytes = BinaryConverter.ToBytes(test, true);
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void Exception()
        {
            Test2 test = new Test2 {Error = Enum1.B};
            Assert.Equal(Enum1.B, test.Error);
            Assert.Throws<ArgumentException>(() => BinaryConverter.ToBytes(test));
        }
    }
}