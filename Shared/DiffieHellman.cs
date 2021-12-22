using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DiffieHellman
    {
        private const string P = "121999492637070040497464880653482451122159715698431661862504934268987469885677710797799523307422120568454593141727668682332216679465216347609718241998150443969871262326615939878834844507147192404574401325870276945218845272041195113380201145626974399759092850500988371156171063899568397919181947787377580179491"; //23
        // G is a 160 bit prime 
        private const string G = "5322540848240629191790738444031509035347191367506640093778351221586257526250970342938004178185797187285021641998886132756315601193005759267241157022234467562219"; //7

        private const int coeffLength = 125;

        private BigInteger myCoeff;

        public DiffieHellman()
        {
            this.myCoeff = this.RandomKey();
        }

        public DiffieHellman(BigInteger myCoeff)
        {
            this.myCoeff = myCoeff;
        }

        public BigInteger GetMyPublic()
        {
            var p = BigInteger.Parse(P);
            var g = BigInteger.Parse(G);

            return BigInteger.ModPow(g, this.myCoeff, p);
        }
        
        /* calculates a DH value using hardcoded P, chosen secret m, and shared DH value: [(G^n)^m mod P] */
        public byte[] getFinalKey(BigInteger otherPublic)
        {
            var p = BigInteger.Parse(P);

            var finaldh = BigInteger.ModPow(otherPublic, myCoeff, p);
            return getHashSha256(finaldh.ToByteArray());
        }
        private BigInteger RandomKey()
        {
            var rand = new Random();
            var buff = new byte[coeffLength];
            rand.NextBytes(buff);

            return BigInteger.Abs(new BigInteger(buff));
        }

        // hashes shared password with SHA-256 to get Kab
        private byte[] getHashSha256(byte[] input)
        {
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(input);
            return hash;
        }
    }
}
