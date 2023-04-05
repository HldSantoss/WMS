using Domain.Entities;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;

namespace Application.Services
{
    public class TagCorreiosService : ITagCorreiosService
    {
        private readonly ITagCorreiosSLService _tagCorreiosService;

        public TagCorreiosService(ITagCorreiosSLService tagCorreiosService)
        {
            _tagCorreiosService = tagCorreiosService;
        }

        public async Task<(string sro, string contract)> GetSro(string method)
        {
            var sroData = await GetNext(method);
            var sro = FormatSro(ref sroData);

            return (sro, sroData.Contract);
        }

        private static string FormatSro(ref SroData sroData)
        {
            var current = sroData.Current++.ToString().PadLeft(8, '0');
            var digit = Digit(current);

            var sro = $"{sroData.Prefix}{current}{digit}{sroData.Suffix}";
            return sro;
        }

        private async Task<SroData> GetNext(string method)
        {
            var sroData = await _tagCorreiosService.GetCurrent(method);

            if (sroData == null)
                throw new NullReferenceException($"Range de etiquetas dos correios não cadastrada para {method}");

            sroData.Current++;
            await _tagCorreiosService.SetCurrent(sroData.Code, sroData.Current);

            if (sroData.End <= sroData.Current)
                await _tagCorreiosService.SetFinished(sroData.Code);

            return sroData;
        }

        private static char Digit(string current)
        {
            if (current.Length != 8)
                throw new Exception($"Código {current} não tem 8 dígitos, não foi possível gerar o dígito verificador!");

            int range1, range2, range3, range4, range5, range6, range7, range8, result, module;

            range1 = Convert.ToInt32(current[0]);
            range2 = Convert.ToInt32(current[1]);
            range3 = Convert.ToInt32(current[2]);
            range4 = Convert.ToInt32(current[3]);
            range5 = Convert.ToInt32(current[4]);
            range6 = Convert.ToInt32(current[5]);
            range7 = Convert.ToInt32(current[6]);
            range8 = Convert.ToInt32(current[7]);

            result = (range1 * 8) + (range2 * 6) + (range3 * 4) + (range4 * 2) + (range5 * 3) + (range6 * 5) + (range7 * 9) + (range8 * 7);
            module = result % 11;

            if (module == 0)
                module = 5;
            else if (module == 1)
                module = 0;
            else
                module = 11 - module;

            return module.ToString()[0];
        }
    }
}