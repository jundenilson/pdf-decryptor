using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;

namespace PdfDecryptor {
    public class Program {

        public static int Main(string[] args) {
            if (!ValidateArgs(args)) return -1;

            try {
                var encryptedFile = args[0];
                var decryptedFile = args[1];

                var passwords = args.ToList();

                passwords.RemoveRange(0, 2);

                if (passwords.Count == 0) passwords = new List<string> { "" };

                var remaingTries = passwords.Count;

                while (remaingTries > 0)
                    try { if(TryDecrypt(encryptedFile, decryptedFile, passwords[--remaingTries])) break; }
                    catch (BadPasswordException ex) {
                        if (remaingTries > 0) continue;
                        Console.Error.WriteLine($"Password is required for {encryptedFile}. Provided dont match: {string.Join("-", passwords)}.");
                        return -1;
                    }
                
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return -2;
            }
            return 0;
        }

        private static bool ValidateArgs(string[] args) {
            if (args.Length>= 2) return true;

            Console.WriteLine("Usage: PdfDecryptor.exe \"encrypted_input.pdf\" \"decrypted_output.pdf\" \"password1\" \"password2\" ... ");
            return false;
        }

        private static bool TryDecrypt(string encryptedPath, string decryptedFilePath, string password) {
            using (var writer = new PdfWriter(decryptedFilePath)) {
                var reader = new PdfReader(encryptedPath,
                    new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(password)));
                var doc = new PdfDocument(reader, writer);
                doc.Close();
                reader.Close();
            }
            return true;
        }
    }
}
