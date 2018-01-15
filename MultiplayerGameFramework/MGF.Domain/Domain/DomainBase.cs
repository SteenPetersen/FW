using System;
using System.ComponentModel;
using System.Xml.Serialization;


// video 21

namespace MGF.Domain
{
    [Serializable()]
    public abstract class DomainBase : IProcessDirty
    {
        // Keep track of whether object is new, deleted or dirty
        private bool isObjectNew = true;
        private bool isObjectDirty = true;
        private bool isObjectDeleted;

        #region IProcessDirty Members

        [Browsable(false)]
        public bool IsNew
        {
            get { return isObjectNew; }
            set { isObjectNew = value; } // only used during deserialization - must be public
        }

        [Browsable(false)]
        public bool IsDirty
        {
            get { return isObjectDirty; }
            set { isObjectDirty = value; } // only used during deserialization - must be public
        }

        [Browsable(false)]
        public bool IsDeleted
        {
            get { return isObjectDeleted; }
            set { isObjectDeleted = value; } // only used during deserialization - must be public
        }


        #endregion

        [NonSerialized()]
        private PropertyChangedEventHandler _nonSerializableHandlers;

        private PropertyChangedEventHandler _serializableHandlers;

        [
            Browsable(false),
            XmlIgnore()
        ]
        public virtual bool IsSavable
        {
            // usually some validation done here.
            get { return isObjectDirty; }
        }


        // pattern from CSLA.Net - a domain Driven Design Pattern based on BindableBase - cslanet.com
        // Necessary to make serialization work properly and more importantly safely.
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (value.Method.IsPublic &&
                    (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic))
                {
                    _serializableHandlers =
                        (PropertyChangedEventHandler)Delegate.Combine(_serializableHandlers, value);
                }
                else
                {
                    _nonSerializableHandlers =
                    (PropertyChangedEventHandler)Delegate.Combine(_nonSerializableHandlers, value);
                }
            }
            remove
            {
                if (value.Method.IsPublic &&
                    (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic))
                {
                    _serializableHandlers =
                        (PropertyChangedEventHandler)Delegate.Remove(_serializableHandlers, value);
                }
                else
                {
                    _nonSerializableHandlers =
                        (PropertyChangedEventHandler)Delegate.Remove(_nonSerializableHandlers, value);
                }
            }
        }

        // automaticall call my markDirty. Refreshes all properties (useful in appålications that need o refresh data)
        protected virtual void OnUnknownPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (_nonSerializableHandlers != null)
            {
                _nonSerializableHandlers.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            if (_serializableHandlers != null)
            {
                _serializableHandlers.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Used by Constructor to denote a brand new object that is not stored in the dataase and ensures it is'nt being marked for deletion
        protected virtual void MarkNew()
        {
            isObjectNew = true;
            isObjectDeleted = false;
            MarkDirty();
        }

        // used by Fetch to denote an object that already excists and has been pulled from the database
        protected virtual void MarkOld()
        {
            isObjectNew = false;
            MarkClean();
        }

        protected void MarkDeleted()
        {
            isObjectDeleted = true;
            MarkDirty();
        }

        // Call any time data changes to denote that the object needs to be saved
        protected void MarkDirty()
        {
            MarkDirty(false);
        }

        protected void MarkDirty(bool supressEvent)
        {
            isObjectDirty = true;
            if (!supressEvent)
            {
                // Force properties to refresh
                OnUnknownPropertyChanged();
            }
        }

        protected void PropertyHasChanged()
        {
            // the first 4 letters of the substring
            PropertyHasChanged(new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4));
        }

        protected virtual void PropertyHasChanged(string propertyName)
        {
            MarkDirty(true);
            OnPropertyChanged(propertyName);
        }

        protected void MarkClean()
        {
            isObjectDirty = false;
        }

        public virtual void Delete()
        {
            this.MarkDeleted();
        }

    }
}
