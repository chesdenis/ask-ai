using System.Security.Cryptography;
using System.Text;

namespace AskAI.Model;

public static class PromptExtensions
{
    public static string ToStringHash(this Prompt prompt)
    {
        using var md5 = MD5.Create();
        
        var contentBytes = Encoding.UTF8.GetBytes(prompt.content);
        var hashBytes = md5.ComputeHash(contentBytes);
        var hashStringBuilder = new StringBuilder();
        foreach (var b in hashBytes)
        {
            hashStringBuilder.Append(b.ToString("x2"));
        }
        return hashStringBuilder.ToString();
    }
}