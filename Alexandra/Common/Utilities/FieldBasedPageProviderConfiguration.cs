namespace Alexandra.Common.Utilities
{
    public class FieldBasedPageProviderConfiguration
    {
        // Thanks Zack for inspiring me
        // https://github.com/Zackattak01/CheeseBot/blob/main/CheeseBot/Disqord/FieldBasedPageProviderConfiguration.cs
        public static FieldBasedPageProviderConfiguration Default =>
            new FieldBasedPageProviderConfiguration().WithFieldsPerPage(5);
        
        public string Content { get; set; }
        
        public int FieldsPerPage { get; set; }

        public FieldBasedPageProviderConfiguration WithFieldsPerPage(int fieldsPerPage)
        {
            FieldsPerPage = fieldsPerPage;
            return this;
        }

        public FieldBasedPageProviderConfiguration WithContent(string content)
        {
            Content = content;
            return this;
        }
    }
}