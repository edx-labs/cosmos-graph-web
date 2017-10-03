namespace TechRecruiting.Models
{
    public class Portrait : IEntity
    {
        public string Id { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string ImageAuthorName { get; set; }
        
        public string ImageAuthorId { get; set; }
        
        public string ImageSourceId { get; set; }
    }
}