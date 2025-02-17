using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace StudyDeepSeek
{
    public class DeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string API_URL = "https://api.siliconflow.cn/v1/chat/completions";

        public DeepSeekService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<string> GetCompletionAsync(string prompt, List<(string role, string content)> history = null)
        {
            // 添加输入验证
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return "Error: 输入不能为空";
            }

            if (prompt.Length > 4000)  // 添加长度限制
            {
                return "Error: 输入长度超过限制";
            }

            var messages = new List<object>();
            
            // 添加历史消息
            if (history != null)
            {
                foreach (var (role, content) in history)
                {
                    messages.Add(new { role, content });
                }
            }
            
            // 添加当前消息
            messages.Add(new { role = "user", content = prompt });
            
            var requestBody = new
            {
                model = "deepseek-ai/DeepSeek-V2.5",
                messages = messages.ToArray(),
                stream = true,
                temperature = 0.3,
                top_p = 0.8,
                max_tokens = 2000
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync(API_URL, requestContent);
                response.EnsureSuccessStatusCode();
                
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                
                var fullResponse = new StringBuilder();
                var buffer = new char[1]; // 每次只读取一个字符
                var jsonBuffer = new StringBuilder();
                
                Console.OutputEncoding = Encoding.UTF8;
                
                while (true)
                {
                    var bytesRead = await reader.ReadAsync(buffer, 0, 1);
                    if (bytesRead == 0) break;
                    
                    var c = buffer[0];
                    jsonBuffer.Append(c);
                    
                    if (c == '\n')
                    {
                        var line = jsonBuffer.ToString().TrimEnd();
                        jsonBuffer.Clear();
                        
                        if (line.StartsWith("data: "))
                        {
                            var jsonData = line.Substring(6);
                            if (jsonData == "[DONE]") break;
                            
                            try 
                            {
                                var chunk = JsonSerializer.Deserialize<JsonElement>(jsonData);
                                var messageContent = chunk.GetProperty("choices")[0].GetProperty("delta").GetProperty("content").GetString();
                                if (!string.IsNullOrEmpty(messageContent))
                                {
                                    Console.Write(messageContent);
                                    Console.Out.Flush();
                                    fullResponse.Append(messageContent);
                                    
                                    // 添加小延迟以实现更明显的逐字效果
                                    await Task.Delay(10);
                                }
                            }
                            catch { }
                        }
                    }
                }
                
                Console.WriteLine();
                return fullResponse.ToString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
} 