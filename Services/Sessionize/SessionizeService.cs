using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;
using EventManagement.Web.Configuration;
using EventManagement.Web.Services.Sessionize.Models;

namespace EventManagement.Web.Services
{
    public interface ISessionizeService
    {
        Task<IList<Speaker>> FetchSpeakersAsync();
        Task<IEnumerable<Session>> FetchSessionsAsync();
    }

    public class SessionizeService : ISessionizeService
    {
        private readonly SessionizeConfig _config;

        public SessionizeService(IOptions<SessionizeConfig> config)
        {
            _config = config.Value;
            _config.ApiUrl = _config.ApiUrl.Replace("{{EventId}}", _config.EventId);
        }

        public async Task<IList<Speaker>> FetchSpeakersAsync() => await Invoke<Speaker[]>("Speakers", HttpMethod.Get);

        public async Task<IEnumerable<Session>> FetchSessionsAsync()
        {
            var groupSessions = await Invoke<GroupSession[]>("Sessions", HttpMethod.Get);

            var groupSession = groupSessions.FirstOrDefault();
            return groupSession?.Sessions;
        }

        private async Task<T> Invoke<T>(string path, HttpMethod method, IDictionary<string, string> data = null)
        {
            var uri = $"{_config.ApiUrl}/{path}";

            if (method == HttpMethod.Get && data?.Count > 0)
            {
                uri = uri + "?" + GetQueryString(data);
            }

            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = method.ToString();
            request.ContentLength = 0;


            string responseBody;
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse) await Task.Factory.FromAsync(request.BeginGetResponse,
                    request.EndGetResponse, null);
                responseBody = ParseResponse(response);
            }
            catch (WebException e)
            {
                response = (HttpWebResponse) e.Response;
                responseBody = ParseResponse(response);
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseBody).Error;
                if (error != null)
                {
                    throw new Exception($"{error.Code}:{error.Message}");
                }
            }

            return JsonSerializer.Deserialize<T>(responseBody);
        }

        private string ParseResponse(WebResponse response)
        {
            var responseBody = string.Empty;
            using var responseStream = response.GetResponseStream();
            if (responseStream == null) return responseBody;

            using var reader = new StreamReader(responseStream);
            responseBody = reader.ReadToEnd();

            return responseBody;
        }

        private string GetQueryString(IDictionary<string, string> dict)
        {
            var list = dict.Select(item => item.Key + "=" + item.Value).ToList();
            return string.Join("&", list);
        }
    }
}