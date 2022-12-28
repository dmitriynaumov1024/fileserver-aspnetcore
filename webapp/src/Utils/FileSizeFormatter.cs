using System;
using System.Globalization;

public class FileSizeFormatter : IFormatProvider, ICustomFormatter
{
    public FileSizeFormat SizeFormat { get; set; }

    public FileSizeFormatter () 
    { 
        this.SizeFormat = FileSizeFormat.Plain;
    }

    public object GetFormat (Type formatType)
    {
        return this;
    }

    public string Format (string fmt, object arg, IFormatProvider formatProvider)
    {
        if (typeof(long).IsAssignableFrom(arg.GetType())) {
            long size = (long)arg;
            double dsize = (double)size;

            if (SizeFormat == FileSizeFormat.Plain) {
                return $"{size} bytes";
            }
            else if (SizeFormat == FileSizeFormat.Decimal) {
                return size switch {
                    >= 1000000000 => $"{(dsize/1000000000):F2} Gb",
                    >= 1000000 => $"{(dsize/1000000):F2} Mb",
                    >= 1000 => $"{(dsize/1000):F2} Kb",
                    _ => $"{size} bytes"
                };
            }
            else if (SizeFormat == FileSizeFormat.Binary) {
                return size switch {
                    >= 1073741824 => $"{(dsize/1073741824):F2} Gb",
                    >= 1048576 => $"{(dsize/1048576):F2} Mb",
                    >= 1024 => $"{(dsize/1024):F2} Kb",
                    _ => $"{size} bytes"
                };
            }
        }
        else if (arg is IFormattable) {
            return (arg as IFormattable).ToString(fmt, CultureInfo.CurrentCulture);
        }
        
        return arg.ToString();
    }

}
