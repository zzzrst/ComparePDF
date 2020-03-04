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
            //bool pass = true;
            //TextInteractor.TextInteractor interactor = new TextInteractor.TextInteractor("\\Test page 1.pdf");
            //Console.WriteLine("Test Same______________________");
            //pass = pass & PDFComparer.ComparePDF(
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page 1.pdf",
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page 2.pdf",
            //    regex);
            //Console.WriteLine("Test No pic______________________");
            //pass = pass & !PDFComparer.ComparePDF(
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page 1.pdf",
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page no pic.pdf",
            //    regex);
            //Console.WriteLine("Test wrong text______________________");
            //pass = pass & !PDFComparer.ComparePDF(
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page 1.pdf",
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page wrong text.pdf",
            //    regex);

            //Console.WriteLine("PASSED TEST? " + pass.ToString());

            PDFComparer pDFComparer = new PDFComparer("Test page 1.pdf", "Test page 2.pdf", new ConcreteLogger());
            pDFComparer.ComparePDFText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.txt"));
            pDFComparer.ComparePDFImages(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Result.zip"));

        }
    }
}
