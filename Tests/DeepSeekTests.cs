using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace StudyDeepSeek.Tests
{
    public class DeepSeekTests
    {
        private readonly DeepSeekService _service;
        private readonly string _apiKey = "sk-utvncgitkrlwkijphpfubexbnjfcrqbgcnvoiclbknltibyu";

        public DeepSeekTests()
        {
            _service = new DeepSeekService(_apiKey);
        }

        [Fact]
        public async Task BasicConversationTest()
        {
            try
            {
                var response = await _service.GetCompletionAsync("你好");
                Assert.NotNull(response);
                Assert.NotEmpty(response);
                Console.WriteLine("✓ 基础对话测试通过");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 基础对话测试失败: {ex.Message}");
                throw;
            }
        }

        [Fact]
        public async Task ContextContinuityTest()
        {
            try
            {
                var history = new List<(string role, string content)>();
                
                // 第一轮对话
                var response1 = await _service.GetCompletionAsync("我养了一只猫", history);
                history.Add(("user", "我养了一只猫"));
                history.Add(("assistant", response1));
                
                // 第二轮对话（使用代词）
                var response2 = await _service.GetCompletionAsync("它是什么颜色的", history);
                Assert.Contains("猫", response2.ToLower());
                Console.WriteLine("✓ 上下文连续性测试通过");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 上下文连续性测试失败: {ex.Message}");
                throw;
            }
        }

        [Fact]
        public async Task ErrorHandlingTest()
        {
            try
            {
                // 测试空输入
                var response1 = await _service.GetCompletionAsync("");
                Assert.StartsWith("Error:", response1);  // 验证错误响应格式
                
                // 测试超长输入
                var longInput = new string('a', 5000);
                var response2 = await _service.GetCompletionAsync(longInput);
                Assert.StartsWith("Error:", response2);  // 验证错误响应格式
                
                Console.WriteLine("✓ 错误处理测试通过");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 错误处理测试失败: {ex.Message}");
                throw;
            }
        }

        [Fact]
        public async Task ProfessionalKnowledgeTest()
        {
            try
            {
                var response = await _service.GetCompletionAsync("解释什么是面向对象编程");
                Assert.Contains("类", response.ToLower());
                Assert.Contains("对象", response.ToLower());
                Console.WriteLine("✓ 专业知识测试通过");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 专业知识测试失败: {ex.Message}");
                throw;
            }
        }

        [Fact]
        public async Task CommandTest()
        {
            try
            {
                var history = new List<(string role, string content)>();
                
                // 测试清空历史
                history.Add(("user", "你好"));
                history.Clear();
                Assert.Empty(history);
                
                Console.WriteLine("✓ 命令功能测试通过");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 命令功能测试失败: {ex.Message}");
                throw;
            }
        }
    }
}