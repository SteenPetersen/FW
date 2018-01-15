using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGF.Domain
{
    public interface IProcessDirty : INotifyPropertyChanged
    {
        Boolean IsNew { get; }
        Boolean IsDirty { get; }
        // make a list of deleted object so we dont constantly make calls to a database to delete objects.
        Boolean IsDeleted { get; }
    }
}
