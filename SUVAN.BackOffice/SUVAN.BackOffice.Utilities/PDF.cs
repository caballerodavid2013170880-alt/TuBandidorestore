using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace SUVAN.BackOffice.Utilities
{
    public class PDF
    {


        public static byte[] getBytesPDF(string HtmlContent)
        {
            try
            {
                var documentPDF = new PdfDocument();
                PdfGenerator.AddPdfPages(documentPDF, HtmlContent, PageSize.A4);

                byte[]? bytesPDF = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    documentPDF.Save(ms);
                    bytesPDF = ms.ToArray();
                }

                return bytesPDF;
            }
            catch (Exception e) { 
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
