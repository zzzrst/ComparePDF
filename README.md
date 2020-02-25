# ComparePDF
Compares two pdfs. For use in C#.
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

## Dependancies
 - [TextInteractor](https://github.com/zzzrst/TextInteractor)
