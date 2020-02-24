# ComparePDF
Compares two pdfs. Uses [TextInteractor](https://github.com/zzzrst/TextInteractor)
## How to use
Call the method
```c#
ComparePDF(pdfPath1, pdfPath2, args);
```
It will return true if the pdfs are the same, false otherwise.
in args, use the format:
```c#
"REGEX];[VALUE"
```
where REGEX is the regular expression to replace with VALUE.
