using E_TestHub.Models;

namespace E_TestHub.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<string?> GetRawAsync(string endpoint); // Get raw JSON string
        Task<T?> PostAsync<T>(string endpoint, object data);
        Task<string?> PostRawAsync(string endpoint, object data); // Post and get raw JSON response
        Task<T?> PutAsync<T>(string endpoint, string id, object data);
        Task<bool> DeleteAsync(string endpoint, string id);
        Task<T?> PostAsyncWithoutResponse<T>(string endpoint, object data);
        void SetAuthToken(string token);
        void ClearAuthToken();
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:3000/api";
            // Ensure BaseAddress ends with /
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(
                int.Parse(_configuration["ApiSettings:Timeout"] ?? "30")
            );

            // Set default headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _logger.LogInformation("Auth token set in ApiService");
        }

        public void ClearAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _logger.LogInformation("Auth token cleared from ApiService");
        }

        private void InjectTokenFromSession()
        {
            // Get token from current HTTP session
            var token = _httpContextAccessor.HttpContext?.Session.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _logger.LogInformation("Token injected from session for API request");
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                // Inject token from session before every request
                InjectTokenFromSession();
                
                _logger.LogInformation($"API GET Request: {endpoint}");
                var response = await _httpClient.GetAsync(endpoint);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"API request failed: {response.StatusCode} - {endpoint}");
                    return default(T);
                }
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response: {content.Substring(0, Math.Min(200, content.Length))}...");
                
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                
                return System.Text.Json.JsonSerializer.Deserialize<T>(content, options);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP Error calling API endpoint: {endpoint}");
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling API endpoint: {endpoint}");
                return default(T);
            }
        }

        public async Task<string?> GetRawAsync(string endpoint)
        {
            try
            {
                // Inject token from session before every request
                InjectTokenFromSession();
                
                _logger.LogInformation($"API GET Request (Raw): {endpoint}");
                var response = await _httpClient.GetAsync(endpoint);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"API request failed: {response.StatusCode} - {endpoint}");
                    return null;
                }
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response (Raw): {content.Substring(0, Math.Min(200, content.Length))}...");
                
                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP Error calling API endpoint: {endpoint}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling API endpoint: {endpoint}");
                return null;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                // Inject token from session before every request
                InjectTokenFromSession();
                
                _logger.LogInformation($"API POST Request: {endpoint}");
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"API request failed: {response.StatusCode} - {errorContent}");
                    return default(T);
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...");
                
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                
                return System.Text.Json.JsonSerializer.Deserialize<T>(responseContent, options);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP Error calling API endpoint: {endpoint}");
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling API endpoint: {endpoint}");
                return default(T);
            }
        }

        public async Task<T?> PostAsyncWithoutResponse<T>(string endpoint, object data)
        {
            try
            {
                // Inject token from session before every request
                InjectTokenFromSession();
                
                _logger.LogInformation($"API POST Request (no response): {endpoint}");
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"API request failed: {response.StatusCode} - {errorContent}");
                    return default(T);
                }
                
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling API endpoint: {endpoint}");
                return default(T);
            }
        }

        public async Task<string?> PostRawAsync(string endpoint, object data)
        {
            try
            {
                // Inject token from session before every request
                InjectTokenFromSession();
                
                _logger.LogInformation($"API POST Request (Raw): {endpoint}");
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"API request failed: {response.StatusCode} - {errorContent}");
                    return null;
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response (Raw): {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...");
                
                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP Error calling API endpoint: {endpoint}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling API endpoint: {endpoint}");
                return null;
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, string id, object data)
        {
            try
            {
                // Inject token from session before every request
                InjectTokenFromSession();
                
                _logger.LogInformation($"API PUT Request: {endpoint}/{id}");
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{endpoint}/{id}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"API request failed: {response.StatusCode} - {errorContent}");
                    return default(T);
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                
                return System.Text.Json.JsonSerializer.Deserialize<T>(responseContent, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling API endpoint: {endpoint}/{id}");
                return default(T);
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, string id)
        {
            try
            {
                // Inject token from session before every request
                InjectTokenFromSession();
                
                _logger.LogInformation($"API DELETE Request: {endpoint}/{id}");
                var response = await _httpClient.DeleteAsync($"{endpoint}/{id}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"API request failed: {response.StatusCode} - {errorContent}");
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling API endpoint: {endpoint}/{id}");
                return false;
            }
        }
    }
}

