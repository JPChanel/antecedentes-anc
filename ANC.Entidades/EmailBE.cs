using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANC.Entidades
{
    public class WSEmailBE
    {
        public string ipPc { get; set; }
        public string pcNameipPc { get; set; }
        public string usuarioSis { get; set; }
        public string usuarioRed { get; set; }
        public string nombreSo { get; set; }
        public string macAddressPc { get; set; }

        //listadoDestinatarios puede ser array
        public string[] destinatarioEmail { get; set; }
        //Tipo de receptor. TO: hacia, CC: Con copia, BCC: Con copia oculta 
        public string[] destinatarioTipo { get; set; }


        public string titulo { get; set; }

        //campo opcional
        public string cuerpoTexto { get; set; }

        //html q va dentro de la plantilla Html original
        public string TextoHtml { get; set; }

        //en caso se use una plantilla nueva personalizada en casos excepcionales
        public string cuerpoHtml{ get; set; }

        //adjunto puede ser array
        public string[] nombreDocumento { get; set; }
        public string[] extensionDocumento { get; set; }
        public string[] binarioDocumento { get; set; }

        public string motivoEmail { get; set; }
       
        public string nombreCompleto { get; set; }

   
    }
}
