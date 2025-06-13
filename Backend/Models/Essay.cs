namespace Backend.Models
{
    public class Essay
    {
        public int Id { get; set; }
        public string OriginalText { get; set; } // Bài luận gốc
        public string CorrectedText { get; set; } // Bài luận đã sửa lỗi
        public DateTime CreatedAt { get; set; } // Thời gian tạo
    }
}