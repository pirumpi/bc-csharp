using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class BitStringTest
        : SimpleTest
    {
        private void DoTestZeroLengthStrings()
        {
            // basic construction
            DerBitString s1 = new DerBitString(new byte[0], 0);

            s1.GetBytes();

            if (!Arrays.AreEqual(s1.GetEncoded(), Hex.Decode("030100")))
            {
                Fail("zero encoding wrong");
            }

            try
            {
                new DerBitString(null, 1);
                Fail("exception not thrown");
            }
            catch (ArgumentNullException e)
            {
                //if (!"data cannot be null".Equals(e.Message))
                //{
                //    Fail("Unexpected exception");
                //}
            }

            try
            {
                new DerBitString(new byte[0], 1);
                Fail("exception not thrown");
            }
            catch (ArgumentException e)
            {
                //if (!"zero length data with non-zero pad bits".Equals(e.Message))
                //{
                //    Fail("Unexpected exception");
                //}
            }

            try
            {
                new DerBitString(new byte[1], 8);
                Fail("exception not thrown");
            }
            catch (ArgumentException e)
            {
                //if (!"pad bits cannot be greater than 7 or less than 0".Equals(e.Message))
                //{
                //    Fail("Unexpected exception");
                //}
            }

            DerBitString s2 = new DerBitString(0);
            if (!Arrays.AreEqual(s1.GetEncoded(), s2.GetEncoded()))
            {
                Fail("zero encoding wrong");
            }
        }

        private void DoTestRandomPadBits()
        {
            byte[] test = Hex.Decode("030206c0");

            byte[] test1 = Hex.Decode("030206f0");
            byte[] test2 = Hex.Decode("030206c1");
            byte[] test3 = Hex.Decode("030206c7");
            byte[] test4 = Hex.Decode("030206d1");

            EncodingCheck(test, test1);
            EncodingCheck(test, test2);
            EncodingCheck(test, test3);
            EncodingCheck(test, test4);
        }

        private void EncodingCheck(byte[] derData, byte[] dlData)
        {
            if (Arrays.AreEqual(derData, Asn1Object.FromByteArray(dlData).GetEncoded()))
            {
                //Fail("failed DL check");
                Fail("failed BER check");
            }
            if (!Arrays.AreEqual(derData, Asn1Object.FromByteArray(dlData).GetDerEncoded()))
            {
                Fail("failed DER check");
            }
        }

        public override void PerformTest()
        {
            KeyUsage k = new KeyUsage(KeyUsage.DigitalSignature);
            if ((k.GetBytes()[0] != (byte)KeyUsage.DigitalSignature) || (k.PadBits != 7))
            {
                Fail("failed digitalSignature");
            }

            k = new KeyUsage(KeyUsage.NonRepudiation);
            if ((k.GetBytes()[0] != (byte)KeyUsage.NonRepudiation) || (k.PadBits != 6))
            {
                Fail("failed nonRepudiation");
            }

            k = new KeyUsage(KeyUsage.KeyEncipherment);
            if ((k.GetBytes()[0] != (byte)KeyUsage.KeyEncipherment) || (k.PadBits != 5))
            {
                Fail("failed keyEncipherment");
            }

            k = new KeyUsage(KeyUsage.CrlSign);
            if ((k.GetBytes()[0] != (byte)KeyUsage.CrlSign)  || (k.PadBits != 1))
            {
                Fail("failed cRLSign");
            }

            k = new KeyUsage(KeyUsage.DecipherOnly);
            if ((k.GetBytes()[1] != (byte)(KeyUsage.DecipherOnly >> 8))  || (k.PadBits != 7))
            {
                Fail("failed decipherOnly");
            }

			// test for zero length bit string
			try
			{
				Asn1Object.FromByteArray(new DerBitString(new byte[0], 0).GetEncoded());
			}
			catch (IOException e)
			{
				Fail(e.ToString());
			}

            DoTestRandomPadBits();
            DoTestZeroLengthStrings();
        }

        public override string Name
        {
			get { return "BitString"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new BitStringTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
