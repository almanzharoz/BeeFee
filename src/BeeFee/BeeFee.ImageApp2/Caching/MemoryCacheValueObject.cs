using System.Collections.Generic;
using System.Linq;
using BeeFee.ImageApp2.Embed;

namespace BeeFee.ImageApp.Caching
{
	public class MemoryCacheValueObject
	{
		public List<EOperationType> OperationTypes { get; }

		public MemoryCacheValueObject(params EOperationType[] types)
		{
			OperationTypes = types.ToList();
		}
	}
}