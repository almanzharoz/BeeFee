using BeeFee.Model.Projections;

namespace BeeFee.Model.Interfaces
{
	public interface IWithOwner
	{
		BaseUserProjection Owner { get; }
	}
}