using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using static System.Console;
using static System.String;
using static System.Text.Encoding;

namespace PdfDecryptor {
    public class Program {
        private const string _usage = 
@"Usage: PdfDecryptor.exe encrypted_input.pdf decrypted_output_path.pdf pwd1 pwd2...
Returns True if the input was decrypted.";

        public static int Main(string[] args) {
            if (!ValidateArgs(args)) return -1;

            var encryptedFile = args[0];
            var decryptedFile = args[1];
            var passwords = args.Skip(2).ToArray();

            try {
                if (TryOpenWithoutPassword(encryptedFile)) { WriteLine(false); return 0; }

                WriteLine(true);

                foreach (var password in passwords) {
                    try { if (TryDecrypt(encryptedFile, decryptedFile, password)) return 0; } 
                    catch (BadPasswordException) { }
                }
            } catch (Exception ex) {
                Error.WriteLine(ex.Message);
                Error.WriteLine(ex.StackTrace);
                return -2;
            }

            Error.WriteLine($"Password is required for {encryptedFile}, tried: {Join("; ", passwords)}");
            return -1;
        }
        
        private static bool ValidateArgs(IReadOnlyCollection<string> args) {
            if (args.Count >= 2) return true;
            WriteLine(_usage);
            return false;
        }

        private static bool TryDecrypt(string encryptedPath, string decryptedFilePath, string password) {
            var readerProperties = new ReaderProperties().SetPassword(UTF8.GetBytes(password));
            using (var writer = new PdfWriter(decryptedFilePath))
            using (var reader = new PdfReader(encryptedPath, readerProperties))
            using (var doc = new PdfDocument(reader, writer)) {
                doc.Close();
                reader.Close();
            }
            return true;
        }

        private static bool TryOpenWithoutPassword(string nonEncryptedFile) {
            try {
                using (var reader = new PdfReader(nonEncryptedFile))
                using (var doc = new PdfDocument(reader)) {
                    doc.Close();
                    reader.Close();
                    return true;
                }
            }
            catch(Exception) { return false; }
        }
    }
}
