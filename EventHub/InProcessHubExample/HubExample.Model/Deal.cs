namespace HubExample.Model;

public class Deal
{
    public Guid Id { get; set; }

    /// <summary>Both the order it was created and the order it was sent.</summary>
    public int Order { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public int NumberOfYearsToKeepOnRecord { get; set; }
    public bool IsGoodDeal { get; set; }
}