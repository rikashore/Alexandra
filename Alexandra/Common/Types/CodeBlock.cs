namespace Alexandra.Common.Types
{
    public class CodeBlock
    {
        public string Text { get; set; }

        public CodeBlock(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return $"```\n{Text}\n```";
        }
    }
}