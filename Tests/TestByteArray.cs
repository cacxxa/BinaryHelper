using System.Linq;
using BinaryHelper;
using Xunit;

namespace Tests
{
    public class TestByteArray
    {
        private class Test
        {
            [Binary(Count = 5)]
            public byte[] Field;

            [Binary(Count = 1)]
            public byte[] Property { get; set; }

            [Binary(Count = 3)]
            public byte[] Null { get; set; }

            [Binary(Count = 3)]
            public byte[] Empty { get; set; }

            [Binary(Reverse = true, Count = 5)]
            public byte[] Reverse1 { get; set; }

            [Binary(Reverse = true, Count = 1)]
            public byte[] Reverse2 { get; set; }

            [Binary]
            public byte[] Infinite { get; set; }
        }

        [Theory]
        [InlineData(new byte[] // корректный
        {
            0x01, 0x02, 0x03, 0x04, 0x05, // field
            0x10, // Property
            0x00, 0x00, 0x00, // Null
            0x00, 0x00, 0x00, // Empty
            0x05, 0x04, 0x03, 0x02, 0x01, // Reverse1
            0x10, // Reverse2
            0x06, 0x07, 0x08, 0x09, 0x0a // Infinite
        }, 1, false)]
        [InlineData(new byte[] // корректный, проверка реверса
        {
            0x01, 0x02, 0x03, 0x04, 0x05, // field
            0x10, // Property
            0x00, 0x00, 0x00, // Null
            0x00, 0x00, 0x00, // Empty
            0x05, 0x04, 0x03, 0x02, 0x01, // Reverse1
            0x10, // Reverse2
            0x06, 0x07, 0x08, 0x09, 0x0a // Infinite
        }, 1, true)]
        [InlineData(new byte[] // недостаточно байтов 1
        {
            0x01, 0x02, 0x03
        }, 2, false)]
        [InlineData(new byte[] // недостаточно байтов 2
        {
            0x01, 0x02, 0x03, 0x04, 0x05,
            0x10
        }, 3, false)]
        [InlineData(new byte[0], 4, false)]
        [InlineData(null, 5, false)]
        public void Read(byte[] bytes, byte variant, bool reverse)
        {
            Test test = BinaryConverter.FromBytes<Test>(bytes, reverse);
            switch (variant)
            {
                case 1: //корректный
                    Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, test.Field);
                    Assert.Equal(new byte[] { 0x10 }, test.Property);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Null);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Empty);
                    Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, test.Reverse1);
                    Assert.Equal(new byte[] { 0x10 }, test.Reverse2);
                    Assert.Equal(new byte[] { 0x06, 0x07, 0x08, 0x09, 0x0a }, test.Infinite);
                    break;
                case 2: // недостаточно байтов 1
                    Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x00, 0x00 }, test.Field);
                    Assert.Equal(new byte[] { 0x00 }, test.Property);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Null);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Empty);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, test.Reverse1);
                    Assert.Equal(new byte[] { 0x00 }, test.Reverse2);
                    Assert.Empty(test.Infinite);
                    break;
                case 3: // недостаточно байтов 2
                    Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, test.Field);
                    Assert.Equal(new byte[] { 0x10 }, test.Property);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Null);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Empty);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, test.Reverse1);
                    Assert.Equal(new byte[] { 0x00 }, test.Reverse2);
                    Assert.Empty(test.Infinite);
                    break;
                case 4:
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, test.Field);
                    Assert.Equal(new byte[] { 0x00 }, test.Property);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Null);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00 }, test.Empty);
                    Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, test.Reverse1);
                    Assert.Equal(new byte[] { 0x00 }, test.Reverse2);
                    Assert.Empty(test.Infinite);                    
                    break;
                default:
                    Assert.Null(test.Field);
                    Assert.Null(test.Property);
                    Assert.Null(test.Null);
                    Assert.Null(test.Empty);
                    Assert.Null(test.Reverse1);
                    Assert.Null(test.Reverse2);
                    Assert.Null(test.Infinite);
                    break;
            }
        }

        [Fact]
        public void WriteEmpty()
        {
            Test test = new Test();
            byte[] expected = Enumerable.Repeat<byte>(0, 18).ToArray();
            byte[] bytes = BinaryConverter.ToBytes(test);
            Assert.Equal(expected, bytes);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Write(bool reverse)
        {
            Test test = new Test
            {
                Field = new byte[] {0x01, 0x02, 0x03, 0x04, 0x05},
                Property = new byte[] { 0x10 },
                Null = null,
                Empty = new byte[0],
                Reverse1 = new byte[] {0x01, 0x02, 0x03, 0x04, 0x05},
                Reverse2 = new byte[] {0x10},
                Infinite = new byte[] {0x06, 0x07, 0x08, 0x09, 0x0a}
            };
            byte[] expected =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, // field
                0x10, // Property
                0x00, 0x00, 0x00, // Null
                0x00, 0x00, 0x00, // Empty
                0x05, 0x04, 0x03, 0x02, 0x01, // Reverse1
                0x10, // Reverse2
                0x06, 0x07, 0x08, 0x09, 0x0a // Infinite
            };
            byte[] bytes = BinaryConverter.ToBytes(test, reverse);
            Assert.Equal(expected, bytes);
        }
    }
}