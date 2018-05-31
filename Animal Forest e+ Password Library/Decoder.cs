using System;
using System.Linq;

namespace PasswordLibrary.Decoder
{
    public class Decoder
    {
        public static void mMpswd_decode_bit_shuffle(ref byte[] Data, bool Unknown)
        {
            int r28 = Unknown ? 0x17 : 0x16; // Count
            int r31 = Unknown ? 0x09 : 0x0D; // Bit index

            byte TableIndex = Data[r31];
            byte[] ShuffledData = new byte[23]; // Exclude the r31 byte

            for (int i = 0, idx = 0; i < 23; i++)
            {
                if (i == r31)
                {
                    idx++; // Skip r31 byte
                }
                ShuffledData[i] = Data[idx++];
            }

            byte[] ZeroedData = new byte[23];
            int[] ShuffleTable = Common.mMpswd_select_idx_table[((Data[r31] << 2) & 0xC) >> 2]; // Shouldn't this be & 0xF?? (It's & 0xC in code)

            int OffsetIdx = 0;
            int ZeroedDataIdx = 0;
            while (OffsetIdx < r28)
            {
                int r4 = 0;
                int r9 = 0;

                for (int x = 0; x < 8; x++)
                {
                    int OutputOffset = (ShuffleTable[r4++] + OffsetIdx) % r28;
                    byte CurrentByte = ShuffledData[OutputOffset];

                    CurrentByte >>= r9;
                    CurrentByte &= 1;
                    CurrentByte <<= r9++;

                    ZeroedData[ZeroedDataIdx] |= CurrentByte;
                }

                OffsetIdx++;
                ZeroedDataIdx++;
            }

            ZeroedData.Take(r31).ToArray().CopyTo(Data, 0);
            Data[r31] = TableIndex;
            ZeroedData.Skip(r31).Take(ZeroedDataIdx - r31).ToArray().CopyTo(Data, r31 + 1);
        }

        public static void mMpswd_decode_bit_code(ref byte[] Data)
        {
            int Method = Data[1] & 0x0F;

            if (Method > 12)
            {
                Common.mMpswd_bit_shift(ref Data, -Method * 3);
                Common.mMpswd_bit_reverse(ref Data);
                Common.mMpswd_bit_arrange_reverse(ref Data);
            }
            else if (Method > 8)
            {
                Common.mMpswd_bit_shift(ref Data, Method * 5);
                Common.mMpswd_bit_arrange_reverse(ref Data);
            }
            else if (Method > 4)
            {
                Common.mMpswd_bit_reverse(ref Data);
                Common.mMpswd_bit_shift(ref Data, Method * 5);
            }
            else
            {
                Common.mMpswd_bit_arrange_reverse(ref Data);
                Common.mMpswd_bit_shift(ref Data, -Method * 3);
            }
        }

        public static void mMpswd_decode_RSA_cipher(ref byte[] Data)
        {
            int ModCount = 0;
            byte[] OutputBuffer = Data.Clone() as byte[];

            Tuple<int, int, int, int[]> PrimeData = Common.mMpswd_get_RSA_key_code(Data);

            int Prime1 = PrimeData.Item1;
            int Prime2 = PrimeData.Item2;
            int Prime3 = PrimeData.Item3;
            int[] IndexTable = PrimeData.Item4;

            int PrimeProduct = Prime1 * Prime2;
            int LessProduct = (Prime1 - 1) * (Prime2 - 1);
            int LoopEndValue = 0;
            int ModValue = 0;

            do
            {
                ModCount++;
                LoopEndValue = (ModCount * LessProduct + 1) % Prime3;
                ModValue = (ModCount * LessProduct + 1) / Prime3;
            } while (LoopEndValue != 0);

            int CurrentValue = 0;

            for (int i = 0; i < 8; i++)
            {
                int Value = Data[IndexTable[i]];
                Value |= ((Data[23] >> i) << 8) & 0x100;
                CurrentValue = Value;

                for (int x = 0; x < ModValue - 1; x++)
                {
                    Value = (Value * CurrentValue) % PrimeProduct;
                }

                OutputBuffer[IndexTable[i]] = (byte)Value;
            }

            for (int i = 0; i < 24; i++)
            {
                Data[i] = OutputBuffer[i];
            }
        }

        public static void mMpswd_decode_substitution_cipher(ref byte[] Data)
        {
            for (int i = 0; i < 24; i++)
            {
                for (int x = 0; x < 256; x++)
                {
                    if (Data[i] == Common.mMpswd_chg_code_table[x])
                    {
                        Data[i] = (byte)x;
                        break;
                    }
                }
            }
        }

        // Main Code \\
        public static byte[] Decode(byte[] Input)
        {
            if (Input.Length != 32)
            {
                throw new Exception("Input must be 32 bytes long");
            }

            byte[] PasswordData = new byte[24];

            Common.mMpswd_chg_password_font_code(ref Input);
            Common.mMpswd_chg_8bits_code(ref PasswordData, Input);
            Common.mMpswd_transposition_cipher(ref PasswordData, true, 1);
            mMpswd_decode_bit_shuffle(ref PasswordData, true);
            mMpswd_decode_bit_code(ref PasswordData);
            mMpswd_decode_RSA_cipher(ref PasswordData);
            mMpswd_decode_bit_shuffle(ref PasswordData, false);
            Common.mMpswd_transposition_cipher(ref PasswordData, false, 0);
            mMpswd_decode_substitution_cipher(ref PasswordData);

            return PasswordData;
        }

        public static byte[] Decode(string Password)
        {
            if (Password.Length == 32)
            {
                byte[] Data = new byte[32];
                for (int i = 0; i < 32; i++)
                {
                    int Idx = Array.IndexOf(Common.AFe_CharList, Password.Substring(i, 1));
                    if (Idx < 0)
                    {
                        throw new Exception("The password contains an invalid character!");
                    }
                    Data[i] = (byte)Idx;
                }
                return Decode(Data);
            }
            return null;
        }
    }
}
