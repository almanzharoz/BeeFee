using System;
using System.Linq;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Projections
{
	internal class UserUpdateProjection : BaseEntity, IProjection<User>, IUpdateProjection, IGetProjection
	{
		public EUserRole[] Roles { get; private set; }

		public UserUpdateProjection(string id, EUserRole[] roles) : base(id)
		{
			Roles = roles;
		}

		public UserUpdateProjection StartOrg()
		{
			if (Roles.Contains(EUserRole.MultiOrganizer))
				return this;
			Roles = Roles.Add(Roles.Contains(EUserRole.Organizer) ? EUserRole.MultiOrganizer : EUserRole.Organizer);
			return this;
		}
	}
}