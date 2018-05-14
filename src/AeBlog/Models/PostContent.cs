namespace AeBlog.Models
{
    public class PostContent
    {
        public string Markdown { get; set; }
        public string Html => CommonMark.CommonMarkConverter.Convert(Markdown);
    }
}
