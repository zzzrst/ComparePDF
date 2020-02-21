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
            string regex = "";
            TextInteractor.TextInteractor interactor = new TextInteractor.TextInteractor("\\Test page 1.pdf");
            Console.WriteLine(PDFComparer.ComparePDF(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page 1.pdf",
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Test page no pic.pdf",
                regex));
        }
    }
}
