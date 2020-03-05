# ComparePDF
This is a .Net 2.0 Standard Library. 
This directory contains the source code for the package ComparePDF. This program compares two different PDFs and see if they are identical. Uses software that runs on Windows 32 bit.

## How to use
```c#
public class Compare()
{
    using ComparePDF;

    public static void Main(string[] args)
    {
        bool same;
        string pdfPath1 = "Page.pdf";
        string pdfPath2 = "Page.pdf";
        PDFComparer pDFComparer = new PDFComparer(pdfPath1, pdfPath2)
        same = ComparePDFText(pdfPath1, pdfPath2);
        Console.WriteLine(same);
        //true
        same = ComparePDFImages(pdfPath1, pdfPath2);
        Console.WriteLine(same);
        //true
    }
}
```
It will return true if the pdfs are the same, false otherwise.

## Dependancies
 - [TextInteractor](https://github.com/zzzrst/TextInteractor)
