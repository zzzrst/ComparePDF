# ComparePDF
This is a .Net 2.0 Standard Library. 
This directory contains the source code for the package ComparePDF. This program compares two different PDFs and see if they are identical. There are options to ingore regexs.

## How to use
```c#
public class Compare()
{
    using ComparePDF;

    public static void Main(string[] args)
    {
        bool same = false;;
        string pdfPath1 = "C:\\Page.pdf";
        string pdfPath2 = "C:\\Page.pdf";
        string args = "];[";
        same = ComparePDF(pdfPath1, pdfPath2, args);
        Console.WriteLine(same);
        //true
    }
}
```
It will return true if the pdfs are the same, false otherwise. In args, use the format:
```c#
string args = "REGEX];[VALUE"
```
where REGEX is the regular expression to replace with VALUE. If the argument is not in this format, no strings will be replaced. The REGEX is the same as the default c# library for Regexs.

## Dependancies
 - [TextInteractor](https://github.com/zzzrst/TextInteractor)
