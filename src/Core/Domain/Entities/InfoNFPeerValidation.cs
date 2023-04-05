using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InfoNFPeerValidation
    {
        [JsonPropertyName("value")]
        public List<InfoNFPeerValidationLine> NF { get; set; }
    }

    public class InfoNFPeerValidationLine
    {
        public string? CANCELED { get; set; }
        public int DocEntry { get; set; }
        public int DocEntryNF { get; set; }
        public int DocNum { get; set; }
        public string? U_CT_Qualidade { get; set; }
        public string? U_ChaveAcesso { get; set; }
        public string? U_WMS_Status { get; set; }
        public string? U_WM_TagDanfe { get; set; }

        public string? U_CT_Label { get; set; }
    }

}
