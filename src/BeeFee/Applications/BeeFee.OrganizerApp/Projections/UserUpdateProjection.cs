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
			if (Roles.Contains(EUserRole.Organizer))
				//throw  new Exception("Organizer role already exists");
				return this;
			Roles = Roles.Add(EUserRole.Organizer);
			return this;
		}
	}
}