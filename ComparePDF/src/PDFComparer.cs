// <copyright file="PDFComparer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ComparePDF
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using Microsoft.Extensions.Logging;
    using TextInteractor;

    /// <summary>
    /// Compares two different PDFs.
    /// </summary>
    public class PDFComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFComparer"/> class.
        /// </summary>
        /// <param name="pdfFilePath1">Sets the file path to the first PDF.</param>
        /// <param name="pdfFilePath2">Sets the file path to the second PDF.</param>
        public PDFComparer(string pdfFilePath1, string pdfFilePath2)
            : this(pdfFilePath1, pdfFilePath2, logger: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFComparer"/> class.
        /// </summary>
        /// <param name="pdfFilePath1">Sets the file path to the first PDF.</param>
        /// <param name="pdfFilePath2">Sets the file path to the second PDF.</param>
        /// <param name="logger">The logger to use throughout the class.</param>
        public PDFComparer(string pdfFilePath1, string pdfFilePath2, ILogger logger)
        {
            this.PDFFilePath1 = pdfFilePath1;
            this.PDFFilePath2 = pdfFilePath2;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets or sets the HashAlgorithmName to be used to compare extracted files.
        /// Default is set to MD5 hash.
        /// </summary>
        public HashAlgorithmName Hash { get; set; } = HashAlgorithmName.MD5;

        /// <summary>
        /// Gets or sets file path to the first PDF.
        /// </summary>
        private string PDFFilePath1 { get; set; }

        /// <summary>
        /// Gets or sets file path to the second PDF.
        /// </summary>
        private string PDFFilePath2 { get; set; }

        /// <summary>
        /// Gets or sets the logger to be used throughout the class.
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Compares two PDFS by the set hash algorithm name.
        /// </summary>
        /// <returns>True if the hash are the same.</returns>
        public bool ComparePDFByHash()
        {
            return FileHashComparison.CompareFiles(this.PDFFilePath1, this.PDFFilePath2, this.Hash);
        }

        ///// <summary>
        ///// We extract the text from the PDFs provided. We compare the text based on the arguements provided. We ignore whitespaces by default.
        ///// </summary>
        ///// <param name="resultFilePath">Name of the file for PDF text comparison.</param>
        ///// <param name="regex">Regex to be replaced.</param>
        ///// <param name="replacement">Replacement value for the regex.</param>
        ///// <param name="caseInsensitive">Toggle when comparing extracted text. Default to be false.</param>
        ///// <param name="ignoreWhitespace">Toggle when comparing extracted text. Default to be true.</param>
        ///// <returns><code>true</code> if the PDF texts are the same. </returns>
        //public bool ComparePDFText(string resultFilePath, string regex = default, string replacement = default, bool caseInsensitive = false, bool ignoreWhitespace = true)
        //{
        //    if ((regex == default && replacement != default) || (regex != default && replacement == default))
        //    {
        //        throw new ArgumentException("Both regex and replacement have to be filled out at the same time.");
        //    }

        //    return this.ComparePDFText(resultFilePath, (regex, replacement), caseInsensitive, ignoreWhitespace);
        //}

        /// <summary>
        /// We extract the text from the PDFs provided. We compare the text based on the arguements provided. We ignore whitespaces by default.
        /// </summary>
        /// <param name="resultFilePath">Name of the file for PDF text comparison.</param>
        /// <param name="regexReplacement">(REGEX, REPLACEMENT) -> A 2-Tuple string for the regex and replacement.</param>
        /// <param name="caseInsensitive">Toggle when comparing extracted text. Default to be false.</param>
        /// <param name="ignoreWhitespace">Toggle when comparing extracted text. Default to be true.</param>
        /// <returns><code>true</code> if the PDF texts are the same. </returns>
        public bool ComparePDFText(string resultFilePath, (string regex, string replacement) regexReplacement = default, bool caseInsensitive = false, bool ignoreWhitespace = true)
        {
            // Convert both pdfs into text
            string pdf1FileName = Path.GetTempFileName();
            string pdf2FileName = Path.GetTempFileName();

            PDFToolWrapper.RunPDFToText(this.PDFFilePath1, pdf1FileName, this.Logger);
            PDFToolWrapper.RunPDFToText(this.PDFFilePath2, pdf2FileName, this.Logger);

            TextFile pdfText1 = new TextInteractor(pdf1FileName, this.Logger);
            TextFile pdfText2 = new TextInteractor(pdf2FileName, this.Logger);

            if (regexReplacement != default)
            {
                pdfText1.ReplaceOccurances(toReplace: regexReplacement.regex, replaceWith: regexReplacement.replacement);
                pdfText2.ReplaceOccurances(toReplace: regexReplacement.regex, replaceWith: regexReplacement.replacement);
            }

            return pdfText1.Compare(pdfText2, resultFilePath, ignoreWhitespace, caseInsensitive);
        }

        /// <summary>
        /// Compares the embedded files inside the PDF through extraction and hashing.
        /// </summary>
        /// <param name="resultZipFile">Zip file that contains the extracted files.</param>
        /// <returns><c>True</c> if the embedded files hash matches.</returns>
        public bool ComparePDFEmbeddedFiles(string resultZipFile)
        {
            // We create a folder for PDF file 1, and PDF file 2
            string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string pdfFile1Folder = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(this.PDFFilePath1));
            string pdfFile2Folder = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(this.PDFFilePath2));
            Directory.CreateDirectory(pdfFile1Folder);
            Directory.CreateDirectory(pdfFile2Folder);

            // Extract Images
            PDFToolWrapper.RunPDFDetach(this.PDFFilePath1, pdfFile1Folder, this.Logger);
            PDFToolWrapper.RunPDFDetach(this.PDFFilePath2, pdfFile2Folder, this.Logger);
            ZipFile.CreateFromDirectory(tempFolder, resultZipFile);

            return this.CompareDirectory(pdfFile1Folder, pdfFile2Folder);
        }

        /// <summary>
        /// Compares the images inside the PDF through extraction and hashing.
        /// </summary>
        /// <param name="resultZipFile">The zip file representing the images extracted.</param>
        /// <returns><c>True</c> if the embedded files hash matches.</returns>
        public bool ComparePDFImages(string resultZipFile)
        {
            // We create a folder for PDF file 1, and PDF file 2
            string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string pdfFile1Folder = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(this.PDFFilePath1));
            string pdfFile2Folder = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(this.PDFFilePath2));

            this.DeleteDirectory(pdfFile1Folder);
            this.DeleteDirectory(pdfFile2Folder);
            this.DeleteFile(resultZipFile);

            Directory.CreateDirectory(pdfFile1Folder);
            Directory.CreateDirectory(pdfFile2Folder);

            // Extract Images
            PDFToolWrapper.RunPDFtoImage(this.PDFFilePath1, pdfFile1Folder, this.Logger);
            PDFToolWrapper.RunPDFtoImage(this.PDFFilePath2, pdfFile2Folder, this.Logger);
            ZipFile.CreateFromDirectory(tempFolder, resultZipFile);

            return this.CompareDirectory(pdfFile1Folder, pdfFile2Folder);
        }

        private void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);

            }
        }

        private void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        private bool CompareDirectory(string directory1, string directory2)
        {
            // We check if the image produced from each is the same.
            string[] directory1FileList = Directory.GetFiles(directory1);
            string[] directroy2FileList = Directory.GetFiles(directory2);

            bool isFileCountSame = directory1FileList.Length == directroy2FileList.Length;
            if (!isFileCountSame)
            {
                this.Logger.LogError("The number of files are not the same.");
                return isFileCountSame;
            }

            bool areSameFiles = true;
            foreach (string fileFullPath in directory1FileList)
            {
                string fileName = Path.GetFileName(fileFullPath);
                string directory1FilePath = Path.Combine(directory1, fileName);
                string directroy2FilePath = Path.Combine(directory2, fileName);
                bool isSameFile = FileHashComparison.CompareFiles(directory1FilePath, directroy2FilePath, this.Hash);

                if (!isSameFile)
                {
                    this.Logger.LogError($"{fileName} were not the same for both PDFs");
                    areSameFiles &= isSameFile;
                }
            }

            return areSameFiles;
        }
    }
}
