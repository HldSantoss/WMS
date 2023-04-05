using Domain.Entities;

namespace Infra.Pdf.Operations;

public static class TemplateGenerator
{
    public static string GetHTMLString(PackingList packingList)
    {
        if (packingList?.Items == null)
            return "";

        string s = $@"<html>
                        <head></head>
                        <body>
                            <div class='header'><h1>Romaneio de Embarque</h1></div>
                            <table class='itens' align='left'>
                                <tr>
                                    <td>Romaneio:</td> <td>{packingList.DocEntry}</td>
                                </tr>
                                <tr>
                                    <td>Data de Impressão:</td> <td>{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</td>
                                </tr>
                                <tr>
                                    <td>Transportadora:</td> <td>{packingList.CarrierId}</td>
                                </tr>
                                <tr>
                                    <td>Método de Envio:</td> <td>{packingList.Method}</td>
                                </tr>
                                <tr>
                            </table>
                            <table class='itens' align='center'>
                                <tr>
                                    <th>#</th>
                                    <th>Cliente</th>
                                    <th>NF SAP</th>
                                    <th>Numero NF</th>
                                    <th>Serie</th>
                                </tr>";
        int line = 1;
        foreach (var item in packingList.Items)
        {
            s += $@"<tr>
                        <td>{line++}</td>
                        <td>{item.CardName}</td>
                        <td>{item.DocNum}</td>
                        <td>{item.SequenceSerial}</td>
                        <td>{item.SeriesStr}</td>
                    </tr>";
        }

        s += @"             </table>
                        </body>
                    </html>";

        return s;
    }
}