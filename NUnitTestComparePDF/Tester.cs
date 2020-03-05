using System;
using System.IO;
using System.Reflection;
using ComparePDF;
using NUnit.Framework;

namespace ComparePDFTester
{
    class Tester
    {
        private static string textResultPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.txt");
        private static string imageResultPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.zip");

        [Test]
        public void TestSamePdf()
        {
            PDFComparer pDFComparer = new PDFComparer("Test page 1.pdf", "Test page 2.pdf", new ConcreteLogger());
            Assert.IsTrue(pDFComparer.ComparePDFText(textResultPath),"Text In files are the same");
            Assert.IsTrue(pDFComparer.ComparePDFImages(imageResultPath), "Images in file are the same");
        }

        [Test]
        public void TestIgnoreRegex()
        {
            PDFComparer pDFComparer = new PDFComparer("Test page 1.pdf", "Test page 2.pdf", new ConcreteLogger());
            Assert.IsTrue(pDFComparer.ComparePDFText(textResultPath, ("[a-zA-Z0-9]","")), "Text In files are the same with the given regex");
            Assert.IsTrue(pDFComparer.ComparePDFImages(imageResultPath), "Images in file are the same");
        }

        [Test]
        public void TestDifferentImagesPDF()
        {
            PDFComparer pDFComparer = new PDFComparer("Test page 1.pdf", "Test page no pic.pdf", new ConcreteLogger());
            Assert.IsTrue(pDFComparer.ComparePDFText(textResultPath), "Text In files are the same");
            Assert.IsFalse(pDFComparer.ComparePDFImages(imageResultPath), "Images in file are not the same");
        }

        [Test]
        public void TestDifferentTextPDF()
        {
            PDFComparer pDFComparer = new PDFComparer("Test page 1.pdf", "Test page wrong text.pdf", new ConcreteLogger());
            Assert.IsFalse(pDFComparer.ComparePDFText(textResultPath), "Text In files are not the same");
            Assert.IsTrue(pDFComparer.ComparePDFImages(imageResultPath), "Images in file are the same");
        }
    }
}
