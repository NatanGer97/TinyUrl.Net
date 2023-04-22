using System.Text;

namespace TinyUrl.Strategy
{
    public class TinyCodeGenerator : ITinyUrlCodeGenerator
    {
        private const int TINY_SIZE = 4;
        private readonly string CHARS = "ABCDEFHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public string GenerateCode()
        {
            StringBuilder code = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < TINY_SIZE; i++)
            {
                code.Append(CHARS.ElementAt(random.Next(CHARS.Length)));
            }

            return code.ToString();
        }
    }
}
