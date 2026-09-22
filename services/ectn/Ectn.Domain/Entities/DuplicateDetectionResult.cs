using System;
using System.Collections.Generic;

namespace Ectn.Domain.Entities;

public class DuplicateDetectionResult
{
    public double MatchScore { get; set; }
    public Guid MatchedEctnId { get; set; }
    public List<string> Reasons { get; set; } = new();
    public string Severity { get; set; } // Warning, Blocking
}
