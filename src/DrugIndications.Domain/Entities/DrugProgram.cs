namespace DrugIndications.Domain.Entities;
public class DrugProgram
{
    public int Id { get; set; }
    public string ProgramName { get; set; }
    public List<string> CoverageEligibilities { get; set; }
    public string ProgramType { get; set; }
    public List<ProgramEligibilityRequirement> Requirements { get; set; }
    public List<ProgramBenefit> Benefits { get; set; }
    public List<ProgramForm> Forms { get; set; }
    public ProgramFunding Funding { get; set; }
    public List<ProgramDetail> Details { get; set; }
    public string EligibilityDetails { get; set; }

}
