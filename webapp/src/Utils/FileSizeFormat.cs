using System;

public enum FileSizeFormat 
{
    /* Here file sizes are as-is, 1000 bytes shall be displayed as 1000 bytes */
    Plain = 0,

    /* Here 1Kb is considered 1000 bytes */
    Decimal = 1, 

    /* Here 1Kb is considered 1024 bytes */
    Binary = 2 
}
