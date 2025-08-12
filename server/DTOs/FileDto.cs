namespace AIChat1.DTOs
{
    public class FileDto
    {
        public record FileUploadResult(int Id, string Filename, string Url, DateTime UploadedAt);
    }
}
