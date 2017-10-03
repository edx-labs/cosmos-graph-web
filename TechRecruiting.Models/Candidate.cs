using System.Collections.Generic;

namespace TechRecruiting.Models
{
    public sealed class Candidate : Person
    {
        public string SkillDescription { get; set; }

        public List<Candidate> Friends { get; set; }
    }
}