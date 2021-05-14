namespace Alexandra.Common.Types
{
    public class CodeBlock
    {
        public string Text { get; set; }
        public string Language { get; set; }

        public CodeBlock(string text)
        {
            Text = text;
        }

        public CodeBlock(string text, string language)
        {
            Text = text;
            Language = language;
        }

        public override string ToString()
        {
            return $"```{Language ?? ""}\n{Text}\n```";
        }
    }
}