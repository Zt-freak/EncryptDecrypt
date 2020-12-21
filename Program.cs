using System;
using System.IO;
using System.Security.Cryptography;

namespace EncryptDecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EncryptDecrypt >");

            EncryptorDecryptor ed = new EncryptorDecryptor();
            ed.GetInput();
        }
    }

    class EncryptorDecryptor
    {
        private readonly byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        private void Encrypt(string fileName)
        {
            try
            {
                string text = System.IO.File.ReadAllText(fileName);
                string encryptFileName = fileName+".ende";
                using FileStream myStream = new FileStream(encryptFileName, FileMode.OpenOrCreate);

                using Aes aes = Aes.Create();
                aes.Key = this.key;

                byte[] iv = aes.IV;
                myStream.Write(iv, 0, iv.Length);

                using CryptoStream cryptStream = new CryptoStream(
                    myStream,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write);

                using StreamWriter sWriter = new StreamWriter(cryptStream);
  
                sWriter.WriteLine(text);

                Console.WriteLine("File encrypted");
            }
            catch
            {
                Console.WriteLine("Encryption failed");
                throw;
            }
        }
        private void Decrypt(string fileName)
        {
            try
            {
                string decryptFileName = "";
                if (!fileName.EndsWith(".ende"))
                {
                    decryptFileName = fileName;
                    fileName += ".ende";
                }
                else
                {
                    decryptFileName = fileName.Substring(0, fileName.Length - 4);
                }
                Console.WriteLine(decryptFileName);

                using FileStream myStream = new FileStream(fileName, FileMode.Open);
                using FileStream decryptStream = new FileStream(decryptFileName, FileMode.OpenOrCreate);

                using Aes aes = Aes.Create();

                byte[] iv = new byte[aes.IV.Length];
                myStream.Read(iv, 0, iv.Length);

                using CryptoStream cryptStream = new CryptoStream(
                   myStream,
                   aes.CreateDecryptor(this.key, iv),
                   CryptoStreamMode.Read);

                using StreamReader sReader = new StreamReader(cryptStream);
                using StreamWriter sWriter = new StreamWriter(decryptStream);
                sWriter.WriteLine(sReader.ReadToEnd());

                Console.WriteLine("File decrypted");
            }
            catch
            {
                Console.WriteLine("Decryption failed.");
                throw;
            }
        }
        public void GetInput()
        {
            string input = Console.ReadLine();
            int pathNameStart = input.IndexOf('"')+1;
            int pathNameEnd = input.IndexOf('"', input.IndexOf('"') + 1);
            String pathName = "";
            if (pathNameStart != -1 && pathNameEnd != -1)
            {
                pathName = input.Substring(pathNameStart, pathNameEnd - pathNameStart);
            }

            if (input.StartsWith("encrypt") && pathName != "")
            {
                this.Encrypt(pathName);
            }
            else if (input.StartsWith("decrypt") && pathName != "")
            {
                this.Decrypt(pathName);
            }
            else if (input.StartsWith("help"))
            {
                Console.WriteLine("usage: [method] \"filename.txt\"");
                Console.WriteLine("method: encrypt/decrypt");
            }
            else
            {
                Console.WriteLine("invalid input");
            }
            this.GetInput();
        }
    }
}
