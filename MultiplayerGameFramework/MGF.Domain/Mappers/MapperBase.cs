using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using MGF.Domain;

namespace MGF.Mappers
{
    // a mapper is a class you use to get data out of a database.
    public abstract class MapperBase<T> where T : DomainBase
    {
        public XDocument ConvertToXML(string rootElementName, string data)
        {
            XElement rootElement = new XElement(rootElementName);

            if (!String.IsNullOrEmpty(data))
            {
                rootElement.Add(XDocument.Parse(data).Root.Nodes());
            }

            return new XDocument(rootElement);
        }

        public void Delete(int id)
        {
            this.DeleteNow(id);
        }

        public T Load(int id)
        {
            return Fetch(id);
        }

        public T Save(T obj)
        {
            if (null == obj) // Null == obj prevents objects null exception when trying to use its Equals Operator.
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!obj.IsSavable)
            {
                // throw new InvalidOperationException("Operation cannot be completed on object in current state");
            }

            if (obj.IsNew)
            {
                obj = this.Insert(obj);
            }
            else if (obj.IsDeleted)
            {
                obj = this.Delete(obj);
            }
            else if (obj.IsDirty)
            {
                obj = this.Update(obj);
            }

            return obj;
        }

        protected abstract T Delete(T domainObject);
        protected abstract void DeleteNow(int id);

        /// Gets all objects of this type and returns a list (e.g. all players, all items etc)
        protected abstract IList<T> Fetch();

        protected abstract T Fetch(int id);
        protected abstract T Insert(T domainObject);
        /// <summary>
        /// Maps an Entity over to an actual domain object, provides seperation from EF6 vs NHIBERNATE classes
        /// </summary>
        /// <param name="domainObject"></param>
        /// <param name="entity"></param>
        protected abstract void Map(T domainObject, object entity);

        /// <summary>
        /// updates the object to the actual database, until update is called, the entiti might have insaved changes, donted by the isDirty flag.
        /// </summary>
        /// <param name="domainObject"></param>
        protected abstract T Update(T domainObject);
    }
}
