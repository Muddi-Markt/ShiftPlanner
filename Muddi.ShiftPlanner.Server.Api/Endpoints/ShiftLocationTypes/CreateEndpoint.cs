using Microsoft.EntityFrameworkCore;
using Muddi.ShiftPlanner.Server.Api.Contracts.Requests;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Muddi.ShiftPlanner.Server.Database.Entities;

namespace Muddi.ShiftPlanner.Server.Api.Endpoints.ShiftLocationTypes;

public class CreateEndpoint : MuddiEndpoint<CreateLocationTypeRequest, DefaultCreateResponse>
{
	public CreateEndpoint(ShiftPlannerContext database) : base(database)
	{
	}

	protected override void MuddiConfigure()
	{
		Post("/location-types");
	}

	public override async Task HandleAsync(CreateLocationTypeRequest req, CancellationToken ct)
	{
		var type = new ShiftLocationType()
		{
			Id = Guid.NewGuid(),
			Name = req.Name,
		};
		Database.Add(type);
		await Database.SaveChangesAsync(ct);

		Response.Id = type.Id;
	}
}

public class CreateLocationTypeRequest
{
	public string Name { get; set; }
}