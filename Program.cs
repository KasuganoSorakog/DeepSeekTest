using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StudyDeepSeek
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 请从 https://cloud.siliconflow.cn/account/ak 获取API密钥
            string apiKey = "sk-utvncgitkrlwkijphpfubexbnjfcrqbgcnvoiclbknltibyu";
            
            var deepSeekService = new DeepSeekService(apiKey);
            var messageHistory = new List<(string role, string content)>(); // 添加消息历史记录
            
            while (true)
            {
                Console.WriteLine("\n请输入您的问题(输入'exit'退出，输入'clear'清空对话历史)：");
                string prompt = Console.ReadLine();
                
                if (prompt?.ToLower() == "exit")
                    break;
                    
                if (prompt?.ToLower() == "clear")
                {
                    messageHistory.Clear();
                    Console.WriteLine("对话历史已清空");
                    continue;
                }
                    
                if (string.IsNullOrWhiteSpace(prompt))
                    continue;
                
                // 添加用户消息到历史记录
                messageHistory.Add(("user", prompt));
                
                Console.WriteLine("正在思考...");
                string response = await deepSeekService.GetCompletionAsync(prompt, messageHistory);
                
                // 添加助手回复到历史记录
                messageHistory.Add(("assistant", response));

                Console.WriteLine(); // 换行
            }
        }
    }
}
