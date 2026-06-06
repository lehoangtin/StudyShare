using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StudyShare.Services.Interfaces;

namespace StudyShare.Services
{
    public class AIModerationResponse
    {
        public bool isFlagged { get; set; }
        public string reason { get; set; }
    }

    public class ChatResponse
    {
        public string reply { get; set; }
    }

    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "DayLaMatKhauBaoMatCuaToi";

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // Đọc URL từ config: appsettings.json hoặc biến môi trường AIServiceUrl
            var baseUrl = configuration["AIServiceUrl"] ?? "http://127.0.0.1:8000/";
            _httpClient.BaseAddress = new Uri(baseUrl);

            if (!_httpClient.DefaultRequestHeaders.Contains("X-PBL3-API-KEY"))
            {
                _httpClient.DefaultRequestHeaders.Add("X-PBL3-API-KEY", _apiKey);
            }
        }

        // Hàm gửi tin nhắn Chat sang Python và nhận phản hồi
        public async Task<string> ChatWithAIAsync(string message)
        {
            var payload = new { session_id = "default_user", message = message };
            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/chat", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ChatResponse>(responseString);
                    return result?.reply ?? "Xin lỗi, không thể xử lý phản hồi từ AI.";
                }
                else
                {
                    return $"Lỗi Server AI: HTTP {response.StatusCode} - Vui lòng kiểm tra lại API Key hoặc Python Console.";
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi kết nối C#: {ex.Message}";
            }
        }

        // Hàm gửi nội dung sang Python kiểm tra và nhận kết quả có bị chặn hay không
        public async Task<AIModerationResponse> CheckContentAsync(string text)
        {
            var payload = new { text = text };
            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/moderate", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<AIModerationResponse>(responseString, options);

                    return result ?? new AIModerationResponse { isFlagged = false, reason = "An toàn" };
                }
                else
                {
                    return new AIModerationResponse { isFlagged = true, reason = $"Python báo lỗi HTTP: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                return new AIModerationResponse { isFlagged = true, reason = $"Lỗi kết nối C#: {ex.Message}" };
            }
        }
    }
}