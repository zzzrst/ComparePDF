using System;
using System.IO;
using System.Reflection;
using ComparePDF;

namespace ComparePDFTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //"[a-zA-Z0-9]];[
            //string regex = "";
            bool pass = true;
            Console.WriteLine("test same______________________");
            PDFComparer pDFComparer = new PDFComparer("Test page 1.pdf", "Test page 2.pdf", new ConcreteLogger());
            pass = pass & pDFComparer.ComparePDFText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.txt"));
            pass = pass & pDFComparer.ComparePDFImages(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.zip"));
            Console.WriteLine("PASSED TEST? " + pass.ToString());
            Console.WriteLine("test no pic______________________");
            pass = true;
            pDFComparer = new PDFComparer("Test page 1.pdf", "Test page no pic.pdf", new ConcreteLogger());
            pass = pass & pDFComparer.ComparePDFText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.txt"));
            pass = pass & !pDFComparer.ComparePDFImages(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.zip"));
            Console.WriteLine("PASSED TEST? " + pass.ToString());
            Console.WriteLine("test wrong text______________________");
            pass = true;
            pDFComparer = new PDFComparer("Test page 1.pdf", "Test page wrong text.pdf", new ConcreteLogger());
            pass = pass & !pDFComparer.ComparePDFText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.txt"));
            pass = pass & pDFComparer.ComparePDFImages(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.zip"));
            Console.WriteLine("PASSED TEST? " + pass.ToString());
        }
    }
}
