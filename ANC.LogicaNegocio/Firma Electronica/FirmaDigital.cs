using ANC.Entidades;
using ANC.LogicaNegocio.CertificadoElectronico;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Pdfa;
using iText.Signatures;
using Org.BouncyCastle.X509;
using System.Text;
using ZXing;

namespace ANC.LogicaNegocio.FirmaElectronica
{
    public class FirmaDigital
    {

        public RespuestaArchivoBE FirmarDocumento(byte[] pdfOriginal, string ruta_certificado, string password_certificado,string ruta_imagen_firma, string razon, string ubicacion, string texto1, string texto2, bool visible,DateTime fecha_generacion)
        {
            RespuestaArchivoBE rpta = new RespuestaArchivoBE();

            if (string.IsNullOrEmpty(ruta_certificado))
            {
              
                rpta.codigo = -1;
                rpta.valor = "Especifique la ruta del certificado ";
                return rpta;
            }else
            {
                string directorioPadre = System.IO.Path.GetDirectoryName(ruta_certificado);


                if (!string.IsNullOrEmpty(directorioPadre))
                {
                    // Verificar si el directorio padre existe
                    if (!Directory.Exists(directorioPadre))
                    {
                        try
                        {
                            // Intentar crear el directorio padre
                            Directory.CreateDirectory(directorioPadre);

                        }
                        catch (Exception ex)
                        {
                            rpta.codigo = -1;
                            rpta.valor = "No se pudo crear el Directorio Especificado.";
                            return rpta;
                        }
                    }
                }
                else
                {
                    rpta.codigo = -1;
                    rpta.valor = "La ruta proporcionada no es válida.";
                    return rpta;
                }
               
            }


            if (!File.Exists(ruta_certificado))
            {
                

                rpta.codigo = -1;
                rpta.valor = "No se encotro Archivo pfx en la ruta proporcionada";

                return rpta;
            }

           
            if (string.IsNullOrEmpty(password_certificado))
            {
                
                rpta.codigo = -1;
                rpta.valor = "Especifique la clave del certificado ";
                return rpta;
            }

            
         
            Certificado cert = new Certificado(ruta_certificado, password_certificado);

            try
            {
                using (MemoryStream msInput = new MemoryStream(pdfOriginal))
                using (MemoryStream msOutput = new MemoryStream())
                {
                   
                    PdfWriter writer = new PdfWriter(msOutput);
                    PdfReader reader = new PdfReader(msInput);

                    PdfSigner signer = new PdfSigner(reader, msOutput, new StampingProperties());

                    signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);

                    //Determinamos la Fecha de la certFile
                    DateTime fechaFirma = fecha_generacion;
                    signer.SetSignDate(fechaFirma);
                    signer.SetFieldName("sig" + fechaFirma.ToString("yyyyMMddHHmmss"));
                    signer.SetCertificationLevel(PdfSigner.CERTIFIED_FORM_FILLING);

                    // Configurar las páginas a firmar (todas las páginas del documento)

                        // Create the signature appearance
                        PdfSignatureAppearance appearance = signer.GetSignatureAppearance();

                        Rectangle rectangle = new Rectangle(40, 690, 110, 40);
                        if (visible)
                        {
                            //La Firma será Visible y en la Ultima página del PDF             

                            ImageData imageData = ImageDataFactory.Create(ruta_imagen_firma);
                            appearance.SetReasonCaption("Razón");
                            appearance.SetReason(razon);
                            appearance.SetLocationCaption("Ubicación");
                            appearance.SetLocation(ubicacion);

                            appearance.SetImage(imageData);
                            appearance.SetReuseAppearance(false);
                            appearance.SetPageRect(rectangle);
                            appearance.SetPageNumber(1);
                            appearance.SetImageScale(0.24f);

                            //Añadimos el Nombre y el DNI en la firma como Texto.
                            string signatureText = $"{texto1}\n{texto2}";
                            StringBuilder buf = new StringBuilder();
                            buf.Append(signatureText).Append('\n').Append('\n').Append('\n').Append('\n');
                            string text = buf.ToString();

                            iText.Kernel.Font.PdfFont font = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                            appearance.SetLayer2Font(font);
                            appearance.SetLayer2Text(text);
                            appearance.SetLayer2FontSize(6);
                        }
                     
                        // Obtener la clave privada y la cadena de certificados para la firma
                        IExternalSignature pks = new PrivateKeySignature(cert.Key, DigestAlgorithms.SHA256);
                        X509Certificate[] chain = cert.Chain;



                        // Configurar el nivel de certificación y firmar el documento
                        signer.SetCertificationLevel(PdfSigner.CERTIFIED_NO_CHANGES_ALLOWED);
                        signer.SetFieldName("sig" + fechaFirma.ToString("yyyyMMddHHmmss"));


                        signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
                        

                    


                    byte[] pdfFirmadoBytes = msOutput.ToArray();
                    
                    bool rpt_validFirma = VerificarFirmaExistente(pdfFirmadoBytes);
                
                    if (rpt_validFirma)
                    {
                        rpta.codigo = 0;
                        rpta.valor = "OK";
                        rpta.ArchivosPDF = pdfFirmadoBytes;
                    }
                    else
                    {
                        rpta.codigo = -1;
                        rpta.valor = "Firma No válida";
                        rpta.ArchivosPDF = null;
                    }
                    reader.Close();
                    writer.Close();
                }
            }
            catch (Exception er)
            {
                rpta.codigo = -1;
                rpta.valor = "ERROR " + er.Message ;
            }


            return rpta;
        }


        public bool VerificarFirmaExistente(byte[] pdfBytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(pdfBytes))
                {
                    using (PdfReader reader = new PdfReader(ms))
                    {
                        PdfDocument pdfDocument = new PdfDocument(reader);

                        SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
                        IList<string> firmas = signatureUtil.GetSignatureNames();

                        if (firmas.Count > 0)
                        {
                            string nombreFirma = firmas[firmas.Count - 1]; // Obtener la última firma
                                                                           // Verificar la firma digital

                            foreach (string firma_exist in firmas)
                            {
                                PdfPKCS7 firma_valid = signatureUtil.ReadSignatureData(nombreFirma);
                                bool valid = firma_valid.VerifySignatureIntegrityAndAuthenticity();
                                if (firma_valid.VerifySignatureIntegrityAndAuthenticity())
                                {
                                    return true;
                                }

                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                        pdfDocument.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }




    }
}
