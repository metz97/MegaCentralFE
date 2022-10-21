using FE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace FE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _Configure;
        private readonly string apiBaseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Configure = configuration;

            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }

        public async Task<IActionResult> Index()
        {
            using (HttpClient client = new HttpClient())
            {
                string endpoint = apiBaseUrl + "/MsStorageLocation";
                using (var Response = await client.GetAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseBody = await Response.Content.ReadAsStringAsync();
                        IList<MsStorageLocation> data = JsonConvert.DeserializeObject<
                            IList<MsStorageLocation>>(responseBody);

                        ViewBag.Location = new SelectList(data, "LocationId", "LocationName", "select");
                        ViewBag.Message = "";
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Failed Get Location");
                        ViewBag.Message = "Failed Get Location";
                        return View();
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(TrBpkbModel formModel)
        {
            using (HttpClient client = new HttpClient())
            {
                string endpoint = apiBaseUrl + "/TrBpkbs";
                var myContent = JsonConvert.SerializeObject(formModel);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var Response = await client.PostAsync(endpoint, byteContent))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        ModelState.Clear();
                        ViewBag.Message = "Save Success";
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Failed Get Location");
                        ViewBag.Message = "Error Save";
                        return View();
                    }
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}