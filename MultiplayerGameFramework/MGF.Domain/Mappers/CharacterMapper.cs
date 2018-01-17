using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using MGF.Domain;

namespace MGF.Mappers
{
    public class CharacterMapper : MapperBase<Character>
    {
        protected override Character Delete(Character domainObject)
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
                DataEntities.Character entity = new DataEntities.Character { Id = id };
                // gets the character list and attaches the entity to the container (makes this object exist in the list of objects)
                entities.Characters.Attach(entity);
                // remove the character from the container
                entities.Characters.Remove(entity);
                entities.SaveChanges();
            }
        }

        // get a list of EVERY ENTRY in the database
        protected override IList<Character> Fetch()
        {
            using (MGFContext entities = new MGFContext())
            {
                return entities.Characters
                    // Do not cache the entities in EntityFramework
                    .AsNoTracking()
                    //order the entities by ID
                    .OrderBy(characterEntity => characterEntity.Id)
                    // Execute the queru and return a list of entities
                    .ToList()
                    // Using the list of entities, create new DomainBase Characters
                    .Select(characterEntity => new Character(
                        characterEntity.Id,
                        characterEntity.Name))
                    // return a List<Character> of Characters.
                    .ToList();
            }
        }

        protected override Character Fetch(int id)
        {
            Character characterObject = null;
            using (MGFContext entities = new MGFContext())
            {
                DataEntities.Character entity = entities.Characters
                    // Eagerly grab this entities linked object - stats
                    //.Include(characterEntity => characterEntity.Stats)
                    .FirstOrDefault(characterEntity => characterEntity.Id == id);

                if (entity != null)
                {
                    // Load all data, such as linked objects or XML data etc
                    characterObject = new Character(entity.Id, entity.Name);
                }
            }

            return characterObject;
        }

        protected override Character Insert(Character domainObject)
        {
            using (MGFContext entities = new MGFContext())
            {
                DataEntities.Character entity = new DataEntities.Character();
                Map(domainObject, entity);
                entities.Characters.Add(entity);
                domainObject = SaveChanges(entities, entity);
            }

            return domainObject;
        }


        // One way mapping of all data in the domain object to the entity for adding/updating
        protected override void Map(Character domainObject, object entity)
        {
            DataEntities.Character characterEntity = entity as DataEntities.Character;

            if (null == domainObject)
            {
                throw new ArgumentNullException(nameof(domainObject));
            }

            if (null == entity)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (null == characterEntity)
            {
                throw new ArgumentOutOfRangeException(nameof(entity));
            }

            // map all fields from th domain object to the entity except the ID if it isnt allowed to change ( most IDs should never be changed)
            //characterEntity.Id = domainObject.Id;
            characterEntity.Name = domainObject.Name;
            foreach (Stat stat in domainObject.Stats)
            {
                DataEntities.Stat statEntity = null;
                StatMapper mapper = new StatMapper();
                mapper.MapStat(stat, statEntity);
                characterEntity.Stats.Add(statEntity);
            }
        }

        protected override Character Update(Character domainObject)
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
                DataEntities.Character entity = entities.Characters
                    .Include(characterEntity => characterEntity.Stats)
                    .FirstOrDefault(characterEntity => characterEntity.Id == id);

                if (entity != null)
                {
                    Map(domainObject, entity);
                    domainObject = SaveChanges(entities, entity);
                }
            }

            return domainObject;
        }

        private Character SaveChanges(MGFContext entities, DataEntities.Character entity)
        {
            // save everything in the context (unit of work means it should only be this entity and anything in contains)
            entities.SaveChanges();
            // reload what the database has based on the ID that we modified
            return Fetch(entity.Id);
        }

        public static IList<Stat> LoadStats(Character domainObject)
        {
            int id;
            List<Stat> stats;

            if (null == domainObject)
            {
                throw new ArgumentNullException(nameof(domainObject));
            }

            id = domainObject.Id;

            stats = new List<Stat>();
            using(MGFContext entities = new MGFContext())
            {
                var query = entities.Stats
                    .Where(statEntity => statEntity.CharacterId == id);
                foreach (DataEntities.Stat stat in query)
                {
                    stats.Add(new Stat(stat.StatId, stat.Name, stat.Value));
                }
            }

            return stats;
        }
    }
}
