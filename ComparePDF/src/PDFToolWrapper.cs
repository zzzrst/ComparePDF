// <copyright file="PDFToolWrapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ComparePDF
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Class to represent the wrappers for all the PDFTools.
    /// </summary>
    internal static class PDFToolWrapper
    {
        /// <summary>
        /// Runs PDF to text with the provided arguments of -nopgbrk and -table.
        /// </summary>
        /// <param name="pdfPath">Path of the pdf file to convert to text.</param>
        /// <param name="resultFileName">Name of the text file.</param>
        /// <param name="logger">Logger to be used.</param>
        public static void RunPDFToText(string pdfPath, string resultFileName, ILogger logger)
        {
            string pdfToolPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "xpdf", "pdftotext.exe");
            string arguments = $"-table -nopgbrk \"{pdfPath}\" \"{resultFileName}\"";
            RunPDFToolWrapper(pdfToolPath, arguments, logger);
        }

        /// <summary>
        /// Runs PDF to image with the provided arguments of pdfPath and resultFolder.
        /// </summary>
        /// <param name="pdfPath">Path of the pdf file to convert to text.</param>
        /// <param name="resultFolder">Folder to place images.</param>
        /// <param name="logger">Logger to be used.</param>
        public static void RunPDFtoImage(string pdfPath, string resultFolder, ILogger logger)
        {
            string pdfToolPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "poppler", "pdfimages.exe");
            string arguments = $"-all \"{pdfPath}\" \"{resultFolder}/images\"";
            RunPDFToolWrapper(pdfToolPath, arguments, logger);
        }

        /// <summary>
        /// Runs PDF to image with the provided arguments of pdfPath and resultFolder.
        /// </summary>
        /// <param name="pdfPath">Path of the pdf file to convert to text.</param>
        /// <param name="resultFolder">Folder to place images.</param>
        /// <param name="logger">Logger to be used.</param>
        public static void RunPDFDetach(string pdfPath, string resultFolder, ILogger logger)
        {
            string pdfToolPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "poppler", "pdfdetach.exe");
            string arguments = $"\"{pdfPath}\" -o \"{resultFolder}/embeddedFile\"";
            RunPDFToolWrapper(pdfToolPath, arguments, logger);
        }

        /// <summary>
        /// Wrapper for running the different PDFTools included.
        /// </summary>
        /// <param name="pdfToolPath">The path to the PDFTool.</param>
        /// <param name="arguments">The arguements to be passed to the PDFTool.</param>
        /// <param name="logger">The logger used for the standard output and error from the tool.</param>
        private static void RunPDFToolWrapper(string pdfToolPath, string arguments, ILogger logger)
        {
            try
            {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = pdfToolPath,
                    Arguments = arguments,
                };

                p.StartInfo = startInfo;
                p.Start();

                string line;
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                    logger.LogDebug(line);
                }

                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    string nameOfTool = Path.GetFileName(pdfToolPath);
                    logger.LogError($"Something went wrong while trying to call:{nameOfTool} {arguments}");
                    logger.LogError(p.StandardError.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                throw;
            }
        }
    }
}
