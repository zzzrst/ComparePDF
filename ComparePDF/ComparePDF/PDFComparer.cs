// <copyright file="PDFComparer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ComparePDF
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using TextInteractor;

    /// <summary>
    /// Compares two different PDFs.
    /// </summary>
    public class PDFComparer
    {
        /// <summary>
        /// Compares if two pdfs are the same.
        /// </summary>
        /// <param name="pdfPath1">Path of the first file.</param>
        /// <param name="pdfPath2">Path of the second file.</param>
        /// <param name="args">Regex to replace, written as REGEX];[REPLACE.</param>
        /// <returns>true if pdfs are the same.</returns>
        public static bool ComparePDF(string pdfPath1, string pdfPath2, string args)
        {
            // return false if the text is not the same
            if (!ComparePDFText(pdfPath1, pdfPath2, $@"{args}"))
            {
                return false;
            }

            if (!ComparePDFImages(pdfPath1, pdfPath2))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Main program.
        /// </summary>
        /// <param name="args">arguments that are passed in.</param>
        internal static void Main(string[] args)
        {

        }

        private static bool ComparePDFImages(string pdfPath1, string pdfPath2)
        {
            // deletes the old directory first
            string tempLocation1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\pdf1";
            string tempLocation2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\pdf2";

            if (Directory.Exists(tempLocation1))
            {
                Directory.Delete(tempLocation1, true);
            }

            if (Directory.Exists(tempLocation2))
            {
                Directory.Delete(tempLocation2, true);
            }

            // Extracts the images
            RunPdfToHtml(pdfPath1, "pdf1");
            RunPdfToHtml(pdfPath2, "pdf2");

            // if pages are not the same, return false
            int pageCount1 = Directory.GetFiles(tempLocation1, "*.*", SearchOption.AllDirectories)
                 .Where(file => new string[] { ".jpg", ".gif", ".png" }
                 .Contains(Path.GetExtension(file)))
                 .ToList().Count;

            int pageCount2 = Directory.GetFiles(tempLocation2, "*.*", SearchOption.AllDirectories)
                 .Where(file => new string[] { ".jpg", ".gif", ".png" }
                 .Contains(Path.GetExtension(file)))
                 .ToList().Count;

            if (pageCount1 != pageCount2)
            {
                return false;
            }

            for (int i = 1; i <= pageCount1; i++)
            {
                if (File.Exists(tempLocation1 + $"\\page{i}.png") && File.Exists(tempLocation2 + $"\\page{i}.png"))
                {
                    if (!CompareImages(tempLocation1 + $"\\page{i}.png", tempLocation2 + $"\\page{i}.png") == true)
                    {
                        // there is a difference in the images.
                        return false;
                    }
                }
                else
                {
                    // one of the files do not exists means one of the images is not in one of the pdfs.
                    return false;
                }
            }

            return true;
        }

        private static bool ComparePDFText(string pdfPath1, string pdfPath2, string args)
        {
            try
            {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe",
                    Arguments = $"/C exit | \"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\converter.bat\" \"{pdfPath1}\" \"{pdfPath2}\"",
                };
                p.StartInfo = startInfo;
                p.Start();
                string line;
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                }

                p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            string expectedFilePath = "C:\\TEMP\\expected.txt";
            string actualFilePath = "C:\\TEMP\\actual.txt";

            bool result = false;

            TextInteractor fileExpected = new TextInteractor(expectedFilePath);
            TextInteractor fileActual = new TextInteractor(actualFilePath);

            if (fileExpected.Open() & fileActual.Open())
            {
                // remove the regex substrings.
                fileExpected.Modify(3, args);
                fileActual.Modify(3, args);

                result = fileExpected.Compare(fileActual, ignoreWhitespace: true);

                fileExpected.Close();
                fileActual.Close();
            }

            return result;
        }

        private static bool RunPdfToHtml(string pdfPath, string htmlname)
        {
            bool pass = false;
            try
            {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\pdftohtml.exe",
                    Arguments = $" -q \"{pdfPath}\" \"{htmlname}\"",
                };
                p.StartInfo = startInfo;
                p.Start();
                p.WaitForExit();
                int exitCode = p.ExitCode;

                if (exitCode != 0)
                {
                    throw new Exception("something went wrong :(");
                }

                pass = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return pass;
        }

        private static bool CompareImages(string imagePath1, string imagePath2)
        {
            string hash1, hash2;
            hash1 = GetImageHash(imagePath1);
            hash2 = GetImageHash(imagePath2);

            return hash1.Equals(hash2);
        }

        private static string GetImageHash(string imagePath)
        {
            Image img = Image.FromFile(imagePath);

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(imagePath))
                {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }
    }
}
