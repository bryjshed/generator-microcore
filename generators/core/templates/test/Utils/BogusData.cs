using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bogus;

namespace <%= namespace %>.Tests
{
    public class BogusData
    {
        <%_ if(createModel) { _%>
        public static <%= modelName %> Get<%= modelName %>(params KeyValuePair<string, object>[] values)
        {
            return SetProperties(Get<%= modelName %>s()[0], values);
        }

        public static <%= modelName %>Dto Get<%= modelName %>Dto(params KeyValuePair<string, object>[] values)
        {
            return SetProperties(Get<%= modelName %>Dtos()[0], values);
        }

        public static Create<%= modelName %>Dto GetCreate<%= modelName %>Dto()
        {
            return new Faker<Create<%= modelName %>Dto>()
                .Generate();
        }

        public static Update<%= modelName %>Dto GetUpdate<%= modelName %>Dto()
        {
            return new Faker<Update<%= modelName %>Dto>()
                .Generate();
        }

        public static IList<<%= modelName %>Dto> Get<%= modelName %>Dtos(
            int count = 1)
        {
            var dtos = new Faker<<%= modelName %>Dto>()
                .RuleFor(u => u.CreationDate, f => DateTime.UtcNow.FormatDate(Constants.ISODateTimeFormat))
                .RuleFor(u => u.LastUpdatedDate, f => DateTime.UtcNow.FormatDate(Constants.ISODateTimeFormat))
                .RuleFor(u => u.CreatedByDisplayName, f => f.Name.LastName())
                .RuleFor(u => u.CreatedById, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.UpdatedByDisplayName, f => f.Name.LastName())
                .RuleFor(u => u.UpdatedById, f => Guid.NewGuid().ToString())
                .Generate(count).ToList();
            return dtos;
        }

        public static IList<<%= modelName %>> Get<%= modelName %>s(
            int count = 1)
        {
            var models = new Faker<<%= modelName %>>()
                .RuleFor(u => u.CreationDate, f => DateTime.UtcNow)
                .RuleFor(u => u.LastUpdatedDate, f => DateTime.UtcNow)
                .RuleFor(u => u.CreatedByDisplayName, f => f.Name.LastName())
                .RuleFor(u => u.CreatedById, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.UpdatedByDisplayName, f => f.Name.LastName())
                .RuleFor(u => u.UpdatedById, f => Guid.NewGuid().ToString())
                .Generate(count).ToList();
            return models;
        }

        <%_ } _%>
        <%_ if(createService) { _%>

        <%_ for(var i = 0; i < externalServices.length; i++) { _%>
        public static <%= externalServices[i].serviceDtoName %>Dto Get<%= externalServices[i].serviceDtoName %>Dto(params KeyValuePair<string, object>[] values)
        {
            return SetProperties(Get<%= externalServices[i].serviceDtoName %>Dtos()[0], values);
        }
        
        public static IList<<%= externalServices[i].serviceDtoName %>Dto> Get<%= externalServices[i].serviceDtoName %>Dtos(
            int count = 1)
        {
            var dtos = new Faker<<%= externalServices[i].serviceDtoName %>Dto>()
                .Generate(count).ToList();
            return dtos;
        }

        <%_ } _%>
        <%_ } _%>
        public static string RandomString(int length)
        {
            var faker = new Faker();
            return faker.Lorem.Random.AlphaNumeric(length);
        }

        private static E SetProperties<E>(E entity, params KeyValuePair<string, object>[] values)
        {
            var myType = entity.GetType();
            foreach (var val in values)
            {
                var pinfo = myType.GetProperty(val.Key);
                if (pinfo == null)
                {
                    throw new InvalidOperationException("The property does not belong to this object.");
                }
                pinfo.SetValue(entity, val.Value, null);
            }
            return entity;
        }
    }
}
