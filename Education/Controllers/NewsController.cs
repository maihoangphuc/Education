using Education.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;

namespace Education.Controllers
{
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "https://api-intern-test.h2aits.com/";

        public NewsController(ILogger<NewsController> logger, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ActionResult> Index(int? page, int? record, int? sequenceStatus, string? searchText, int? schoolId)
        {
            int pageNumber = page ?? 1;
            int pageSize = record ?? 6;
            int status = sequenceStatus ?? 1;
            string search = searchText ?? "";
            int school = schoolId ?? 9;

            if (page < 1)
            {
                pageNumber = 1;
            }

            try
            {
                // Gọi API và xử lý kết quả
                string apiUrl = $"{_baseUri}News/GetListByPaging?sequenceStatus={status}&?record={pageSize}&?page={pageNumber}&searchText={search}&schoolId={school}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    // Model
                    var newsListModel = JsonConvert.DeserializeObject<NewsModel<List<NewsItemModel>>>(apiResponse);

                    // Xử lý kết quả API ở đây
                    var newsLists = newsListModel?.Data;

                    return View(newsLists?.ToList().ToPagedList(pageNumber, pageSize));
                }
            }
            catch (HttpRequestException e)
            {
                // Xử lý lỗi gọi API
                ViewBag.Error = "Error calling API: " + e.Message;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id < 1)
            {
                return RedirectToAction("index", "news");
            }

            try
            {
                string apiUrl = $"{_baseUri}News/GetById?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsAsync<NewsModel<NewsItemModel>>();
                var news = res?.Data;
                return View(news);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }
    }
}
