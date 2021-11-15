using Popug.Common;

namespace Popug.Messages.Contracts.Values.CUD.Popugs;
/// <summary>
/// First version of CUD event value for popug
/// </summary>
/// <param name="ChipId">Public id for the popug</param>
/// <param name="Name">Popug's name, not proofed to be correct</param>
/// <param name="Role">Current popug's role</param>
public record PopugValue(string ChipId, string Name, Roles Role) : IEventValue
{
    public int Version => 1;
}
