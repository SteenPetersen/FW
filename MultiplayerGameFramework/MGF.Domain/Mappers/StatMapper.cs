using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.Domain;

namespace MGF.Mappers
{
    public class StatMapper : MapperBase<Stat>
    {
        protected override Stat Delete(Stat domainObject)
        {
            if (null == domainObject)
            {
                throw new ArgumentNullException(nameof(domainObject));
            }

            // Immediately call delete now and return the object
            DeleteNow(domainObject.Id);
            return domainObject;
        }

        protected override void DeleteNow(int id)
        {
            using (MGFContext entities = new MGFContext())
            {
                DataEntities.Stat entity = new DataEntities.Stat { StatId = id };
                // gets the Stat list and attaches the entity to the container (makes this object exist in the list of objects)
                entities.Stats.Attach(entity);
                // remove the stat from the container
                entities.Stats.Remove(entity);
                entities.SaveChanges();
            }
        }

        protected override IList<Stat> Fetch()
        {
            using (MGFContext entities = new MGFContext())
            {
                return entities.Stats
                    // Do not cache the entities in EntityFramework
                    .AsNoTracking()
                    //order the entities by ID
                    .OrderBy(statEntity => statEntity.StatId)
                    // Execute the query and return a list of entities
                    .ToList()
                    // Using the list of entities, create new DomainBase Stats
                    .Select(statEntity => new Stat(
                        statEntity.StatId,
                        statEntity.Name,
                        statEntity.Value))
                    // return a List<Stat> of Stats.
                    .ToList();
            }
        }

        protected override Stat Fetch(int id)
        {
            Stat statObject = null;
            using (MGFContext entities = new MGFContext())
            {
                DataEntities.Stat entity = entities.Stats
                    // Eagerly grab this entities linked object - stats
                    //.Include(characterEntity => characterEntity.Stats)
                    .FirstOrDefault(statEntity => statEntity.StatId == id);

                if (entity != null)
                {
                    // Load all data, such as linked objects or XML data etc
                    statObject = new Stat(entity.StatId, entity.Name, entity.Value);
                }
            }

            return statObject;
        }

        protected override Stat Insert(Stat domainObject)
        {
            using (MGFContext entities = new MGFContext())
            {
                DataEntities.Stat entity = new DataEntities.Stat();
                Map(domainObject, entity);
                entities.Stats.Add(entity);
                domainObject = SaveChanges(entities, entity);
            }

            return domainObject;
        }

        protected override void Map(Stat domainObject, object entity)
        {
            DataEntities.Stat statEntity = entity as DataEntities.Stat;

            if (null == domainObject)
            {
                throw new ArgumentNullException(nameof(domainObject));
            }

            if (null == entity)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (null == statEntity)
            {
                throw new ArgumentOutOfRangeException(nameof(entity));
            }

            // map all fields from th domain object to the entity except the ID if it isnt allowed to change ( most IDs should never be changed)
            //characterEntity.Id = domainObject.Id;
            statEntity.Name = domainObject.Name;
            statEntity.Value = domainObject.Value;
        }

        public void MapStat(Stat domainObject, object entity)
        {
            Map(domainObject, entity);
        }

        protected override Stat Update(Stat domainObject)
        {
            // Pull out the id because well be using it in the limbda that might be deferred when im calling and the thread may not have access to the domain objects context
            int id;

            if (null == domainObject)
            {
                throw new ArgumentNullException(nameof(domainObject));
            }

            id = domainObject.Id;
            using (MGFContext entities = new MGFContext())
            {
                DataEntities.Stat entity = entities.Stats
                    .FirstOrDefault(statEntity => statEntity.StatId == id);

                if (entity != null)
                {
                    Map(domainObject, entity);
                    domainObject = SaveChanges(entities, entity);
                }
            }

            return domainObject;
        }

        private Stat SaveChanges(MGFContext entities, DataEntities.Stat entity)
        {
            // save everything in the context (unit of work means it should only be this entity and anything in contains)
            entities.SaveChanges();
            // reload what the database has based on the ID that we modified
            return Fetch(entity.StatId);
        }
    }
}
