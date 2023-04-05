using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Picking
    {
        private string carrier;

        public Picking(long docEntry, string numAtCard, string cardName, string carrier, List<PickingItem> items, TaxExtension taxExtension)
        {
            DocEntry = docEntry;
            NumAtCard = numAtCard;
            CardName = cardName;
            Carrier = carrier;
            Items = items;
            TaxExtension = taxExtension;
        }

        public Picking()
        { }

        [JsonPropertyName("BPL_IDAssignedToInvoice")]
        public int BPLId { get; set; }
        public long DocEntry { get; set; }
        public long DocNum { get; set; }
        public string NumAtCard { get; set; }
        public string CardName { get; set; }
        public string? Carrier { get; set; }
        public string CarrierName { get; set; }

        [JsonPropertyName("U_CT_Method")]
        public string CarrierMethod { get; set; }

        [JsonPropertyName("U_CT_DeliveryType")]
        public string CarrierType { get; set; }

        [JsonPropertyName("U_CT_TrackingCode")]
        public string TrackingCode { get; set; }
        public string Contract { get; set; }
        public DateTime DocDate { get; set; }

        [JsonPropertyName("U_ChaveAcesso")]
        public string AccessKey { get; set; }

        [JsonPropertyName("U_CT_Label")]
        public string Label { get; set; }

        public string SeriesString { get; set; }
        public long? SequenceSerial { get; set; }
        public double DocTotal { get; set; }

        [JsonPropertyName("U_TX_DtComp")]
        public DateTime? InvoiceDate { get; set; }

        [JsonPropertyName("DocumentLines")]
        public List<PickingItem> Items { get; set; }
        public TaxExtension TaxExtension { get; set; }
        public AddressExtension AddressExtension { get; set; }
        public List<Label> Labels { get; set; }

        public void CreateLabelsDanfe(Branch branch, List<Label> labels)
        {
            if (labels == null)
                labels = new List<Label>();

            var label = LabelLayoutDanfe().Replace("%AccessKey%", $"{AccessKey.Trim()}")
                                     .Replace("%BPLName%", $"{branch.BPLName?.Trim()}")
                                     .Replace("%CNPJ%", $"{branch.CNPJ?.Trim()}")
                                     .Replace("%IE%", $"{branch.IE?.Trim()}")
                                     .Replace("%State%", $"{branch.State?.Trim()}")
                                     .Replace("%Series%", $"{SeriesString.Trim()}")
                                     .Replace("%SequenceSerial%", $"{SequenceSerial}")
                                     .Replace(@"%InvoiceData%", $"{InvoiceDate?.ToString("dd/MM/yyyy")}")
                                     .Replace(@"%CardName%", $"{CardName.Trim()}")
                                     .Replace(@"%CPF%", $"{TaxExtension.CPF?.Trim()}{TaxExtension.CNPJ?.Trim()}")
                                     .Replace(@"%Endereco%", $"{AddressExtension.ShipToAddressType} {AddressExtension.ShipToStreet}, {AddressExtension.ShipToStreetNo} - {AddressExtension.ShipToBlock}")
                                     .Replace(@"%City%", $"{AddressExtension.ShipToCity}")
                                     .Replace(@"%ZipCode%", $"{AddressExtension.ShipToZipCode}")
                                     .Replace(@"%StateS%", $"{AddressExtension.ShipToState}")
                                     .Replace(@"%Carrier%", $"{CarrierName?.Trim()}")
                                     .Replace(@"%Total%", $"{string.Format(new CultureInfo("pt-BR"), "{0:C}", DocTotal)}")
                                     .Replace("%DocNum%", DocNum.ToString());

            var plainTextBytes = Encoding.UTF8.GetBytes(label);
            labels.Add(new Label(Convert.ToBase64String(plainTextBytes)));

            Labels = labels;
        }

        public void CreateLabelsDanfeCarreios(Branch branch, List<Label> labels)
        {
            if (labels == null)
                labels = new List<Label>();

            string layout = "";

            if (CarrierMethod?.ToUpper() == "PAC")
                layout = LabelLayoutDanfeCorreiosPac();
            else
                layout = LabelLayoutDanfeCorreiosSedex();

            var label = layout.Replace("%AccessKey%", $"{AccessKey.Trim()}")
                                     .Replace("%BPLNameSender%", $"{branch.BPLName?.Trim()}")
                                     .Replace("%CNPJSender%", $"{branch.CNPJ?.Trim()}")
                                     .Replace("%IESender%", $"{branch.IE?.Trim()}")
                                     .Replace("%State%", $"{branch.State?.Trim()}")
                                     .Replace("%Series%", $"{SeriesString.Trim()}")
                                     .Replace("%SequenceSerial%", $"{SequenceSerial}")
                                     .Replace("%InvoiceDate%", $"{InvoiceDate?.ToString("dd/MM/yyyy")}")
                                     .Replace("%CardName%", $"{CardName.Trim()}")
                                     .Replace("%CPF%", $"{TaxExtension.CPF?.Trim()}{TaxExtension.CNPJ?.Trim()}")
                                     .Replace("%Endereco%", $"{AddressExtension.ShipToAddressType?.Trim()} {AddressExtension.ShipToStreet?.Trim()}, {AddressExtension.ShipToStreetNo?.Trim()} - {AddressExtension.ShipToBlock?.Trim()}")
                                     .Replace("%City%", $"{AddressExtension.ShipToCity}")
                                     .Replace("%ZipCode%", $"{AddressExtension.ShipToZipCode}")
                                     .Replace("%StateS%", $"{AddressExtension.ShipToState}")
                                     .Replace("%Carrier%", $"{CarrierName?.Trim()}")
                                     .Replace("%Total%", $"{string.Format(new CultureInfo("pt-BR"), "{0:C}", DocTotal)}")
                                     .Replace("%DocNum%", DocNum.ToString())
                                     .Replace("%TrackingCode%", $"{TrackingCode?.Trim()}")
                                     .Replace("%AddressSender%", $"{branch.AddrType}{branch.Street},{branch.StreetNo} - {branch.Block}")
                                     .Replace("%CitySender%", $"{branch.City}")
                                     .Replace("%ZipCodeSender%", $"{branch.ZipCode}")
                                     .Replace("%StateSender%", $"{branch.State}")
                                     .Replace("%Contract%", $"{Contract}");

            var plainTextBytes = Encoding.UTF8.GetBytes(label);
            labels.Add(new Label(Convert.ToBase64String(plainTextBytes)));

            Labels = labels;
        }

        private string LabelLayoutDanfeCorreiosPac()
        {
            return @"CT~~CD,~CC^~CT~
^XA
~TA000
~JSN
^LT0
^MNW
^MTT
^PON
^PMN
^LH0,0
^JMA
^PR4,4
~SD15
^JUS
^LRN
^CI27
^PA0,1,1,0
^XZ
^XA
^MMT
^PW799
^LL1199
^LS0
^FT177,49^A0N,34,33^FH\^CI28^FDDANFE Simplificado – Etiqueta^FS^CI27
^FT313,1018^A0N,23,23^FH\^CI28^FDCHAVE DE ACESSO^FS^CI27
^BY2,3,145^FT123,1169^BCN,,Y,N
^FH\^FD>;%AccessKey%^FS
^FT36,853^A0N,23,23^FH\^CI28^FD%BPLNameSender%^FS^CI27
^FT36,886^A0N,23,23^FH\^CI28^FDCNPJ:^FS^CI27
^FT100,886^A0N,23,23^FH\^CI28^FD%CNPJSender%^FS^CI27
^FT410,882^A0N,23,23^FH\^CI28^FDIE:^FS^CI27
^FT452,882^A0N,23,23^FH\^CI28^FD%IESender%^FS^CI27
^FO33,779^GB711,0,3^FS
^FT28,134^A0N,23,23^FH\^CI28^FDSÉRIE :^FS^CI27
^FT28,104^A0N,23,23^FH\^CI28^FDNÚMERO:^FS^CI27
^FT108,134^A0N,23,23^FH\^CI28^FD%Series%^FS^CI27
^FT127,104^A0N,23,23^FH\^CI28^FD%SequenceSerial%^FS^CI27
^FT28,164^A0N,23,23^FH\^CI28^FDDATA DE EMISSÃO:^FS^CI27
^FT218,164^A0N,23,23^FH\^CI28^FD%InvoiceDate%^FS^CI27
^FT28,616^A0N,23,23^FH\^CI28^FD%CardName%^FS^CI27
^FT271,684^A0N,23,23^FH\^CI28^FD/^FS^CI27
^FT28,751^A0N,23,23^FH\^CI28^FD%CPF%^FS^CI27
^FT28,650^A0N,23,23^FH\^CI28^FD%Endereco%^FS^CI27
^FT28,684^A0N,23,23^FH\^CI28^FD%City%^FS^CI27
^FT28,717^A0N,23,23^FH\^CI28^FD%ZipCode%^FS^CI27
^FT283,684^A0N,23,23^FH\^CI28^FD%State%^FS^CI27
^FT28,224^A0N,23,23^FH\^CI28^FDTIPO OPER.:^FS^CI27
^FT158,223^A0N,23,23^FH\^CI28^FD1-SAÍDA^FS^CI27
^FT28,194^A0N,23,23^FH\^CI28^FDVALOR TOTAL :^FS^CI27
^FT181,194^A0N,23,23^FH\^CI28^FD%Total%^FS^CI27
^FT636,210^A0N,23,23^FH\^CI28^FDVOL.:^FS^CI27
^FT699,210^A0N,23,23^FH\^CI28^FD%vol%^FS^CI27
^FT28,255^A0N,23,23^FH\^CI28^FDPAC - CONTRATO:^FS^CI27
^FT245,253^A0N,23,23^FH\^CI28^FD%Contract%^FS^CI27
^FO636,58^GFA,445,1904,16,:Z64:eJzNlT1uxCAQhXGiaKWkcBWl5Ag5go+So5ijpIz2EuYIOYJLlxQprBXyrJffmcECuYsrf3ow87DhIYR7rlcl0PMOoBE+AcCGBnzsDL+ZHwhbwhfHkPjT8xxZejZkeu7QBY4F+sgzmZ4KTJFXz2Nk6/A5YnCYGWj50EAyHjKvpHwwkNE16BBb2s415Nxj1iVLzDNjQ+w5gyNjqOmW6Rux/2BidzfM+Y2yulDW/Tl9lkyXdd0MlNeWzthMTB/r4wsd6ro9qdv/pjM+u/7i+zE++/8M1xkX+4fvr8b+01xn+101zoPqmM7PU/382VIfMK+N87808mTPD7JAnkeK5hcc5Bs2aI/yERk0R/mKDPA8ViT+Q35nA7fj/E8NFs+pgRakQbqgRlI+XSBL5AuZ7q8/fL95Byazc/CTWXwB/CEUr9Pt27/dAfL3sCA=:5FA3
^BY3,3,143^FT116,466^BCN,,Y,Y
^FH\^FD>:%TrackingCode%^FS
^FT28,489^A0N,23,23^FH\^CI28^FDRecebedor:^FS^CI27
^FO146,489^GB598,0,3^FS
^FT28,526^A0N,23,23^FH\^CI28^FDAssinatura:^FS^CI27
^FO143,525^GFA,33,84,28,:Z64:eJz7/x8H+MPAgEuKkBwAM79Nqg==:057F
^FT353,526^A0N,23,23^FH\^CI28^FDDocumento:^FS^CI27
^FO471,524^GB273,0,3^FS
^FO28,540^GB189,39,37^FS
^LRY^FT28,571^A0N,31,30^FH\^CI28^FDDESTINATÁRIO^FS^CI27^LRN
^FO223,542^GB521,0,3^FS
^BY3,3,148^FT423,750^BCN,,Y,N
^FH\^FD>:%ZipCode%^FS
^FT33,817^A0N,34,33^FH\^CI28^FDREMETENTE^FS^CI27
^FT36,919^A0N,23,23^FH\^CI28^FD%AddressSender%^FS^CI27
^FT36,952^A0N,23,23^FH\^CI28^FD%CitySender%^FS^CI27
^FT36,985^A0N,23,23^FH\^CI28^FD%ZipCodeSender%^FS^CI27
^FT410,952^A0N,23,23^FH\^CI28^FD%StateSender%^FS^CI27
^FT28,287^A0N,23,23^FH\^CI28^FDPedido SAP:^FS^CI27
^FT161,287^A0N,23,23^FH\^CI28^FD%DocNum%^FS^CI27
^FT389,952^A0N,23,23^FH\^CI28^FD/^FS^CI27
^PQ1,0,1,Y
^XZ
";
        }

        private string LabelLayoutDanfeCorreiosSedex()
        {
            return @"CT~~CD,~CC^~CT~
^XA
~TA000
~JSN
^LT0
^MNW
^MTT
^PON
^PMN
^LH0,0
^JMA
^PR4,4
~SD15
^JUS
^LRN
^CI27
^PA0,1,1,0
^XZ
^XA
^MMT
^PW799
^LL1199
^LS0
^FT177,49^A0N,34,33^FH\^CI28^FDDANFE Simplificado – Etiqueta^FS^CI27
^FT312,1018^A0N,23,23^FH\^CI28^FDCHAVE DE ACESSO^FS^CI27
^BY2,3,145^FT122,1169^BCN,,Y,N
^FH\^FD>;%AccessKey%^FS
^FT36,853^A0N,23,23^FH\^CI28^FD%BPLNameSender%^FS^CI27
^FT36,886^A0N,23,23^FH\^CI28^FDCNPJ:^FS^CI27
^FT100,886^A0N,23,23^FH\^CI28^FD%CNPJSender%^FS^CI27
^FT410,882^A0N,23,23^FH\^CI28^FDIE:^FS^CI27
^FT452,882^A0N,23,23^FH\^CI28^FD%IESender%^FS^CI27
^FO33,779^GB711,0,3^FS
^FT28,134^A0N,23,23^FH\^CI28^FDSÉRIE :^FS^CI27
^FT28,104^A0N,23,23^FH\^CI28^FDNÚMERO:^FS^CI27
^FT108,134^A0N,23,23^FH\^CI28^FD%Series%^FS^CI27
^FT127,104^A0N,23,23^FH\^CI28^FD%SequenceSerial%^FS^CI27
^FT28,164^A0N,23,23^FH\^CI28^FDDATA DE EMISSÃO:^FS^CI27
^FT218,164^A0N,23,23^FH\^CI28^FD%InvoiceDate%^FS^CI27
^FT28,616^A0N,23,23^FH\^CI28^FD%CardName%^FS^CI27
^FT271,684^A0N,23,23^FH\^CI28^FD/^FS^CI27
^FT28,751^A0N,23,23^FH\^CI28^FD%CPF%^FS^CI27
^FT28,650^A0N,23,23^FH\^CI28^FD%Endereco%^FS^CI27
^FT28,684^A0N,23,23^FH\^CI28^FD%City%^FS^CI27
^FT28,717^A0N,23,23^FH\^CI28^FD%ZipCode%^FS^CI27
^FT283,684^A0N,23,23^FH\^CI28^FD%State%^FS^CI27
^FT28,224^A0N,23,23^FH\^CI28^FDTIPO OPER.:^FS^CI27
^FT158,223^A0N,23,23^FH\^CI28^FD1-SAÍDA^FS^CI27
^FT28,194^A0N,23,23^FH\^CI28^FDVALOR TOTAL :^FS^CI27
^FT181,194^A0N,23,23^FH\^CI28^FD%Total%^FS^CI27
^FT636,210^A0N,23,23^FH\^CI28^FDVOL.:^FS^CI27
^FT699,210^A0N,23,23^FH\^CI28^FD%vol%^FS^CI27
^FT28,255^A0N,23,23^FH\^CI28^FDSEDEX - CONTRATO:^FS^CI27
^FT245,253^A0N,23,23^FH\^CI28^FD%Contract%^FS^CI27
^FO636,58^GFA,565,1984,16,:Z64:eJytlTtuwzAMQOUaRQpkcJeio/ZOvYFyiaLXyAEKyEfJWGTpEayjeMyoJYBROGb1F0nASYZye3gSSUWiI4SPp+N3L1B8wmWHsAWAX8SvjqEuePAIU+FN4EthFRgOmXVki7L7WMh2F3Q7QM/YRM6YOmgKzyR9LlDSpwKqciggKxuSHmBE3ecTtMhPpHxsAPuFlg8NdJh7Uj40oDCPpHxgjHBi3jI/kfY8t4Rn5mfmF9q+O0BHub/lJfPyujeK8njLqxteM4br3vL13LP9fP2dfjn+YO+H7wX5kx+yofhLfcfBT3UKg09zIZPPc7VNPs9do6M/5bkcoi9zK4NfCrfB28JN8GW7f2vOG4ES2DK1Irx15yu6I1iU3hew2iIWzo+UtcEsLU7vCliNUWzsQri1X4QbOzHeExb7kfGO8rthTFE8i/+JtwPBRzgTHsg1hCGeEXcA5Hf2LwN9v0V5ZJjrRcRvQE2QZpykB3TRMrLJrCKPmdMMlAJpFvJVNIzbFZ9PuE289v/RMV71PWm/HEDd68f7vGJ+YF6vrU/8kWPn4A9Mhh8U:982A
^BY3,3,143^FT116,466^BCN,,Y,Y
^FH\^FD>:%TrackingCode%^FS
^FT28,489^A0N,23,23^FH\^CI28^FDRecebedor:^FS^CI27
^FO146,489^GB598,0,3^FS
^FT28,526^A0N,23,23^FH\^CI28^FDAssinatura:^FS^CI27
^FO143,525^GFA,33,84,28,:Z64:eJz7/x8H+MPAgEuKkBwAM79Nqg==:057F
^FT353,526^A0N,23,23^FH\^CI28^FDDocumento:^FS^CI27
^FO471,524^GB273,0,3^FS
^FO28,540^GB189,39,37^FS
^LRY^FT28,571^A0N,31,30^FH\^CI28^FDDESTINATÁRIO^FS^CI27^LRN
^FO223,542^GB521,0,3^FS
^BY3,3,148^FT423,750^BCN,,Y,N
^FH\^FD>:%ZipCode%^FS
^FT33,817^A0N,34,33^FH\^CI28^FDREMETENTE^FS^CI27
^FT36,919^A0N,23,23^FH\^CI28^FD%AddressSender%^FS^CI27
^FT36,952^A0N,23,23^FH\^CI28^FD%CitySender%^FS^CI27
^FT36,985^A0N,23,23^FH\^CI28^FD%ZipCodeSender%^FS^CI27
^FT410,952^A0N,23,23^FH\^CI28^FD%StateSender%^FS^CI27
^FT28,287^A0N,23,23^FH\^CI28^FDPedido SAP:^FS^CI27
^FT161,287^A0N,23,23^FH\^CI28^FD%DocNum%^FS^CI27
^FT389,952^A0N,23,23^FH\^CI28^FD/^FS^CI27
^PQ1,0,1,Y
^XZ
";
        }

        public void CreateLabelControl()
        {
            var labels = new List<Label>();

            var label = LabelLayoutControl().Replace("%DocEntry%", $"{DocEntry}")
                                            .Replace("%OrderDate%", $"{DocDate.ToString("dd/MM/yyyy")}");


            var plainTextBytes = Encoding.UTF8.GetBytes(label);
            labels.Add(new Label(Convert.ToBase64String(plainTextBytes)));
            Labels = labels;
        }

        private string LabelLayoutControl()
        {
            return @"CT~~CD,~CC^~CT~
^XA
~TA000
~JSN
^LT0
^MNW
^MTT
^PON
^PMN
^LH0,0
^JMA
^PR4,4
~SD15
^JUS
^LRN
^CI27
^PA0,1,1,0
^XZ
^XA
^MMT
^PW799
^LL400
^LS0
^FO18,20^GFA,1013,3000,20,:Z64:eJy1lkFq4zAUhp+dGAYvSgrRBboqLnT2Mxsvmr0D1n3ErEIXPUPISkggZhkcmCPMGbo0ZvARwjxJjm1JbhoPzAuJwfn5/el/T0qEwGqapu3rD9CgShB9VSdbR9huP9IppW72k/LiR7AmdXP4qhEf9QF73Sw+edIvbvjIlG5WfqeeL8D7b/m1/9bfD3Xz+Eb5UUJXJNTxNOQrYEIHT+P8Oh1M6CD1/UoYhCMdpNImeMkP/QCIm1/N8N5r6Afg+glz75uTn9URNz/tx766+eWhzho+OflREuqwwTsA5fdjwq/Ce9+d/OyDPb5GAz67fsUE397w9flpPBqutxEMmM9nconCfkAmnfwm+tH1zZ8XrCTQxQz5hvwoXU30owbG2U55fknu6zhkx9jLT89poEuV8vMrCA38sFiQn0nGza9pa7z3y+tvEeRnGvJUef0ddshw/vF+g4z9iK8T+1hv4C4/as+XVUJdHZ5/tdBeZ8dvOD9G89cf0Mcr54ZS4tbzRQoLqP0QMNrem6I06nXAgTG2Y6rRI2j8ClhDBLCCxCza6PbwxYwGLN4h7fj01jC6KNezZXRM68zHDuLOD2C53GidnWmre3yMBV/8TIFliy4/wCdHRVQkAGWvaznEdVzfMXZm584Pv4rKRM80vegAxy/m8R5iOEWn6mh11g8hBz8cP4F+kLK25yuWCdV+Cayj8TrQ78uCwUKiOeZn1m/8hvXucZ1Q/3h/jd/ZXeeXY36AfkuUXnQcHo1fBkecQctXQGHWQaCE0bxgh1tshfq0v9LMjDxVUnI7nc5vnNU1olWqadV4XnJCinJD77cl6XWH7PWAlwdeZVXHR/P76GW9IVEx0nGR8uffbwd+fuv9NuvkpUxIlKPpwPdwyDJ5EPih+Swejj1x+Bq9P5pb51n/g9GbpLqyP5T+AVa3+knz7vh8nYWb8/+lQ7zGZwDbDvEWPw04zafTs/ldGG/y+4RP5ydm8X2Wn77OyE9c40M0i3iF7y9zgDS4:82A8
^BY6,3,106^FT158,331^BCN,,Y,N
^FH\^FD>:%DocEntry%^FS
^FT192,76^A0N,56,56^FH\^CI28^FDPed.:^FS^CI27
^FT335,76^A0N,56,56^FH\^CI28^FD%DocEntry%^FS^CI27
^FT192,156^A0N,56,56^FH\^CI28^FDDt.:^FS^CI27
^FT290,156^A0N,56,56^FH\^CI28^FD%OrderDate%^FS^CI27
^FO6,194^GB786,0,4^FS
^PQ1,0,1,Y
^XZ
";
        }

        private string LabelLayoutDanfe()
        {
            return @"CT~~CD,~CC^~CT~
^XA
~TA000
~JSN
^LT0
^MNW
^MTT
^PON
^PMN
^LH0,0
^JMA
^PR4,4
~SD15
^JUS
^LRN
^CI27
^PA0,1,1,0
^XZ
^XA
^MMT
^PW799
^LL1199
^LS0
^FT30,87^A0N,56,56^FH\^CI28^FDDANFE Simplificado – Etiqueta^FS^CI27
^FO14,26^GFA,225,9200,100,:Z64:eJztmiEOxEAMAw/3FYv7zIP3Cquo6iuPdHkCbMmRBwXNyDzPefy+F+77JB0fDc+QHapOqAFzv6rD9qs6bH/ogSEdtl/VYfvVnVAD5n5Vh+3fLHN/6IEhHbZf1WH71Z1QA+b+zTL3b2DuDz0wpMP2qzpsv7oTaixz/wbmflWH7Q89MKTD9qs6bL+6E2rA3K/qsP2qDtsfekDUof2VvIdqxx8lzaCT:DDD2
^FO8,141^GB791,0,4^FS
^FT287,175^A0N,29,30^FH\^CI28^FDCHAVE DE ACESSO^FS^CI27
^BY2,3,145^FT123,345^BCN,,Y,N
^FH\^FD>;%AccessKey%^FS
^FO4,395^GB792,0,4^FS
^FT190,450^A0N,28,30^FH\^CI28^FD%BPLName%^FS^CI27
^FT190,501^A0N,28,30^FH\^CI28^FDCNPJ:^FS^CI27
^FT274,501^A0N,28,30^FH\^CI28^FD%CNPJ%^FS^CI27
^FT190,551^A0N,28,30^FH\^CI28^FDIE:^FS^CI27
^FT274,553^A0N,28,30^FH\^CI28^FD%IE%^FS^CI27
^FT653,553^A0N,28,30^FH\^CI28^FDUF:^FS^CI27
^FT724,551^A0N,28,30^FH\^CI28^FD%State%^FS^CI27
^FO8,661^GB791,0,4^FS
^FO8,788^GB791,0,4^FS
^FO8,842^GB791,0,4^FS
^FO547,665^GB0,124,4^FS
^FT282,831^A0N,39,38^FH\^CI28^FDDESTINATÁRIO^FS^CI27
^FT28,720^A0N,31,30^FH\^CI28^FDSÉRIE :^FS^CI27
^FT39,763^A0N,31,30^FH\^CI28^FDNº^FS^CI27
^FT160,720^A0N,31,30^FH\^CI28^FD%Series%^FS^CI27
^FT98,763^A0N,31,30^FH\^CI28^FD%SequenceSerial%^FS^CI27
^FT280,717^A0N,28,30^FH\^CI28^FDDATA DE EMISSÃO:^FS^CI27
^FT331,763^A0N,31,30^FH\^CI28^FD%InvoiceData%^FS^CI27
^FT24,882^A0N,25,25^FH\^CI28^FDNOME/RAZÃO SOCIAL :^FS^CI27
^FT294,882^A0N,25,25^FH\^CI28^FD%CardName%^FS^CI27
^FT24,924^A0N,25,25^FH\^CI28^FDCPF/CNPJ :^FS^CI27
^FT24,967^A0N,25,25^FH\^CI28^FDENDEREÇO :^FS^CI27
^FT24,1010^A0N,25,25^FH\^CI28^FDCIDADE :^FS^CI27
^FT253,1053^A0N,25,25^FH\^CI28^FDUF :^FS^CI27
^FT24,1053^A0N,25,25^FH\^CI28^FDCEP :^FS^CI27
^FT166,924^A0N,25,25^FH\^CI28^FD%CPF%^FS^CI27
^FT170,967^A0N,25,25^FH\^CI28^FD%Endereco%^FS^CI27
^FT143,1010^A0N,25,25^FH\^CI28^FD%City%^FS^CI27
^FT98,1053^A0N,25,25^FH\^CI28^FD%ZipCode%^FS^CI27
^FT313,1053^A0N,25,25^FH\^CI28^FD%State%^FS^CI27
^FO242,665^GB0,124,4^FS
^FT562,712^A0N,23,23^FH\^CI28^FDTIPO OPER.:^FS^CI27
^FT691,712^A0N,23,23^FH\^CI28^FD1-SAÍDA^FS^CI27
^FO8,1083^GB791,0,4^FS
^FT23,1173^A0N,25,25^FH\^CI28^FDVALOR TOTAL :^FS^CI27
^FT596,1173^A0N,25,25^FH\^CI28^FD%Total%^FS^CI27
^FO29,422^GFA,925,2960,20,:Z64:eJydlj+K4zAYxZ+1YwgZCBmI+iFVyN5gt/HApJfBuY9JtaSYM5ipjAyuFwf2LCmNixxh2U+SZVv+s3FGSRU/Hr/vPUmOlP2V4DhYAs3j4lKvBGE4pcvzW7Ou//fLssz6cVqjuqq6zfYripbv2Ads+fK5fhpQfQwfH9M9xNfmN8D7Sn5yVn7VfL5uftN8X8zvyI+CD3XlecgnMNQl2MpOfvUz8L4uBZZDv1bY0YEpuE5+SmeFNj+lw3noB7h+Uv/m5heN6X4BC+RufsGIjiYGvnfys0JXR/XGQL+PaMLPzc8A9vrVA8/gM/M2+VG9Y/NW5VR+/kgfhZufaO0cXSrv90HTLsEuzvkd06XxOY7jvh8PBn5MytdefsJTlj2/XCXo5idG+1AJ/qzzO+r8zCRuftWtHOSnC+HDfpns9Us6r5efzBPYE9zxs7Ku316257cGDLirU9dLe8E8cP9N6fLh/XKY8qOV0bfOb421rhdcXx9aVyLFKcYrPq5qaO0XWJ0fqAy1LsEODKRjMZ5rPlidp/eq1sXpbinTZbJgcbqq/SDE00H4wQYQVgfapox0e8Z+fzP5HUGPPNKt4UWe9avSmJXL6+LE/jLjF1kdnryo48e25Icl+V0KwyfWHle6DYTn8K2uWHX41AhK53fmoCtI8e2ead4sa+bVfB6aXFLKj5Uf1/RU4k/tJ2D4Iq/NWW6bjrdFYfh6r7jmfum8Qe69P6jeS5bJzOy/gPO3kPNDGPGuLi3POV0zeeMXRP57yDf+m2j9PveUC2F+Fj8avmDjQ5AuEK6fApRV63eIDu+R2obRCJ96x2WT+7lSB2T2+aD01MfyTejyB/IzjBN8Bu6B/wcG8Q6f7jfP5/PJyfwqaRGrB+a9x0fl2i04j+9efpV8hE8VnODFWeFLGAr8A0TgKeU=:31EC
^FT590,762^A0N,34,33^FH\^CI28^FDVOL.:^FS^CI27
^FT680,762^A0N,34,33^FH\^CI28^FD%vol%^FS^CI27
^FO4,579^GB792,0,4^FS
^FT30,634^A0N,39,38^FH\^CI28^FDTransp.:^FS^CI27
^FT177,634^A0N,39,38^FH\^CI28^FD%Carrier%^FS^CI27
^FT24,1126^A0N,25,25^FH\^CI28^FDPedido SAP:^FS^CI27
^FT179,1126^A0N,25,25^FH\^CI28^FD%DocNum%^FS^CI27
^PQ1,0,1,Y
^XZ
";
        }
    }

    public class TaxExtension
    {
        public string? Carrier { get; set; }
        public string? StateS { get; set; }

        [JsonPropertyName("TaxId4")]
        public string? CPF { get; set; }

        [JsonPropertyName("TaxId0")]
        public string? CNPJ { get; set; }
    }

    public class AddressExtension
    {
        public string? ShipToAddressType { get; set; }
        public string? ShipToStreet { get; set; }
        public string? ShipToStreetNo { get; set; }
        public string? ShipToBlock { get; set; }
        public string? ShipToBuilding { get; set; }
        public string? ShipToCity { get; set; }
        public string? ShipToZipCode { get; set; }
        public string? ShipToState { get; set; }
    }

    public class PickingItem
    {
        public PickingItem(int lineNum, string itemCode, string itemDescription, double quantity, IEnumerable<BinLocations> binAllocations, IEnumerable<Series> serialNumbers, string barCode, string manSerNum, string manBtchNum)
        {
            LineNum = lineNum;
            ItemCode = itemCode;
            ItemDescription = itemDescription;
            Quantity = quantity;
            BinAllocations = binAllocations;
            SerialNumbers = serialNumbers;
            BarCode = barCode;
            ManSerNum = manSerNum;
            ManBtchNum = manBtchNum;
            Labels = new List<Label>();
        }

        public PickingItem()
        {
        }

        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string ItemDescription { get; set; }
        public string ManSerNum { get; set; }
        public string ManBtchNum { get; set; }
        public double Quantity { get; set; }
        public string TreeType { get; set; }

        [JsonPropertyName("DocumentLinesBinAllocations")]
        public IEnumerable<BinLocations> BinAllocations { get; set; }
        public IEnumerable<Series> SerialNumbers { get; set; }
        public List<Label> Labels { get; set; }

        public void CreateLabels(long docEntry, string carrier, string user, int qty)
        {
            var labels = new List<Label>();

            for (int i = 0; i < Quantity; i++)
            {

                var label = LayoutLabel().Replace("%User%", $"{user}")
                                         .Replace("%Volumes%", $"{i + 1}/{Quantity}")
                                         .Replace("%Code%", $"{docEntry}")
                                         .Replace("%Name%", $"{docEntry} - {ItemCode}")
                                         .Replace("%Qty%", $"{qty}")
                                         .Replace("%Carrier%", $"{carrier}");

                var plainTextBytes = Encoding.UTF8.GetBytes(label);
                labels.Add(new Label(Convert.ToBase64String(plainTextBytes)));
            }

            Labels = labels;
        }

        private string LayoutLabel()
        {
            return @"CT~~CD,~CC^~CT~
^XA
~TA000
~JSN
^LT0
^MNW
^MTT
^PON
^PMN
^LH0,0
^JMA
^PR4,4
~SD15
^JUS
^LRN
^CI27
^PA0,1,1,0
^XZ
^XA
^MMT
^PW799
^LL400
^LS0
^FT33,252^BQN,2,7
^FH\^FDLA,%Code%^FS
^FT39,314^A0N,39,38^FH\^CI28^FDPick.:^FS^CI27
^FT479,314^A0N,39,38^FH\^CI28^FDQtde SKU.:^FS^CI27
^FPH,4^FT148,312^AAN,36,20^FH\^FD%User%^FS
^FPH,4^FT648,314^AAN,36,20^FH\^FD%Qty%^FS
^FT350,234^AAN,36,20^FH\^FD%Name%^FS
^FPH,1^FT395,104^A0N,36,35^FH\^CI28^FDVolumes:^FS^CI27
^FPH,1^FT395,164^A0N,56,56^FH\^CI28^FD%Volumes%^FS^CI27
^FPH,1^FT39,371^A0N,56,56^FH\^CI28^FD%Carrier%^FS^CI27
^FO665,24^GB109,25,19^FS
^FO651,11^GFA,21,104,4,:Z64:eJxjYKAPAAAAaAAB:A104
^FO662,21^GFA,21,104,4,:Z64:eJxjYKAPAAAAaAAB:A104
^FO657,20^GFA,21,104,4,:Z64:eJxjYKAPAAAAaAAB:A104
^FO656,17^GFA,21,104,4,:Z64:eJxjYKAPAAAAaAAB:A104
^FO652,15^GFA,21,104,4,:Z64:eJxjYKAPAAAAaAAB:A104
^FO662,19^GFA,21,256,8,:Z64:eJxjYBjZAAABAAAB:2F4E
^FO15,48^GB759,218,3^FS
^PQ1,0,1,Y
^XZ";
        }
    }

    public class BinLocations
    {
        public BinLocations(long baseLineNumber, long serialAndBatchNumbersBaseLine, string binCode, double pickedQuantity, long binAbs)
        {
            BaseLineNumber = baseLineNumber;
            SerialAndBatchNumbersBaseLine = serialAndBatchNumbersBaseLine;
            BinCode = binCode;
            PickedQuantity = pickedQuantity;
            BinAbs = binAbs;
        }

        public BinLocations()
        {
        }

        [JsonPropertyName("BinAbsEntry")]
        public long BinAbs { get; set; }
        public long BaseLineNumber { get; set; }
        public long SerialAndBatchNumbersBaseLine { get; set; }
        public string BinCode { get; set; }
        public double Quantity { get; set; }
        public double PickedQuantity { get; set; }
        public bool IsFinish
        {
            get { return Quantity == PickedQuantity; }
        }
    }

    public class Series
    {
        public Series(long lineNum, long baseLineNumber, string internalSerialNumber, double quantity)
        {
            LineNum = lineNum;
            BaseLineNumber = baseLineNumber;
            InternalSerialNumber = internalSerialNumber;
            Quantity = quantity;
        }

        public Series()
        {
        }

        public long LineNum { get; set; }
        public long BaseLineNumber { get; set; }
        public string InternalSerialNumber { get; set; }
        public double Quantity { get; set; }
    }

    public class Label
    {
        public Label(string zpl)
        {
            Zpl = zpl;
        }

        public Label()
        {
        }

        public string Zpl { get; set; }
    }

    public class PickingUser
    {
       public string U_CT_LoginWms { get; set; }
       public string U_WMS_Status { get; set; }
    }
}