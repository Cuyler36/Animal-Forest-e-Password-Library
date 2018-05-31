using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PasswordLibrary;

namespace Password_Library_Tests
{
    [TestClass]
    public class DecoderTests
    {
        [TestMethod]
        public void mMpswd_chg_password_font_code_Test()
        {
            byte[] TestPassword = new byte[32];
            for (int i = 0; i < TestPassword.Length; i++)
            {
                TestPassword[i] = 0x0C;
            }

            Common.mMpswd_chg_password_font_code(ref TestPassword);
            for (int i = 0; i < TestPassword.Length; i++)
            {
                Assert.AreEqual(TestPassword[i], 0x2B);
            }

            /*Decoder.mMpswd_decode_bit_shuffle(ref TestPassword, true);
            for (int i = 0; i < ShuffledData.Length; i++)
            {
                Console.Write(ShuffledData[i].ToString("X2") + " ");
                if (i % 8 == 0)
                {
                    Console.Write("\r\n");
                }
            }*/
            Console.WriteLine("Done");
        }
    }
}
