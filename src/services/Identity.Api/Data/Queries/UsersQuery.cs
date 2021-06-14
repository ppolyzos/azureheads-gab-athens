using System;
using System.Linq;
using EventManagement.Core.EF.Entity;
using EventManagement.Core.EF.Extensions;
using EventManagement.Core.EF.Queries.Base;
using Identity.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Data.Queries
{
    public class UsersQuery : UtilityQueryBase<ApplicationUser>
    {
        public bool? EmailConfirmed { get; set; }
        public bool? PhoneConfirmed { get; set; }
        public string UserId { get; set; }

        #region Text Filters

        private readonly TextFieldMapping<ApplicationUser>[] _textFieldMappings =
        {
            new(o => o.Email),
            new(o => o.UserName),
            new(o => o.PhoneNumber),
        };

        #endregion

        public override IQueryable<TModel> BuildQuery<TModel>(DbContext dbContext)
        {
            if (!(dbContext is AuthDbContext db))
                throw new ArgumentNullException($"Invalid dataContext type. Expected {typeof(DbContext)}");

            var query = db.Users.AsQueryable()
                .WithContains(IncludedObjectIds, o => o.Id)
                .WithExcludes(ExcludedObjectIds, o => o.Id)
                .With(TextFieldQueries.Any(), TextFieldQueries.BuildFilter(_textFieldMappings))
                .With(!string.IsNullOrEmpty(UserId), o => o.Id == UserId)
                .With(EmailConfirmed.HasValue, o => o.EmailConfirmed == EmailConfirmed.Value)
                .With(PhoneConfirmed.HasValue, o => o.PhoneNumberConfirmed == PhoneConfirmed.Value)
                .JoinIf(SubQueries.Any(), SubQueries.BuildQueries<ApplicationUser>(dbContext),
                    main => main.Id, sub => sub.Id, (main, sub) => main);

            return IncludeCommon(query) as IQueryable<TModel>;
        }
    }
}