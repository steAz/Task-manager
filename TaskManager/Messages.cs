using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    public class AddingMessage
    {
        public Process process; 

        public AddingMessage(Process proc)
        {
            this.process = proc;
        }
    }

    public class DeletingMessage
    {
        public Process process;

        public DeletingMessage(Process proc)
        {
            this.process = proc;
        }
    }

    public class RecoveringMessage
    {
        public Process process;

        public RecoveringMessage(Process proc)
        {
            this.process = proc;
        }
    }

    public class ChangingPriorityMessage
    {
        public Process process;
        public string priorityClass;

        public ChangingPriorityMessage(Process proc, string prior)
        {
            this.process = proc;
            this.priorityClass = prior;
        }
    }

    public class SortingMessage { }

    public class LoadingOnRequestMessage { }
   
}
