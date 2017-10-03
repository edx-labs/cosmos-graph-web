using System.Collections.Generic;

namespace TechRecruiting.Models
{
    public sealed class Recruiter : Person
    {
        public List<Candidate> Candidates { get; set; }
    }
}