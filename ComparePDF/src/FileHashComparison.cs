// <copyright file="FileHashComparison.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ComparePDF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Class to compare files through hashing.
    /// </summary>
    internal static class FileHashComparison
    {
        /// <summary>
        /// Compares two files by hashing both files using the provided HashAlgorithm.
        /// </summary>
        /// <param name="filePath1"> File path to the first file to check.</param>
        /// <param name="filePath2"> File path to the second file to check.</param>
        /// <param name="algorithmName">The hashing algorithm name to use.</param>
        /// <returns><code>true</code> if both files return the same hash.</returns>
        public static bool CompareFiles(string filePath1, string filePath2, HashAlgorithmName algorithmName)
        {
            string file1Hash = GetFileHash(filePath1, algorithmName);
            string file2hash = GetFileHash(filePath2, algorithmName);
            return file1Hash.Equals(file2hash);
        }

        /// <summary>
        /// Gets the file hash based on the provided hashing algorithm name.
        /// </summary>
        /// <param name="filePath"> FIle path to the file to hash.</param>
        /// <param name="algorithmName">The hashing algorithm name to use.</param>
        /// <returns>Base64 encoded string of the hash.</returns>
        public static string GetFileHash(string filePath, HashAlgorithmName algorithmName)
        {
            using (var algo = HashAlgorithm.Create(algorithmName.Name))
            {
                using (var stream = File.OpenRead(filePath))
                {
                    string hash = Convert.ToBase64String(algo.ComputeHash(stream));
                    return hash;
                }
            }
        }
    }
}
