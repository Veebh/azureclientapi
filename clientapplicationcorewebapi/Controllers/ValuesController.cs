﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;

namespace clientapplicationcorewebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        string targetUrl = "https://webapitargetapplication.azurewebsites.net";
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<string>> GetAsync()
        {
            HttpResponseMessage response = null;
            string accessToken=string.Empty; string content = string.Empty;
            try
            {
                var httpclient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler()
                {
                    //Proxy = new System.Net.WebProxy()
                    //{
                    //    Address = new Uri($"http://webproxy-inet.ms.com:8080"),
                    //    BypassProxyOnLocal = true,
                    //    UseDefaultCredentials = true
                    //}
                });

                var keyvalue = new List<KeyValuePair<string, string>>();
                
                    foreach (var header in HttpContext.Request.Headers)
                    {
                        var key = header.Key;
                        var val = header.Value;
                        httpclient.DefaultRequestHeaders.Add("clientapplicationcorewebapi-" + key, val.ToString());

                    }
                    httpclient.DefaultRequestHeaders.Add("X-MS-EndUserAuthorization", Request.Headers["Authorization"][0]);

                
                var azureServiceTokenProvider2 = new AzureServiceTokenProvider();
                accessToken = await azureServiceTokenProvider2.GetAccessTokenAsync(targetUrl).ConfigureAwait(false);
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = httpclient.GetAsync(targetUrl + "/api/values").GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    content = response.Content.ReadAsStringAsync().Result;
                    return Ok(content);
                }
            }catch(Exception ex)
            {
                content = ex.Message + ex.StackTrace;
                return BadRequest(content);
            }
            return BadRequest(response.ReasonPhrase + Environment.NewLine + content + Environment.NewLine + accessToken);

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
